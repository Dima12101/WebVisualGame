using System;
using System.IO;
using System.Text;

namespace GameTextParsing
{
    class Program
    {
        static readonly UTF8Encoding encoder = new UTF8Encoding();

        static readonly string path = "game.txt";

        static GameTextParser parser = new GameTextParser();

        static void Main(string[] args)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                byte[] buffer = new byte[1024];

                int bytes = fs.Read(buffer, 0, buffer.Length);

                string sourceText = encoder.GetString(buffer, 3, bytes - 3);

                parser.ParseGameText(sourceText);

                var cond = parser.MyParseTree.Root.ChildNodes[0].ChildNodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[0];

                var exp = parser.ConvertConditionParseTree(cond);
                var strExpr = parser.ParseCondition(exp);
            }
        }
    }
}
