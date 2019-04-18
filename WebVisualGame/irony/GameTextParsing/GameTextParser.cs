using Irony.Parsing;
using System;
using System.Collections.Generic;

using GameTextParsing.GLan;

namespace GameTextParsing
{
    class GameTextParser
    {
        private Parser MyParser { get; set; }

        public ParseTree MyParseTree { get; set; }

        private DialogPoint Start { get; set; }

        private IdentifierDictionary<string> DPointIDs { get; set; }

        private IdentifierDictionary<string> KeyIDs { get; set; }

        private List<DialogPoint> DPoints { get; set; }

        public List<string> Messages { get; private set; }

        private string defaultAnswer = "next...";

        public GameTextParser()
        {
            Grammar grammar = new Glan();
            MyParser = new Parser(grammar);

            DPointIDs = new IdentifierDictionary<string>();
            KeyIDs = new IdentifierDictionary<string>();
            DPoints = new List<DialogPoint>();
            Messages = new List<string>();
        }

        public void ParseGameText(string sourceText)
        {
            try
            {
                MyParser.Parse(sourceText);

                MyParseTree = MyParser.Context.CurrentParseTree;

                var dialogPoint = MyParseTree.Root.ChildNodes;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        #region helpers

        private List<string> GetChildTokenList(ParseTreeNode ptn)
        {
            var tokenList = new List<string>();

            if (ptn.ChildNodes != null)
            {
                foreach (var child in ptn.ChildNodes)
                {
                    tokenList.Add(child.GetText());
                }
            }

            return tokenList;
        }

        #endregion

        #region processing parse tree

        private GameAction[] ParseActionBlock(ParseTreeNode actBlockNode)
        {
            if (actBlockNode == null)
            {
                return null;
            }

            GameAction action = new GameAction();

            var actionBlock = actBlockNode.ChildNodes.ToArray();

            for (int i = 0; i < actionBlock.Length / 2; i += 2)
            {
                string actionType = actionBlock[i].GetText();

                var keyList = GetChildTokenList(actionBlock[i + 1]);

                foreach (var key in keyList)
                {
                    int keyID = KeyIDs.Add(key, out bool contains);

                    if (actionType.Equals(Trm.Find))
                    {
                        action.FindList.Add(keyID);
                    }
                    else if (actionType.Equals(Trm.Lose))
                    {
                        action.LoseList.Add(keyID);
                    }
                }
            }

            return new GameAction[] { action };
        }

        private enum NodeType { Binary, Single, Leave }

        private class TreeNode
        {
            public TreeNode()
            {
                Id = IdCounter++;
            }

            public object Content { get; set; }
            public TreeNode[] Chld { get; set; }
            public NodeType NType { get; set; }

            private int Id { get; set; }

            private static int IdCounter = 0;
        }

        private TreeNode CondParseTreeToExprTree(ParseTreeNode condition)
        {
            Queue<ParseTreeNode> condQueue = new Queue<ParseTreeNode>();
            Queue<TreeNode> nodeQueue = new Queue<TreeNode>();

            TreeNode root = new TreeNode();

            condQueue.Enqueue(condition);
            nodeQueue.Enqueue(root);

            while (condQueue.Count > 0)
            {
                var cond = condQueue.Dequeue();
                var node = nodeQueue.Dequeue();

                // excluding  empty nodes
                while (cond.ChildNodes != null &&
                    cond.ChildNodes.Count == 1 &&
                    cond.ChildNodes[0].GetName() == NTrm.Condition)
                {
                    cond = cond.ChildNodes[0];
                }

                var chTokens = GetChildTokenList(cond);

                if (chTokens.Count == 0)
                {
                    node.Chld = null;
                    node.NType = NodeType.Leave;
                    node.Content = cond.GetText();
                }
                else if (chTokens.Count == 1)
                {
                    node.Chld = null;
                    node.NType = NodeType.Leave;
                    node.Content = cond.GetText();
                }
                else if (chTokens.Contains(Trm.And) || chTokens.Contains(Trm.Or))
                {
                    node.Chld = new TreeNode[2];
                    node.NType = NodeType.Binary;
                    node.Content = cond.ChildNodes[1].GetText();

                    node.Chld[0] = new TreeNode();
                    node.Chld[1] = new TreeNode();

                    nodeQueue.Enqueue(node.Chld[0]);
                    nodeQueue.Enqueue(node.Chld[1]);

                    condQueue.Enqueue(cond.ChildNodes[0]);
                    condQueue.Enqueue(cond.ChildNodes[2]);
                }
                else if (chTokens.Contains(Trm.Not))
                {
                    node.Chld = new TreeNode[1];
                    node.NType = NodeType.Single;
                    node.Content = cond.ChildNodes[0].GetText();

                    node.Chld[0] = new TreeNode();

                    nodeQueue.Enqueue(node.Chld[0]);

                    condQueue.Enqueue(cond.ChildNodes[1]);
                }
                else
                {
                    // temp, for debug
                    throw new BusinessLogicError("Met condition with strange structure");
                }
            }
            return root;
        }

        private string ProcessCondition(ParseTreeNode condNode)
        {
            TreeNode condTree = CondParseTreeToExprTree(condNode);

            Dictionary<string, string> operatorDict = new Dictionary<string, string>
            {
                { Trm.And, "&" },
                { Trm.Or, "|" },
                { Trm.Not, "-" }
            };

            string cond = "";
            
            Dictionary<TreeNode, int> nextChIndex = new Dictionary<TreeNode, int>();
            Stack<TreeNode> nodeStack = new Stack<TreeNode>();

            nodeStack.Push(condTree);
            nextChIndex.Add(condTree, 0);

            while (nodeStack.Count > 0)
            {
                var node = nodeStack.Peek();
                if (node.NType == NodeType.Leave)
                {
                    int id = KeyIDs.Add((string)node.Content, out bool contains);
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
                Actions = ParseActionBlock(answer.GetChild(NTrm.ActionBlock)),
                NextID = DPointIDs.Add(answer.GetChild(NTrm.NextPointMark).GetText(), out bool tmp)
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
                LinkCondition cond = new LinkCondition { BoolExpr = expr };
                foreach (var link in linkArr)
                {
                    link.Condition = cond;
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
                    
                    if (elseIfList != null)
                    {
                        string goNextCond = ifExpr;

                        for (int i = 0; i < elseIfList.Length; ++i)
                        {
                            var currIfCond = ProcessCondition(elseIfList[i].GetChild(NTrm.Condition));

                            var currAnswerBlock = ParseTransitionUnion(elseIfList[i].GetChild(NTrm.AnswerUnion));

                            var totalCond = $"{goNextCond}{currIfCond}& ";

                            SetExprToLinks(currAnswerBlock, totalCond);

                            linkList.AddRange(currAnswerBlock);

                            goNextCond = $"{goNextCond}{currIfCond}- & ";
                        }
                    }
                }
                else if (answerType.Equals(NTrm.Answer))
                {
                    linkList.Add(ProcessSingleTransition(answer));
                }
            }

            return linkList.ToArray();
        }

        private int ProcessDialogPointSequence(int pointIdentifier, List<string> textList)
        {
            int currID = pointIdentifier;

            var textBlock = textList.ToArray();

            for(int i = 0; i < textBlock.Length - 1; ++i)
            {
                DialogPoint currDP = new DialogPoint
                {
                    ID = currID,

                    Text = textBlock[i]
                };

                currID = DPointIDs.UniqueID();

                DialogLink link = new DialogLink
                {
                    Text = defaultAnswer,
                    NextID = currID
                };

                currDP.Links = new DialogLink[] { link };

                DPoints.Add(currDP);
            }

            return currID;
        }

        public DialogPoint ProcessDialogPoint(ParseTreeNode dp)
        {
            // point identifier in string format
            var pointIdentifier = dp.GetChild(NTrm.DialogPointMark).GetText();

            // adds string identifier and gets int-ID
            var pointID = DPointIDs.Add(pointIdentifier, out bool contains);

            if (contains)
            {
                Messages.Add($"Identifier already exist (Line: {dp.Token.Location.Line}, Pos: {dp.Token.Location.Position})");
                throw new SourceCodeError();
            }

            // dialog point can contain multiple text
            var textList = GetChildTokenList(dp.GetChild(NTrm.TextBlock));

            // converting multiple-text-dialog point to a single-text dialog point
            pointID = ProcessDialogPointSequence(pointID, textList);

            // parsing action block of the DP into GameAction[]
            var actionBlock = ParseActionBlock(dp.GetChild(NTrm.ActionBlock));

            DialogLink[] links = null;

            if (dp.GetChild(NTrm.NextPointMark) != null)
            {
                var nextDP = dp.GetChild(NTrm.NextPointMark).GetText();

                links = new DialogLink[1];

                links[0] = new DialogLink
                {
                    Actions = null,
                    Condition = null,
                    NextID = DPointIDs.Add(nextDP, out bool no_matter),
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

        public bool ProcessParseTree()
        {
            if (MyParseTree == null)
            {
                throw new BusinessLogicError("MyParseTree cannot be null");
            }

            if (MyParseTree.HasErrors())
            {
                foreach (var m in MyParseTree.ParserMessages)
                {
                    Messages.Add($"{m.Level.ToString()}: {m.Message} at ({m.Location.Line}, {m.Location.Column})");
                }
                return false;
            }

            var root = MyParseTree.Root;

            var dpNodes = root.ChildNodes;

            foreach(var p in dpNodes)
            {
                if (p.GetName().Equals("DialogPoint"))
                {
                    var dp = ProcessDialogPoint(p);
                    DPoints.Add(dp);
                }
                else if (p.Tag.Equals("SwitchPoint"))
                {
                    //ProcessSwitchPoint(p);
                }
            }

            return true;
        }

        #endregion
    }
}