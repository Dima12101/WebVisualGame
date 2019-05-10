using GameTextParsing.GLan;
using Irony.Parsing;
using System.Collections.Generic;

namespace GameTextParsing.ParseTreeProcessingHelpers
{
    public enum NodeType { Binary, Single, Leave }

    public class ExprNode
    {
        public object Content { get; set; }
        public ExprNode[] Chld { get; set; }
        public NodeType NType { get; set; }
    }

    static class CondTreeConverter
    {
        public static ExprNode CondParseTreeToExprTree(ParseTreeNode condition)
        {
            Queue<ParseTreeNode> condQueue = new Queue<ParseTreeNode>();
            Queue<ExprNode> nodeQueue = new Queue<ExprNode>();

            ExprNode root = new ExprNode();

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

                var chTokens = cond.GetChildTokenList();

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
                    node.Chld = new ExprNode[2];
                    node.NType = NodeType.Binary;
                    node.Content = cond.ChildNodes[1].GetText();

                    node.Chld[0] = new ExprNode();
                    node.Chld[1] = new ExprNode();

                    nodeQueue.Enqueue(node.Chld[0]);
                    nodeQueue.Enqueue(node.Chld[1]);

                    condQueue.Enqueue(cond.ChildNodes[0]);
                    condQueue.Enqueue(cond.ChildNodes[2]);
                }
                else if (chTokens.Contains(Trm.Not))
                {
                    node.Chld = new ExprNode[1];
                    node.NType = NodeType.Single;
                    node.Content = cond.ChildNodes[0].GetText();

                    node.Chld[0] = new ExprNode();

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
    }
}
