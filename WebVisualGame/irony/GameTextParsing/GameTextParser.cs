using Irony.Parsing;
using System;
using System.Collections.Generic;
using GameTextParsing.GLan;
using GameTextParsing.ParseTreeProcessingHelpers;
using System.Collections;

namespace GameTextParsing
{
    public class MessageBuffer : IEnumerable<string>
    {
        private List<string> Messages { get; set; }

        public MessageBuffer()
        {
            Messages = new List<string>();
        }

        public void PutMsg(string m)
        {
            Messages.Add(m);
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Messages.GetEnumerator();
        }
    }

    public class GameParseMetadata
    {
        public GameParseMetadata()
        {
            DialogPointIdDict = new IdMaker<string>();
            KeyIdDict = new IdMaker<string>();
            DialogPoints = new Dictionary<int, DialogPoint>();
            SwitchPoints = new Dictionary<int, SwitchPoint>();
        }

        public DialogPoint Start { get; set; }

        public IdMaker<string> DialogPointIdDict { get; set; }

        public IdMaker<string> KeyIdDict { get; set; }

        public Dictionary<int, DialogPoint> DialogPoints { get; set; }

        public Dictionary<int, SwitchPoint> SwitchPoints { get; set; }
    }

    public class ParsedGameAnalyzer
    {
        public bool Analyze(GameParseMetadata meta, MessageBuffer messages)
        {
            return
            CheckEntryPoints(meta, messages) &
            CheckHangingTransitions(meta, messages);
        }

        private bool CheckEntryPoints(GameParseMetadata meta, MessageBuffer messages)
        {
            bool hasEntryPoint = false;

            if (meta.DialogPointIdDict.TryGetId("start", out int id))
            {
                hasEntryPoint = meta.DialogPoints.ContainsKey(id) || meta.SwitchPoints.ContainsKey(id);
            }

            if (!hasEntryPoint)
            {
                messages.PutMsg($"Source code should contain entry point (with edentifier 'start')");
            }

            return hasEntryPoint;
        }

        private bool CheckHangingTransitions(GameParseMetadata meta, MessageBuffer messages)
        {
            bool hasNotHangingTransitions = true;

            foreach (var dp in meta.DialogPoints.Values)
            {
                foreach (var link in dp.Links)
                {
                    if (link.NextIdentifier.Equals("")) continue;

                    bool isValidTransition = 
                        meta.DialogPoints.ContainsKey(link.NextID) || meta.SwitchPoints.ContainsKey(link.NextID);

                    if (!isValidTransition)
                    {
                        messages.PutMsg($"Transition №{link.Number} in ({dp.Identifier}) has hanging end-point [{link.NextIdentifier}]");
                        hasNotHangingTransitions = false;
                    }
                }
            }

            foreach (var sp in meta.SwitchPoints.Values)
            {
                foreach (var link in sp.Links)
                {
                    if (link.NextIdentifier.Equals("")) continue;

                    bool isValidTransition =
                        meta.DialogPoints.ContainsKey(link.NextID) || meta.SwitchPoints.ContainsKey(link.NextID);

                    if (!isValidTransition)
                    {
                        messages.PutMsg($"Transition №{link.Number} in ({sp.Identifier}) has hanging end-points: [{link.NextIdentifier}]");
                        hasNotHangingTransitions = false;
                    }
                }
            }

            return hasNotHangingTransitions;
        }
    }

    public class GameTextParser
    {
        #region fields
        private Parser MyParser { get; set; }

        public ParseTree MyParseTree { get; set; }

        private GameParseMetadata Meta { get; set; }

        public MessageBuffer Messages { get; set; }

        //private DialogPoint Start { get; set; }

        //private IdDictionary<string> DialogPointIdDict { get; set; }

        //private IdDictionary<string> KeyIdDict { get; set; }

        //private Dictionary<int, DialogPoint> GameDialogPoints { get; set; }

        //public MessageBuffer Messages { get; private set; }

        private string defaultAnswer = "next...";

        #endregion

        public GameTextParser()
        {
            Grammar grammar = new Glan();
            MyParser = new Parser(grammar);

            Meta = new GameParseMetadata();
            Messages = new MessageBuffer();
        }

        //======================= PROCESSING FUNCTIONS ==========================//

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
                        Messages.PutMsg($"{m.Level.ToString()}: {m.Message} at ({m.Location.Line}, {m.Location.Column})");
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

        public GameParseMetadata ProcessParseTree()
        {
            if (MyParseTree == null)
            {
                throw new BusinessLogicError("MyParseTree cannot be null");
            }

            var root = MyParseTree.Root;

            var gpNodes = ProcessGameBlocks(root.ChildNodes);

            foreach (var gp_pair in gpNodes)
            {
                var p = gp_pair.Item1.ChildNodes[0];
                var style = gp_pair.Item2;

                if (p.GetName().Equals(NTrm.DialogPoint))
                {
                    var dp = ProcessDialogPoint(p, style);

                    Meta.DialogPoints.Add(dp.ID, dp);
                }
                else if (p.GetName().Equals(NTrm.SwitchPoint) || p.GetName().Equals(NTrm.RandomSwitchPoint))
                {
                    var sp = ProcessSwitchPoint(p);
                    Meta.SwitchPoints.Add(sp.ID, sp);
                }
            }

            // analyzing processed source code
            // finding hanging transitions (transitions with not-defined end-point)
            // checking start point
            var analyzer = new ParsedGameAnalyzer();
            var analyzeResult = analyzer.Analyze(Meta, Messages);

            if (!analyzeResult)
            {
                return null;
            }

            // coninue processing:
            // reduce switches
            // reduce actions on dialog points

            ReduceSwitches();

            ReduceDialogPointActions();

			return Meta;
        }

        #region processing parse tree helper functions

        private List<(ParseTreeNode, StyleAttribute)> ProcessGameBlocks(List<ParseTreeNode> gameBlocks)
        {
            List<(ParseTreeNode, StyleAttribute)> gamePoints = new List<(ParseTreeNode, StyleAttribute)>();
            foreach (var block in gameBlocks)
            {
                var gamePoint = block.GetChild(NTrm.GamePoint);
                if (gamePoint != null)
                {
                    gamePoints.Add((gamePoint, null));
                    continue;
                }
                var advGameBlock = block.GetChild(NTrm.AdvancedGameBlock);
                if (advGameBlock != null)
                {
                    var settingBlock = advGameBlock.GetChild(NTrm.SettingBlock);
                    var standBlock = advGameBlock.GetChild(NTrm.StandartGameBlock);
                    if (settingBlock != null && standBlock != null)
                    {
                        var style = ProcessSettingBlock(settingBlock);
                        foreach (var bl in standBlock.ChildNodes)
                        {
                            gamePoints.Add((bl, style));
                        }
                    }
                    else
                    {
                        throw new BusinessLogicError("advGameBlock must contain settingBlock and standartGameBlock");
                    }
                }
                else
                {
                    throw new BusinessLogicError("game block must contain advGameBlock or gamePoint");
                }
            }

            return gamePoints;
        }

        private StyleAttribute ProcessSettingBlock(ParseTreeNode settingBlock)
        {
            string name = settingBlock?.GetChild(NTrm.Setting)?.GetChild(NTrm.BackgroundAttribute)?.GetChild("Name")?.GetText();
            if (name == null) return null;
            StyleAttribute style = new StyleAttribute { BackgroundName = name };
            return style;
        }

        private List<GameAction> ProcessActionBlock(ParseTreeNode actBlockNode)
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
                    int keyID = Meta.KeyIdDict.GetId(key);

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

            return actions;
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
                    int id = Meta.KeyIdDict.GetId((string)node.Content);
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
            string nextIdentifier = answer.GetChild(NTrm.NextPointMark).GetText();
            DialogLink dl = new DialogLink
            {
                Text = answer.GetChild("Text")?.GetText(),
                Actions = ProcessActionBlock(answer.GetChild(NTrm.ActionBlock)),
                NextID = Meta.DialogPointIdDict.GetId(nextIdentifier),
                NextIdentifier = nextIdentifier,
                Condition = ""
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
                dialogLinks[i].Number = i + 1;
            }

            return dialogLinks;
        }

        private List<DialogLink> ProcessDialogTransitionBlock(ParseTreeNode answBlockNode)
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

                    string goNextCond = ifExpr + "- ";

                    if (elseIfList != null)
                    {
                        for (int i = 0; i < elseIfList.Length; ++i)
                        {
                            string currIfCond = ProcessCondition(elseIfList[i].GetChild(NTrm.Condition));

                            var currAnswerUnion = ParseTransitionUnion(elseIfList[i].GetChild(NTrm.AnswerUnion));

                            //string amp = (goNextCond.Equals("")) ? "" : "&";

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

                        var elseConditon = goNextCond;

                        SetExprToLinks(elseAnswerUnion, elseConditon);

                        linkList.AddRange(elseAnswerUnion);
                    }
                }
                else if (answerType.Equals(NTrm.Answer))
                {
                    var newTransition = ProcessSingleTransition(answer);
                    newTransition.Number = linkList.Count;
                    linkList.Add(newTransition);
                }
            }

            return linkList;
        }

        // returns new dialog point id
        private int ProcessDialogPointSequence(int pointIdentifier, List<string> textList, StyleAttribute blockStyle)
        {
            int currID = pointIdentifier;

            var textBlock = textList.ToArray();

            for (int i = 0; i < textBlock.Length - 1; ++i)
            {
                DialogPoint currDP = new DialogPoint
                {
                    ID = currID,
                    Identifier = "",
                    Text = textBlock[i],
                    Style = blockStyle
                };

                currID = Meta.DialogPointIdDict.UniqueID();

                DialogLink link = new DialogLink
                {
                    Text = defaultAnswer,
                    NextID = currID,
                    NextIdentifier = "",
                    Number = 1,
                    Condition = ""
                };

                currDP.Links = new List<DialogLink> { link };

                Meta.DialogPoints.Add(currDP.ID, currDP);
            }

            return currID;
        }

        private DialogPoint ProcessDialogPoint(ParseTreeNode dp, StyleAttribute blockStyle)
        {
            // point identifier in string format
            var pointIdentifier = dp.GetChild(NTrm.DialogPointMark).GetText();

            // adds string identifier and gets int-ID
            var pointID = Meta.DialogPointIdDict.GetId(pointIdentifier);
            bool contains = Meta.DialogPoints.ContainsKey(pointID) || Meta.SwitchPoints.ContainsKey(pointID);

            if (contains)
            {
                Messages.PutMsg($"Identifier already exist (Line: {dp.Span.Location.Line}, Pos: {dp.Span.Location.Column})");
            }

            // dialog point can contain multiple text
            var textList = dp.GetChild(NTrm.TextBlock).GetChildTokenList();

            var dpStyle = ProcessSettingBlock(dp.GetChild(NTrm.SettingBlock));

            // converting multiple-text-dialog point to a single-text dialog point
            pointID = ProcessDialogPointSequence(pointID, textList, dpStyle);

            // parsing action block of the DP into GameAction[]
            var actionBlock = ProcessActionBlock(dp.GetChild(NTrm.ActionBlock));

            if (dpStyle != null)
            {
                blockStyle = dpStyle;
            }

            List<DialogLink> links = null;

            if (dp.GetChild(NTrm.NextPointMark) != null)
            {
                var nextDP = dp.GetChild(NTrm.NextPointMark).GetText();

                var newLink = new DialogLink
                {
                    Actions = null,
                    Condition = "",
                    NextID = Meta.DialogPointIdDict.GetId(nextDP),
                    NextIdentifier = nextDP,
                    Text = defaultAnswer,
                    Number = 1
                };

                links = new List<DialogLink>
                {
                    newLink
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
                Identifier = pointIdentifier,
                Links = links,
                Text = textList[textList.Count - 1],
                Style = blockStyle
            };

            return newDp;
        }

        public SwitchPoint ProcessSwitchPoint(ParseTreeNode sp)
        {
            var typeName = sp.GetName();

            SwitchType type = SwitchType.Determinate;

            if (typeName.Equals(NTrm.SwitchPoint))
            {
                type = SwitchType.Determinate;
            }
            else if (typeName.Equals(NTrm.RandomSwitchPoint))
            {
                type = SwitchType.Probabilistic;
            }
            else
            {
                throw new BusinessLogicError("Error when parsing switch type");
            }

            var name = sp.GetChild(NTrm.DialogPointMark)?.GetText();

            if (name == null)
            {
                throw new BusinessLogicError("switch point must have identifier");
            }

            int id = Meta.DialogPointIdDict.GetId(name);
            bool contains = Meta.DialogPoints.ContainsKey(id) || Meta.SwitchPoints.ContainsKey(id);

            if (contains)
            {
                Messages.PutMsg($"Identifier already exist (Line: {sp.Span.Location.Line}, Pos: {sp.Span.Location.Column})");
            }

            //var actions = ProcessActionBlock(sp.GetChild(NTrm.ActionBlock));

            var caseBlock =
                (type == SwitchType.Determinate) ?
                sp.GetChild(NTrm.CaseBlock)?.ChildNodes :
                sp.GetChild(NTrm.RandomCaseBlock)?.ChildNodes;

            List<SwitchLink> links = new List<SwitchLink>();

            string goNextCondition = "";
            int otherProbability = 100;

            if (caseBlock != null)
            {
                foreach (var _case in caseBlock)
                {
                    string totalCond = "";
                    if (type == SwitchType.Determinate)
                    {
                        var boolExpr = _case.GetChild(NTrm.Condition);
                        string currCond = ProcessCondition(boolExpr);

                        string amp = (goNextCondition.Equals("")) ? "" : "&";

                        totalCond = $"{goNextCondition}{currCond}{amp} ";
                        goNextCondition = $"{goNextCondition}{currCond}- {amp} ";
                    }
                    else if (type == SwitchType.Probabilistic)
                    {
                        var probab = _case.GetChild(NTrm.Probability);
                        otherProbability -= int.Parse(probab.GetText());

                        if (otherProbability < 0)
                        {
                            Messages.PutMsg($"Sum of probabilities couldn't be more than 100% (Line: {_case.Span.Location.Line}, Pos: {_case.Span.Location.Column})");
                        }

                        totalCond = probab.GetText() + "%";
                    }

                    string nextIdentifier = _case.GetChild(NTrm.NextPointMark).GetText();

                    links.Add(new SwitchLink
                    {
                        //Actions = ProcessActionBlock(_case.GetChild(NTrm.ActionBlock)),
                        NextID = Meta.DialogPointIdDict.GetId(nextIdentifier),
                        NextIdentifier = nextIdentifier,
                        Condition = totalCond,
                        Number = links.Count + 1
                    });
                }
            }

            {
                var otherCase =
                    (type == SwitchType.Determinate) ?
                    sp.GetChild(NTrm.OtherCase) :
                    sp.GetChild(NTrm.RandomOtherCase);

                string nextIdentifier = otherCase.GetChild(NTrm.NextPointMark).GetText();

                links.Add(new SwitchLink
                {
                    //Actions = ProcessActionBlock(otherCase.GetChild(NTrm.ActionBlock)),
                    Condition = (type == SwitchType.Determinate) ? goNextCondition : $"{otherProbability}% ",
                    NextID = Meta.DialogPointIdDict.GetId(nextIdentifier),
                    NextIdentifier = nextIdentifier,
                    Number = links.Count + 1
                });
            }

            return new SwitchPoint
            {
                //Actions = actions,
                ID = id,
                Identifier = name,
                Links = links,
                SType = type
            };
        }
        #endregion

        #region reduce switches

        void ReduceSwitches()
        {
            foreach (var pair_id_dp in Meta.DialogPoints)
            {
                var dp = pair_id_dp.Value;

                List<DialogLink> newLinks = new List<DialogLink>();

                foreach (var link in dp.Links)
                {
                    bool goesToSwitch = Meta.SwitchPoints.TryGetValue(link.NextID, out SwitchPoint sp);
                    if (!goesToSwitch)
                    {
                        newLinks.Add(link);
                        continue;
                    };

                    int number = 1;
                    foreach (var switchLink in sp.Links)
                    {
                        string amp = (link.Condition.Equals("")) ? "" : "&";
                        string condition = (sp.SType == SwitchType.Determinate) ?
                            $"{link.Condition}{switchLink.Condition}{amp} " :
                            $"{link.Condition}{link.Number}_{switchLink.Condition}{amp} ";
                        DialogLink newDialogLink = new DialogLink
                        {
                            Actions = link.Actions,
                            Condition = condition,
                            NextID = switchLink.NextID,
                            Number = number++,
                            NextIdentifier = switchLink.NextIdentifier,
                            Text = link.Text
                        };
                        newLinks.Add(newDialogLink);
                    }
                }

                dp.Links = newLinks;
            }
        }

        void ReduceDialogPointActions()
        {
            foreach (var pair_id_dp in Meta.DialogPoints)
            {
                var dp = pair_id_dp.Value;
                
                foreach (var link in dp.Links)
                {
                    var contains = Meta.DialogPoints.TryGetValue(link.NextID, out DialogPoint nextDp);
                    if (!contains) throw new BusinessLogicError();

                    if (nextDp.Actions == null) continue;

                    List<GameAction> newActionList = new List<GameAction>();

                    if (link.Actions != null)
                    {
                        newActionList.AddRange(link.Actions);
                    }

                    foreach (var dpAction in nextDp.Actions)
                    {
                        newActionList.Add(dpAction);
                    }

                    link.Actions = newActionList;
                }
            }
        }

        #endregion
    }
}