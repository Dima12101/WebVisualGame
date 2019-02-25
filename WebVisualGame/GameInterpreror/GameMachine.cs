using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterpreror
{
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

                bool reallyHave = KeyStorage.Contains(conditions[i].keyNumber);

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
                if (actions[i].type == ActionType.FindKey)
                {
                    KeyStorage.Add(actions[i].keyNumber);
                }
                else if (actions[i].type == ActionType.LoseKey)
                {
                    KeyStorage.Remove(actions[i].keyNumber);
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
