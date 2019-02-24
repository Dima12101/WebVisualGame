using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterpreror
{
    enum BaseTokenType
    {
        round_brackets_content, square_brackets_content, text, minus
    }

    enum FinalTokenTypes
    {
        point_number, key_lexem, key_number, lock_type, lock_number, next_point_number, have_lexem, not_lexem, text, minus
    }

    class BaseToken
    {
        public BaseToken(BaseTokenType type, string content = "")
        {
            Content = content;
            Type = type;
        }

        public String Content { get; private set; }

        public BaseTokenType Type { get; private set; }
    }

    struct DialogPointNumber
    {
        int number;
    }

    struct GameText
    {
        string text;
    }

    enum ActionType { FindKey, LoseKey }

    struct GameAction
    {
        public ActionType type;
        public int keyNumber;
    }

    enum ConditionType { HaveNot, Have }

    struct LinkCondition
    {
        public ConditionType type;
        public int keyNumber;
    }

    struct Transition
    {
        int nextPointNumber;
    }
}
