using Irony.Parsing;
using System;
using System.Collections.Generic;

namespace GameTextParsing.ParseTreeProcessingHelpers
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

        public static List<string> GetChildTokenList(this ParseTreeNode ptn)
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
    }

}
