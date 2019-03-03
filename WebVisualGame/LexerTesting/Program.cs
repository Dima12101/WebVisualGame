using System;
using System.IO;
using System.Text;
using GameIterpreror;

namespace LexerTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            UTF8Encoding encoder = new UTF8Encoding();

            Lexer lexer = new Lexer();

            using (FileStream fs = new FileStream("game.txt", FileMode.Open))
            {
                byte[] buffer = new byte[1024];

                fs.Read(buffer, 0, 1024);

                var text = encoder.GetString(buffer);

                var textEnumer = text.GetEnumerator();

                textEnumer.MoveNext();

                var lexemEnumer = lexer.ReadLexems(textEnumer);

                while (lexemEnumer.MoveNext())
                {
                    var lexem = lexemEnumer.Current;

                    Console.WriteLine($"{lexem.Text}: {lexem.Type.ToString()} {lexem.Line}:{lexem.Position}");
                }
            }
        }
    }
}
