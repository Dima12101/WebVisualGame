
using System.Collections.Generic;

namespace GameTextParsing
{
    public enum ActionType { Find, Lose }
    public class GameAction
    {
        public ActionType Action { get; set; }
        public int Key { get; set; }
    }

    public class DialogLink
    {
        public string Text { get; set; }
        public List<GameAction> Actions { get; set; }
        public int NextID { get; set; }
        public string NextIdentifier { get; set; }
        public string Condition { get; set; }
        public int Number { get; set; }

        public DialogLink()
        {
            Actions = new List<GameAction>();
            Condition = "";
        }
    }

    public class DialogPoint
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public string Identifier { get; set; }
        public List<GameAction> Actions { get; set; }
        public List<DialogLink> Links { get; set; }
    }

    public enum SwitchType { Determinate, Probabilistic }

    public class SwitchLink
    {
        public string Condition { get; set; }
        //public GameAction[] Actions { get; set; }
        public int NextID { get; set; }
        public string NextIdentifier { get; set; }
        public int Number { get; set; }
    }

    public class SwitchPoint
    {
        public int ID { get; set; }
        public string Identifier { get; set; }
        public SwitchType SType { get; set; }
        //public GameAction[] Actions { get; set; }
        public List<SwitchLink> Links { get; set; }
    }
}
