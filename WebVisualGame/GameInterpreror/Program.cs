using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterpreror
{

    class Key
    {
        public int ID { get; private set; }
    }

    class DialogLink
    {
        public DialogLink()
        {
            Actions = new GameAction[0];
            Conditions = new LinkCondition[0];
        }

        public string Text { get; set; }
        public int Number { get; set; }
        public GameAction[] Actions { get; set; }
        public DialogPoint NextPoint { get; set; }
        public LinkCondition[] Conditions { get; set; }
    }

    class DialogPoint
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public GameAction[] Actions { get; set; }
        public DialogLink[] Links { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameMachine game = null;
            try
            {
                game = new GameMachine(@"C:\Users\I\Documents\DenisProjects\Game_Examples\game.txt");
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            while (true)
            {
                bool result = false;

                int item = 0;

                while (!result)
                {
                    result = int.TryParse(Console.ReadLine(), out item);
                }

                game.MoveNext(item);
            }

        }
    }
}
