using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterpreror
{
    enum ActionType { FindKey, LoseKey }

    struct GameAction
    {
        public ActionType Type { get; set; }

        public int KeyNumber { get; set; }
    }

    enum ConditionType { HaveNot, Have }

    struct LinkCondition
    {
        public ConditionType Type { get; set; }

        public int KeyNumber { get; set; }
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


    class GameMachine
    {
        private HashSet<int> KeyStorage { get; set; }

        private DialogPoint GameStart { get; set; }

        private DialogPoint CurrentPoint { get; set; }

        private DialogLink[] FilteredCurrentLinks { get; set; }

        private GameMachineReader Reader { get; set; }

        public GameMachine(string path)
        {
            KeyStorage = new HashSet<int>();

            Reader = new GameMachineReader();

            GameStart = Reader.ReadGame(path);

            CurrentPoint = GameStart;

            DoActions(CurrentPoint.Actions);

            RefreshView();
        }

        private bool CheckConditions(LinkCondition[] conditions)
        {
            for (int i = 0; i < conditions.Length; ++i)
            {
                bool haveType = conditions[i].Type == ConditionType.Have;

                bool reallyHave = KeyStorage.Contains(conditions[i].KeyNumber);

                if (haveType != reallyHave) return false;
            }

            return true;
        }

        private DialogLink[] FilterLinks(DialogLink[] links)
        {
            List<DialogLink> tempList = new List<DialogLink>();

            for (int i = 0; i < links.Length; ++i)
            {
                if (CheckConditions(links[i].Conditions))
                {
                    tempList.Add(links[i]);
                }
            }

            return tempList.ToArray();
        }

        private void RefreshView()
        {
            FilteredCurrentLinks = FilterLinks(CurrentPoint.Links);

            Console.WriteLine(CurrentPoint.Text);

            Console.WriteLine();

            for (int i = 0; i < FilteredCurrentLinks.Length; ++i)
            {
                Console.WriteLine($"{i + 1}) {FilteredCurrentLinks[i].Text}");
            }

            Console.WriteLine();
        }

        private void DoActions(GameAction[] actions)
        {
            if (actions == null)
            {
                return;
            }

            for (int i = 0; i < actions.Length; ++i)
            {
                if (actions[i].Type == ActionType.FindKey)
                {
                    KeyStorage.Add(actions[i].KeyNumber);
                }
                else if (actions[i].Type == ActionType.LoseKey)
                {
                    KeyStorage.Remove(actions[i].KeyNumber);
                }
            }
        }

        public void MoveNext(int item)
        {
            --item;

            if (item < 0 || item > FilteredCurrentLinks.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            DoActions(FilteredCurrentLinks[item].Actions);

            CurrentPoint = FilteredCurrentLinks[item].NextPoint;

            DoActions(CurrentPoint.Actions);

            RefreshView();
        }
    }
}
