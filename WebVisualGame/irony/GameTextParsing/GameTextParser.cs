using Irony.Parsing;
using System;
using System.Collections.Generic;

using GameTextParsing.GLan;

namespace GameTextParsing
{
    class IDKeeper<T>
    {
        private Dictionary<T, int> Dict { get; set; }

        private int IdCounter { get; set; }

        public IDKeeper()
        {
            Dict = new Dictionary<T, int>();

            IdCounter = 0;
        }

        public int Add(T element, out bool contains)
        {
            contains = Dict.TryGetValue(element, out int id);

            if (!contains)
            {
                id = ++IdCounter;

                Dict.Add(element, id);
            }

            return id;
        }

        public int UniqueID()
        {
            return ++IdCounter;
        }

        public bool TryGetId(T element, out int id)
        {
            return Dict.TryGetValue(element, out id);
        }
    }

    class GameTextParser
    {
        private Parser MyParser { get; set; }

        public ParseTree MyParseTree { get; set; }

        private DialogPoint Start { get; set; }

        private IDKeeper<string> DPointIDs { get; set; }

        private IDKeeper<string> KeyIDs { get; set; }

        private List<DialogPoint> DPoints { get; set; }

        private string defaultAnswer = "next...";

        public GameTextParser()
        {
            Grammar grammar = new Glan();
            MyParser = new Parser(grammar);

            DPointIDs = new IDKeeper<string>();
            KeyIDs = new IDKeeper<string>();
            DPoints = new List<DialogPoint>();
            
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
                    tokenList.Add(child.FindToken().Text);
                }
            }

            return tokenList;
        }

        private ParseTreeNode GetChild(ParseTreeNode node, string tag)
        {
            return node.ChildNodes?.Find(p => p.Term.Name.Equals(tag));
        }

        #endregion

        #region processing parse tree

        private GameAction[] ParseActionBlock(ParseTreeNode actBlockNode)
        {
            GameAction action = new GameAction();

            var actionBlock = actBlockNode.ChildNodes.ToArray();

            for (int i = 0; i < actionBlock.Length / 2; i += 2)
            {
                string actionType = actionBlock[i].FindToken().Text;

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

        public enum NodeType { Binary, Single, Leave }

        public class TreeNode
        {
            public object Content { get; set; }
            public TreeNode[] Chld { get; set; }
            public NodeType NType { get; set; }
        }

        public TreeNode ConvertConditionParseTree(ParseTreeNode condition)
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
                    cond.ChildNodes[0].Term.Name == NTrm.Condition)
                {
                    cond = cond.ChildNodes[0];
                }

                var chTokens = GetChildTokenList(cond);

                if (chTokens.Count == 0)
                {
                    node.Chld = null;
                    node.NType = NodeType.Leave;
                    node.Content = cond.FindToken().Text;
                }
                else if (chTokens.Count == 1)
                {
                    node.Chld = null;
                    node.NType = NodeType.Leave;
                    node.Content = cond.FindToken().Text;
                }
                else if (chTokens.Contains(Trm.And) || chTokens.Contains(Trm.Or))
                {
                    node.Chld = new TreeNode[2];
                    node.NType = NodeType.Binary;
                    node.Content = cond.ChildNodes[1].Term.Name;

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
                    node.Content = cond.ChildNodes[0].Term.Name;

                    node.Chld[0] = new TreeNode();

                    nodeQueue.Enqueue(node.Chld[0]);

                    condQueue.Enqueue(cond.ChildNodes[1]);
                }
                else
                {
                    // temp, for debug
                    throw new ApplicationException("Met condition with strange structure");
                }
            }
            return root;
        }

        public string ParseCondition(TreeNode condTree)
        {
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
                    cond += (string)node.Content + " ";
                    nodeStack.Pop();
                    continue;
                }

                if (nextChIndex.ContainsKey(node))
                {
                    var nextIndex = nextChIndex[node];

                    if (nextIndex >= node.Chld.Length)
                    {
                        nodeStack.Pop();
                        cond += (string)node.Content + " ";
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

        private DialogLink[] ProcessAnswerBlock(ParseTreeNode answBlockNode)
        {
            var dialogLinks = new List<DialogLink>();

            foreach (var answer in answBlockNode.ChildNodes)
            {
                string answerType = answer.Term.Name;
            }

            return null;
        }

        private int ProcessPointDialogList(int pointIdentifier, List<string> textList)
        {
            int currID = pointIdentifier;

            var textBlock = textList.ToArray();

            for(int i = 0; i < textBlock.Length - 1; ++i)
            {
                DialogPoint currDP = new DialogPoint();

                currDP.ID = currID;

                currDP.Text = textBlock[i];

                currID = DPointIDs.UniqueID();

                DialogLink link = new DialogLink
                {
                    Text = defaultAnswer,
                    Number = 0,
                    NextID = currID
                };

                currDP.Links = new DialogLink[] { link };

                DPoints.Add(currDP);
            }

            return currID;
        }

        private void ProcessDialogPoint(ParseTreeNode dp)
        {
            // point identifier in string format
            var pointIdentifier = GetChild(dp, NTrm.DialogPointMark).FindToken().Text;

            // adds string identifier and gets int-ID
            var pointID = DPointIDs.Add(pointIdentifier, out bool contains);

            if (contains)
            {
                throw new Exception($"Identifier already exist (Line: {dp.Token.Location.Line}, Pos: {dp.Token.Location.Position})");
            }

            // dialog point can contain multiple text
            var textList = GetChildTokenList(GetChild(dp, NTrm.TextBlock));

            // converting multiple-text-dialog point to a single-text dialog point
            pointID = ProcessPointDialogList(pointID, textList);

            // parsing action block of the DP into GameAction[]
            var actionBlock = ParseActionBlock(GetChild(dp, NTrm.ActionBlock));

            
        }

        public void ProcessParseTree()
        {
            if (MyParseTree == null || MyParseTree.Root == null)
            {
                //...
                return;
            }

            var root = MyParseTree.Root;

            var dpNodes = root.ChildNodes;

            foreach(var p in dpNodes)
            {
                if (p.Term.Name.Equals("DialogPoint"))
                {
                    ProcessDialogPoint(p);
                }
                else if (p.Tag.Equals("SwitchPoint"))
                {
                    //ProcessSwitchPoint(p);
                }
            }
        }

        #endregion
    }
}