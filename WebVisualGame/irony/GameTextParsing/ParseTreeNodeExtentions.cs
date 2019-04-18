using Irony.Parsing;
using System;

namespace GameTextParsing
{
    public static class ParseTreeNodeExtensions
    {
        public static ParseTreeNode GetChild(this ParseTreeNode node, string name)
        {
            return node?.ChildNodes?.Find(p => p.Term.Name.Equals(name));
        }

        public static string GetName(this ParseTreeNode node)
        {
            return node.Term?.Name;
        }

        public static string GetText(this ParseTreeNode node)
        {
            return node.FindToken()?.Text;
        }
    }

}
