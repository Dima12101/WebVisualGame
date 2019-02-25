using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterpreror
{
    enum BaseTokenType
    {
        rndBrktContent, sqrBrktContent, text, minus
    }

    enum FinalTokenTypes
    {
        pointNumber, keyLexem, keyNumber, lockType, lockNumber, nextPointNumber, haveLexem, notLexem, text, minus
    }

    class BaseToken
    {
        public BaseToken(BaseTokenType type, string content = "")
        {
            Content = content;
            Type = type;
        }

        public string Content { get; private set; }

        public BaseTokenType Type { get; private set; }
    }
}
