using System;
using System.Collections.Generic;
using System.Linq;

namespace GameIterpreror
{
    public enum LexemType
    {
        NotInitialized,
        Text,
        RndOpen, RndClosen,
        SqrOpen, SqrClosen,
        Minus,
        Identifier,
        If,
        Else,
        Then,
        And,
        Or,
        Sharp,
        Not,
        Find,
        Lose,
        Have,
        Show,
        Hide,
        Divider,
        Comment
        //Was
        //In
    }

    public class Lexem
    {
        public Lexem(int line, int pos)
        {
            Type = LexemType.NotInitialized;

            Text = "";

            Line = line;

            Position = pos;
        }

        public LexemType Type { get; set; }

        public string Text { get; set; }

        public int Line { get; set; }

        public int Position { get; set; }
    }

    public class Lexer
    {

        private int CurrentLine { get; set; }

        private int CurrentChar { get; set; }

        private IEnumerator<char> CharEnumer { get; set; }



        private readonly char[] whiteSpace = new char[] { ' ', '\r', '\t' };

        private readonly char newLine = '\n';

        private readonly char comment = '@';

        private readonly char quote = '"';

        private readonly char eof = '\0';

        private readonly Dictionary<string, LexemType> specWordDict = new Dictionary<string, LexemType>();

        private readonly Dictionary<char, LexemType> specCharDict = new Dictionary<char, LexemType>();


        public Lexer()
        {
            specWordDict.Add("if", LexemType.If);
            specWordDict.Add("else", LexemType.Else);
            specWordDict.Add("then", LexemType.Then);
            specWordDict.Add("have", LexemType.Have);
            specWordDict.Add("lose", LexemType.Lose);
            specWordDict.Add("find", LexemType.Find);
            specWordDict.Add("and", LexemType.And);
            specWordDict.Add("or", LexemType.Or);
            specWordDict.Add("not", LexemType.Not);
            specWordDict.Add("show", LexemType.Show);
            specWordDict.Add("hide", LexemType.Show);

            specCharDict.Add('(', LexemType.RndOpen);
            specCharDict.Add(')', LexemType.RndClosen);
            specCharDict.Add('[', LexemType.SqrOpen);
            specCharDict.Add(']', LexemType.SqrClosen);
            specCharDict.Add('-', LexemType.Minus);
            specCharDict.Add('#', LexemType.Sharp);
            specCharDict.Add(',', LexemType.Divider);
        }

        private char NextNotSpaceChar()
        {
            char c = eof;

            while (CharEnumer.MoveNext())
            {
                CurrentChar++;

                c = CharEnumer.Current;

                if (c == newLine)
                {
                    CurrentChar = -1;

                    CurrentLine++;
                }
                else if (!whiteSpace.Contains(c))
                {
                    break;
                }
            }

            return c;
        }

        private char NextChar()
        {
            char c = eof;

            if (CharEnumer.MoveNext())
            {
                c = CharEnumer.Current;

                if (c == newLine)
                {
                    CurrentChar = -1;

                    CurrentLine++;
                }
                else
                {
                    CurrentChar++;
                }
            }

            return c;
        }

        private char ReadIdentifier(Lexem lexem)
        {
            char c = NextChar();

            while (c != eof)
            {
                if (specCharDict.ContainsKey(c))
                {
                    return c;
                }

                if (whiteSpace.Contains(c))
                {
                    c = NextNotSpaceChar();

                    return c;
                }

                lexem.Text += c;

                c = NextChar();
            }

            return c;
        }

        private char ReadText(Lexem lexem)
        {
            char c = NextChar();

            bool isSpecialSymbol = false;

            while(c != eof)
            {
                if (isSpecialSymbol)
                {
                    lexem.Text += c;

                    isSpecialSymbol = false;
                }
                else if (c == '\\')
                {
                    isSpecialSymbol = true;
                }
                else if (c == quote)
                {
                    c = NextNotSpaceChar();

                    break;
                }
                else
                {
                    lexem.Text += c;
                }
                c = NextChar();
            }

            if (isSpecialSymbol)
            {
                lexem.Text += '\\';
            }

            lexem.Type = LexemType.Text;

            return c;
        }

        private char SkipComment()
        {
            char c;

            do
            {
                c = NextChar();

            } while (c != eof && c != newLine);
            
            return NextNotSpaceChar();  
        }
        
        public IEnumerator<Lexem> ReadLexems(IEnumerator<char> charEnumer)
        {
            CharEnumer = charEnumer;

            CurrentChar = -1;

            CurrentLine = 0;

            char c = NextNotSpaceChar();

            while (c != '\0')
            {
                Lexem currLexem = new Lexem(CurrentLine, CurrentChar);

                if (specCharDict.ContainsKey(c))
                {
                    currLexem.Text += c;

                    specCharDict.TryGetValue(c, out LexemType type);

                    currLexem.Type = type;

                    c = NextNotSpaceChar();

                    yield return currLexem;
                }
                else if (c == comment)
                {
                    c = SkipComment();
                }
                else if (c == quote)
                {
                    c = ReadText(currLexem);

                    yield return currLexem;
                }
                else
                {
                    currLexem.Text += c;

                    c = ReadIdentifier(currLexem);

                    if (specWordDict.ContainsKey(currLexem.Text))
                    {
                        specWordDict.TryGetValue(currLexem.Text, out LexemType type);

                        currLexem.Type = type;
                    }
                    else
                    {
                        currLexem.Type = LexemType.Identifier;
                    }

                    yield return currLexem;
                }
            }
        }
    }
}
