using System;
using System.Collections.Generic;
using System.Text;

namespace GameTextParsing
{
    class GameAction
    {
        public List<int> FindList { get; set; }
        public List<int> LoseList { get; set; }

        public GameAction()
        {
            FindList = new List<int>();
            LoseList = new List<int>();
        }
    }

    class LinkCondition
    {
        public string BoolExpr { get; set; }

        public LinkCondition()
        {
            BoolExpr = "";
        }
    }

    class DialogLink
    {
        public string Text { get; set; }
        public GameAction[] Actions { get; set; }
        public int NextID { get; set; }
        public LinkCondition Condition { get; set; }

        public DialogLink()
        {
            Actions = new GameAction[0];
            Condition = new LinkCondition();
        }
    }

    class DialogPoint
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public GameAction[] Actions { get; set; }
        public DialogLink[] Links { get; set; }
    }
}
