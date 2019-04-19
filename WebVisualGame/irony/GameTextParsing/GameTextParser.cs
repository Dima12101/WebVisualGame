using Irony.Parsing;
using System;
using System.Collections.Generic;
using GameTextParsing.GLan;
using GameTextParsing.ParseTreeProcessingHelpers;

namespace GameTextParsing
{
    class GameTextParser
    {
        #region fields
        private Parser MyParser { get; set; }

        public ParseTree MyParseTree { get; set; }

        private DialogPoint Start { get; set; }

        private IdentifierDictionary<string> DialogPointIdDict { get; set; }

        private IdentifierDictionary<string> KeyIdDict { get; set; }

        private List<DialogPoint> GameDialogPoints { get; set; }

        public List<string> Messages { get; private set; }

        private string defaultAnswer = "next...";

        #endregion

        public GameTextParser()
        {
            Grammar grammar = new Glan();
            MyParser = new Parser(grammar);

            DialogPointIdDict = new IdentifierDictionary<string>();
            KeyIdDict = new IdentifierDictionary<string>();
            GameDialogPoints = new List<DialogPoint>();
            Messages = new List<string>();
        }

        //======================= PROCESSING FUNCTIONS ==========================

        public bool ParseGameText(string sourceText)
        {
            try
            {
                MyParser.Parse(sourceText);
                MyParseTree = MyParser.Context.CurrentParseTree;

                if (MyParseTree.HasErrors())
                {
                    foreach (var m in MyParseTree.ParserMessages)
                    {
                        Messages.Add($"{m.Level.ToString()}: {m.Message} at ({m.Location.Line}, {m.Location.Column})");
                    }
                    return false;
                }
                else
                {
                    var dialogPoint = MyParseTree.Root.ChildNodes;
                    return true;
                }
            }
            catch (Exception e)
            {
                new ApplicationException("Something went wrong while parsing; look inner exception", e);
                return false;
            }
        }

        public void ProcessParseTree()
        {
            if (MyParseTree == null)
            {
                throw new BusinessLogicError("MyParseTree cannot be null");
            }

            var root = MyParseTree.Root;

            var dpNodes = root.ChildNodes;

            foreach (var p in dpNodes)
            {
                if (p.GetName().Equals("DialogPoint"))
                {
                    var dp = ProcessDialogPoint(p);
                    GameDialogPoints.Add(dp);
                }
                else if (p.GetName().Equals("SwitchPoint"))
                {
                    var sp = ProcessSwitchPoint(p);
                }
            }
        }

        #region processing parse tree helper functions

        private GameAction[] ProcessActionBlock(ParseTreeNode actBlockNode)
        {
            if (actBlockNode == null)
            {
                return null;
            }

            List<GameAction> actions = new List<GameAction>();

            var actionBlock = actBlockNode.ChildNodes.ToArray();

            for (int i = 0; i < actionBlock.Length / 2; i += 2)
            {
                string actionType = actionBlock[i].GetText();

                var keyList = actionBlock[i + 1].GetChildTokenList();

                foreach (var key in keyList)
                {
                    int keyID = KeyIdDict.Add(key, out bool contains);

                    if (actionType.Equals(Trm.Find))
                    {
                        actions.Add(new GameAction { Action = ActionType.Find, Key = keyID });
                    }
                    else if (actionType.Equals(Trm.Lose))
                    {
                        actions.Add(new GameAction { Action = ActionType.Lose, Key = keyID });
                    }
                }
            }

            return actions.ToArray();
        }

        private string ProcessCondition(ParseTreeNode condNode)
        {
            ExprNode condTree = CondTreeConverter.CondParseTreeToExprTree(condNode);

            Dictionary<string, string> operatorDict = new Dictionary<string, string>
            {
                { Trm.And, "&" },
                { Trm.Or, "|" },
                { Trm.Not, "-" }
            };

            string cond = "";

            Dictionary<ExprNode, int> nextChIndex = new Dictionary<ExprNode, int>();
            Stack<ExprNode> nodeStack = new Stack<ExprNode>();

            nodeStack.Push(condTree);
            nextChIndex.Add(condTree, 0);

            while (nodeStack.Count > 0)
            {
                var node = nodeStack.Peek();
                if (node.NType == NodeType.Leave)
                {
                    int id = KeyIdDict.Add((string)node.Content, out bool contains);
                    cond += $"{id} ";
                    nodeStack.Pop();
                    continue;
                }

                if (nextChIndex.ContainsKey(node))
                {
                    var nextIndex = nextChIndex[node];

                    if (nextIndex >= node.Chld.Length)
                    {
                        nodeStack.Pop();

                        bool contains = operatorDict.TryGetValue((string)node.Content, out string sign);

                        if (contains)
                        {
                            cond += sign + " ";
                        }
                        else
                        {
                            throw new BusinessLogicError($"Unknown operator {(string)node.Content}");
                        }
                        continue;
                    }

                    var nextCh = node.Chld[nextIndex];
                    nodeStack.Push(nextCh);
                    nextChIndex[node] = nextIndex + 1;
                    nextChIndex.Add(nextCh, 0);
                }
            }

            return cond;
        }

        private DialogLink ProcessSingleTransition(ParseTreeNode answer)
        {
            DialogLink dl = new DialogLink
            {
                Text = answer.GetChild("Text")?.GetText(),
                Actions = ProcessActionBlock(answer.GetChild(NTrm.ActionBlock)),
                NextID = DialogPointIdDict.Add(answer.GetChild(NTrm.NextPointMark).GetText(), out bool tmp)
            };

            return dl;
        }

        private DialogLink[] ParseTransitionUnion(ParseTreeNode answerUnion)
        {
            var answers = answerUnion?.ChildNodes?.ToArray();

            if (answers == null)
            {
                throw new BusinessLogicError("If-statement must have body");
            }

            DialogLink[] dialogLinks = new DialogLink[answers.Length];

            for (int i = 0; i < answers.Length; ++i)
            {
                dialogLinks[i] = ProcessSingleTransition(answers[i]);
            }

            return dialogLinks;
        }

        private DialogLink[] ProcessDialogTransitionBlock(ParseTreeNode answBlockNode)
        {
            var dialogLinks = new List<DialogLink>();

            List<DialogLink> linkList = new List<DialogLink>();

            void SetExprToLinks(IEnumerable<DialogLink> linkArr, string expr)
            {
                foreach (var link in linkArr)
                {
                    link.Condition = expr;
                }
            }

            foreach (var answer in answBlockNode.ChildNodes)
            {
                string answerType = answer.GetName();
                if (answerType.Equals(NTrm.ConditionBlock))
                {
                    var ifBlock = answer.GetChild(NTrm.IfBlock);

                    if (ifBlock == null)
                    {
                        throw new BusinessLogicError("Condition-block doesn't contain if-block");
                    }

                    string ifExpr = ProcessCondition(ifBlock.GetChild(NTrm.Condition));

                    DialogLink[] ifThenBlockAns = ParseTransitionUnion(ifBlock.GetChild(NTrm.AnswerUnion));

                    SetExprToLinks(ifThenBlockAns, ifExpr);

                    linkList.AddRange(ifThenBlockAns);

                    var elseIfList = answer.GetChild(NTrm.ElseIfList)?.ChildNodes?.ToArray();

                    string goNextCond = ifExpr;

                    if (elseIfList != null)
                    {
                        for (int i = 0; i < elseIfList.Length; ++i)
                        {
                            var currIfCond = ProcessCondition(elseIfList[i].GetChild(NTrm.Condition));

                            var currAnswerUnion = ParseTransitionUnion(elseIfList[i].GetChild(NTrm.AnswerUnion));

                            var totalCond = $"{goNextCond}{currIfCond}& ";

                            SetExprToLinks(currAnswerUnion, totalCond);

                            linkList.AddRange(currAnswerUnion);

                            goNextCond = $"{goNextCond}{currIfCond}- & ";
                        }
                    }

                    var elseBlock = answer.GetChild(NTrm.ElseBlock);

                    if (elseBlock != null)
                    {
                        var elseAnswerUnion = ParseTransitionUnion(elseBlock.GetChild(NTrm.AnswerUnion));

                        var elseConditon = goNextCond + "-";

                        SetExprToLinks(elseAnswerUnion, elseConditon);

                        linkList.AddRange(elseAnswerUnion);
                    }
                }
                else if (answerType.Equals(NTrm.Answer))
                {
                    linkList.Add(ProcessSingleTransition(answer));
                }
            }

            return linkList.ToArray();
        }

        // returns new dialog point id
        private int ProcessDialogPointSequence(int pointIdentifier, List<string> textList)
        {
            int currID = pointIdentifier;

            var textBlock = textList.ToArray();

            for (int i = 0; i < textBlock.Length - 1; ++i)
            {
                DialogPoint currDP = new DialogPoint
                {
                    ID = currID,

                    Text = textBlock[i]
                };

                currID = DialogPointIdDict.UniqueID();

                DialogLink link = new DialogLink
                {
                    Text = defaultAnswer,
                    NextID = currID
                };

                currDP.Links = new DialogLink[] { link };

                GameDialogPoints.Add(currDP);
            }

            return currID;
        }

        private DialogPoint ProcessDialogPoint(ParseTreeNode dp)
        {
            // point identifier in string format
            var pointIdentifier = dp.GetChild(NTrm.DialogPointMark).GetText();

            // adds string identifier and gets int-ID
            var pointID = DialogPointIdDict.Add(pointIdentifier, out bool contains);

            if (contains)
            {
                Messages.Add($"Identifier already exist (Line: {dp.Token.Location.Line}, Pos: {dp.Token.Location.Position})");
                throw new SourceCodeError();
            }

            // dialog point can contain multiple text
            var textList = dp.GetChild(NTrm.TextBlock).GetChildTokenList();

            // converting multiple-text-dialog point to a single-text dialog point
            pointID = ProcessDialogPointSequence(pointID, textList);

            // parsing action block of the DP into GameAction[]
            var actionBlock = ProcessActionBlock(dp.GetChild(NTrm.ActionBlock));

            DialogLink[] links = null;

            if (dp.GetChild(NTrm.NextPointMark) != null)
            {
                var nextDP = dp.GetChild(NTrm.NextPointMark).GetText();

                links = new DialogLink[1];

                links[0] = new DialogLink
                {
                    Actions = null,
                    Condition = null,
                    NextID = DialogPointIdDict.Add(nextDP, out bool no_matter),
                    Text = defaultAnswer
                };
            }
            else
            {
                links = ProcessDialogTransitionBlock(dp.GetChild(NTrm.AnswerBlock));
            }

            DialogPoint newDp = new DialogPoint
            {
                Actions = actionBlock,
                ID = pointID,
                Links = links,
                Text = textList[textList.Count - 1]
            };

            return newDp;
        }

        public SwitchPoint ProcessSwitchPoint(ParseTreeNode sp)
        {
            var name = sp.GetChild(NTrm.DialogPointMark)?.GetText();

            if (name == null)
            {
                throw new BusinessLogicError("switch point must have identifier");
            }

            int id = DialogPointIdDict.Add(name, out bool contains);

            if (contains)
            {
                Messages.Add($"Identifier already exist (Line: {sp.Token.Location.Line}, Pos: {sp.Token.Location.Position})");
                throw new SourceCodeError();
            }

            var actions = ProcessActionBlock(sp.GetChild(NTrm.ActionBlock));

            var caseBlock = sp.GetChild(NTrm.CaseBlock)?.ChildNodes;

            List<SwitchLink> links = new List<SwitchLink>();

            if (caseBlock != null)
            {
                foreach (var _case in caseBlock)
                {
                    var probCond = _case.GetChild(NTrm.Probability);
                    var determCond = _case.GetChild(NTrm.Condition);

                    string condition = "";

                    if (determCond != null)
                    {
                        condition = ProcessCondition(determCond);
                    }
                    else if (probCond != null)
                    {
                        condition = probCond.GetText() + "%";
                    }

                    links.Add(new SwitchLink
                    {
                        Actions = ProcessActionBlock(_case.GetChild(NTrm.ActionBlock)),
                        NextID = DialogPointIdDict.Add(_case.GetChild(NTrm.NextPointMark).GetText(), out bool tmp_1),
                        Condition = condition
                    });
                }
            }

            var otherCase = sp.GetChild(NTrm.OtherCase);

            links.Add(new SwitchLink
            {
                Actions = ProcessActionBlock(otherCase.GetChild(NTrm.ActionBlock)),
                Condition = "",
                NextID = DialogPointIdDict.Add(otherCase.GetChild(NTrm.NextPointMark).GetText(), out bool tmp_2)
            });

            return new SwitchPoint
            {
                Actions = actions,
                ID = id,
                Links = links.ToArray(),
                SType = SwitchType.Determinate
            };
        }
    }
    #endregion
}