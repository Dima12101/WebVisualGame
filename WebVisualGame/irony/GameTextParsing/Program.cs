using System.IO;

namespace GameTextParsing
{
    class Program
    {
        static readonly string path = "game.txt";

        static GameTextParser parser = new GameTextParser();

        static void Main(string[] args)
        {
            using (StreamReader fs = new StreamReader(path))
            {
                string sourceText = fs.ReadToEnd();

                parser.ParseGameText(sourceText);

                parser.ProcessParseTree();
            }
        }
    }
}
