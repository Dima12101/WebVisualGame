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
            RandomEventGenerator rand = new RandomEventGenerator(0.5, 0.5, 0.9);
            int i = 0;
            while (true)
            {
                var result = rand.GenerateRandom();

                result.TryGetValue("A", out bool A);

                result.TryGetValue("B", out bool B);

                string Ares = (A) ? "A" : "";
                string Bres = (B) ? "B" : "";


                Console.WriteLine($"Experiment {i}: {Ares} {Bres}");
                ++i;
                Console.ReadKey();
            }

            return;

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
