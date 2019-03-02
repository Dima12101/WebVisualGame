using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GameInterpreror
{
    class UnexpectedEndOfInputError : ApplicationException
    {
        public UnexpectedEndOfInputError(string message) : base("End of input error: " + message)
        {

        }
    }

    class SyntaxError : ApplicationException
    {
        public SyntaxError(string message) : base("Syntax error: " + message)
        {

        }
    }

    interface IGameMachineReader
    {
        DialogPoint ReadGame(string charStream);

        IEnumerator<object> GetWarnings();
    }

    class GameMachineReader : IGameMachineReader
    {
        public GameMachineReader()
        {

        }

        private List<string> warnings = new List<string>();

        private UTF8Encoding encoder = new UTF8Encoding();

        private IEnumerator<char> GetCharSequence(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            const int bufSize = 1024 * 1024;

            byte[] buffer = new byte[bufSize];

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                while (true)
                {
                    int readSymbols = fs.Read(buffer, 0, bufSize);

                    if (readSymbols <= 0)
                    {
                        break;
                    }

                    char[] charArray = encoder.GetChars(buffer);

                    for (int i = 0; i < charArray.Length; ++i)
                    {
                        yield return charArray[i];
                    }
                }
            }
        }

        private enum TokenizerState { ReadNothing, ReadRound, ReadSquare, ReadQuote, ReadSlashInText }

        public IEnumerator<BaseToken> GetBaseTokens(IEnumerator<char> charEnumer)
        {
            TokenizerState state = TokenizerState.ReadNothing;

            string currentToken = "";

            while (charEnumer.MoveNext())
            {
                char c = charEnumer.Current;

                switch (state)
                {
                    case TokenizerState.ReadNothing:
                        {
                            if (c == '-')
                                yield return new BaseToken(BaseTokenType.minus, "-");
                            else if (c == '"')
                                state = TokenizerState.ReadQuote;
                            else if (c == '(')
                                state = TokenizerState.ReadRound;
                            else if (c == '[')
                                state = TokenizerState.ReadSquare;
                            else if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                                break;
                            else
                            {

                            }
                            break;
                        }
                    case TokenizerState.ReadQuote:
                        {
                            if (c == '\\')
                                state = TokenizerState.ReadSlashInText;
                            else if (c == '"')
                            {
                                yield return new BaseToken(BaseTokenType.text, currentToken);

                                currentToken = "";
                                state = TokenizerState.ReadNothing;
                            }
                            else
                                currentToken += c;
                            break;
                        }
                    case TokenizerState.ReadSlashInText:
                        {
                            currentToken += c;
                            state = TokenizerState.ReadQuote;
                            break;
                        }
                    case TokenizerState.ReadRound:
                        {
                            if (c == ')')
                            {
                                yield return new BaseToken(BaseTokenType.rndBrktContent, currentToken);
                                currentToken = "";
                                state = TokenizerState.ReadNothing;
                            }
                            else
                                currentToken += c;
                            break;
                        }
                    case TokenizerState.ReadSquare:
                        {
                            if (c == ']')
                            {
                                yield return new BaseToken(BaseTokenType.sqrBrktContent, currentToken);
                                currentToken = "";
                                state = TokenizerState.ReadNothing;
                            }
                            else
                                currentToken += c;
                            break;
                        }
                }
            }


            switch (state)
            {
                case TokenizerState.ReadQuote:
                    {
                        throw new UnexpectedEndOfInputError("Met end of input while reading text, expected (\")");
                    }
                case TokenizerState.ReadSlashInText:
                    {
                        throw new UnexpectedEndOfInputError("Met end of input after reading \"\\\", expected symbol");
                    }
                case TokenizerState.ReadRound:
                    {
                        throw new UnexpectedEndOfInputError("Met end of input while reading round round content, expected \")\"");
                    }
                case TokenizerState.ReadSquare:
                    {
                        throw new UnexpectedEndOfInputError("Met end of input while reading round square content, expected \"]\"");
                    }
                case TokenizerState.ReadNothing:
                    {
                        break;
                    }
            }
        }


        private enum ReadGameStates
        {
            ReadNothing, ReadPointNumber, ReadPointText,
            ReadPointActions, ReadLinkStart, ReadCondition,
            ReadLinkText, ReadLinkAction, ReadLinkTransition
        }

        private readonly string keyRegExp = "(key#([0-9])+)";

        private GameAction[] ParseActions(string text)
        {
            const string findLexem = "find";

            const string loseLexem = "lose";

            const string divider = ",";

            string[] actionTextArray = text.Split(divider.ToCharArray());

            GameAction[] gameActions = new GameAction[actionTextArray.Length];

            for (int i = 0; i < gameActions.Length; ++i)
            {
                if (string.IsNullOrEmpty(actionTextArray[i]))
                {
                    throw new SyntaxError("missed action between ','");
                }

                string[] tokens = actionTextArray[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                gameActions[i] = new GameAction();

                if (tokens[0].Equals(findLexem))
                {
                    gameActions[i].Type = ActionType.FindKey;
                }
                else if (tokens[0].Equals(loseLexem))
                {
                    gameActions[i].Type = ActionType.LoseKey;
                }
                else
                {
                    throw new SyntaxError($"Expected action: 'lose' or 'find', found: {tokens[0]}");
                }

                if (Regex.IsMatch(tokens[1], keyRegExp))
                {
                    MatchCollection collection = Regex.Matches(tokens[1], "[\\d]+");

                    foreach (Match num in collection)
                    {
                        gameActions[i].KeyNumber = int.Parse(num.Value);
                    }
                }
                else
                {
                    throw new SyntaxError($"Unacceptable key definition: {tokens[0]}");
                }

                if (tokens.Length > 2)
                {
                    throw new SyntaxError($"Incorrect action construction, staring from '{tokens[2]}'");
                }
            }

            return gameActions;
        }

        private LinkCondition[] ParseCondition(string text)
        {
            const string haveLexem = "have";

            const string notLexem = "not";

            const string divider = ",";

            string[] conditionTextArray = text.Split(divider.ToCharArray());

            LinkCondition[] linkConditions = new LinkCondition[conditionTextArray.Length];

            for (int i = 0; i < linkConditions.Length; ++i)
            {
                if (string.IsNullOrEmpty(conditionTextArray[i]))
                {
                    throw new SyntaxError("missed condition between ','"); // 
                }

                string[] tokens = conditionTextArray[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                linkConditions[i] = new LinkCondition();

                if (tokens.Length == 0)
                {
                    throw new SyntaxError($"Empty condition");
                }

                if (!tokens[0].Equals(haveLexem))
                {
                    throw new SyntaxError($"Expected: '{haveLexem}', found: '{tokens[0]}'");
                }

                if (tokens.Length == 1)
                {
                    throw new SyntaxError($"Incorrect condition definition: expected name of the key or 'not' after 'have'");
                }

                if (tokens.Length == 3)
                {
                    if (tokens[1].Equals(notLexem))
                    {
                        linkConditions[i].Type = ConditionType.HaveNot;
                    }
                    else
                    {
                        throw new SyntaxError($"Expected: '{notLexem}', found: '{tokens[0]}'");
                    }
                }
                else if (tokens.Length == 2)
                {
                    linkConditions[i].Type = ConditionType.Have;
                }
                else
                {
                    throw new SyntaxError($"Incorrect condition: {conditionTextArray[i]}");
                }

                int keyPosition = tokens.Length - 1;

                if (!Regex.IsMatch(tokens[keyPosition], keyRegExp))
                {
                    throw new SyntaxError($"Unacceptable key definition: {tokens[0]}");
                }

                MatchCollection collection = Regex.Matches(tokens[keyPosition], "[\\d]+");

                foreach (Match num in collection)
                {
                    linkConditions[i].KeyNumber = int.Parse(num.Value);
                }
            }

            return linkConditions;
        }

        private void AnalizeGameGraph(Dictionary<int, DialogPoint> pointDict)
        {
            // it should contain start-state (with number 0)
            // (throws exception)
            DialogPoint start = null;
            {
                bool containsStart = pointDict.TryGetValue(0, out start);

                if (!containsStart || (start.Links == null || start.Links.Length == 0))
                {
                    throw new ApplicationException("Game doesn't contain the start-state (state with number 0)");
                }
            }

            // looking for defined dialog points, which don't exist
            // (adds warnings)
            using (var dictEnumer = pointDict.GetEnumerator())
            {
                while (dictEnumer.MoveNext())
                {
                    var currPoint = dictEnumer.Current.Value;

                    if (currPoint.Links == null) continue;

                    for (int i = 0; i < currPoint.Links.Length; ++i)
                    {
                        var nextPoint = currPoint.Links[i].NextPoint;

                        if (nextPoint.Links == null || nextPoint.Links.Length == 0)
                        {
                            var notDefinedNextPointWarning = $"in point({currPoint.ID}) in {i} position - " +
                                $"transition to point({nextPoint.ID}), which hadn't been read (incorrect transition)";

                            nextPoint = currPoint;

                            nextPoint.Text += "(INVALID TRANSITION)";

                            warnings.Add(notDefinedNextPointWarning);
                        } 
                    }
                }
            }

            // find not-linked components
            var notConnectedPoints = new Dictionary<int, DialogPoint>();

            using (var dictEnumer = pointDict.GetEnumerator())
            {
                while (dictEnumer.MoveNext())
                {
                    notConnectedPoints.Add(dictEnumer.Current.Key, dictEnumer.Current.Value);
                }
            }

            using (var dictEnumer = notConnectedPoints.GetEnumerator())
            {
                var pointQueue = new Queue<DialogPoint>();

                var connectedPoints = new Dictionary<int, DialogPoint>();

                pointQueue.Enqueue(start);

                while(pointQueue.Count > 0)
                {
                    var currPoint = pointQueue.Dequeue();

                    for (int i = 0; i < currPoint.Links.Length; ++i)
                    {

                    }
                }
            }
            
        }

        private DialogPoint ReadGame(IEnumerator<char> charEnumer)
        {
            ReadGameStates state = ReadGameStates.ReadNothing;

            Dictionary<int, DialogPoint> dialogPointDictionary = new Dictionary<int, DialogPoint>();

            DialogPoint currDialogPoint = new DialogPoint();

            List<DialogLink> linkList = new List<DialogLink>();

            DialogLink currLink = new DialogLink();

            using (var baseTokenEnumer = GetBaseTokens(charEnumer))
            {
                while (baseTokenEnumer.MoveNext())
                {
                    var token = baseTokenEnumer.Current;

                    switch (state)
                    {
                        // reading transition equals to reading nothing
                        case ReadGameStates.ReadLinkTransition:
                        case ReadGameStates.ReadNothing:
                            {
                                // if we have read link transition and met minus, it means - now we're reading new transition
                                if (state == ReadGameStates.ReadLinkTransition && token.Type == BaseTokenType.minus)
                                {
                                    currLink = new DialogLink()
                                    {
                                        Number = linkList.Count,
                                    };
                                    linkList.Add(currLink);
                                    state = ReadGameStates.ReadLinkStart;
                                }
                                // it means - now we're reading point number
                                else if (token.Type == BaseTokenType.rndBrktContent)
                                {
                                    // saving previous dialog-point's transitions
                                    if (state == ReadGameStates.ReadLinkTransition)
                                    {
                                        currDialogPoint.Links = linkList.ToArray();
                                    }

                                    bool parseResult = int.TryParse(token.Content, out int result);

                                    if (!parseResult)
                                    {
                                        throw new SyntaxError($"Next point number reading error, found: '{token.Content}'");
                                    }

                                    bool getResult = dialogPointDictionary.TryGetValue(result, out currDialogPoint);

                                    if (!getResult)
                                    {
                                        currDialogPoint = new DialogPoint()
                                        {
                                            ID = result
                                        };
                                        dialogPointDictionary.Add(currDialogPoint.ID, currDialogPoint);
                                    }

                                    state = ReadGameStates.ReadPointNumber;
                                }
                                else
                                    throw new SyntaxError($"Point number reading error: found '{token.Content}'");
                                break;
                            }
                        case ReadGameStates.ReadPointNumber:
                            {
                                if (token.Type == BaseTokenType.text)
                                {
                                    currDialogPoint.Text = token.Content;

                                    state = ReadGameStates.ReadPointText;
                                }
                                else
                                {
                                    throw new SyntaxError($"Expected text after point number, found: '{token.Content}'");
                                }
                                break;
                            }

                        case ReadGameStates.ReadPointActions:
                        case ReadGameStates.ReadPointText:
                            {
                                if (token.Type == BaseTokenType.sqrBrktContent)
                                {
                                    bool parseResult = int.TryParse(token.Content, out int result);

                                    if (!parseResult)
                                    {
                                        throw new SyntaxError($"Point number reading error: found '{token.Content}'");
                                    }

                                    bool getResult = dialogPointDictionary.TryGetValue(result, out DialogPoint nextPoint);

                                    if (!getResult)
                                    {
                                        nextPoint = new DialogPoint()
                                        {
                                            ID = result
                                        };
                                        dialogPointDictionary.Add(nextPoint.ID, nextPoint);
                                    }

                                    DialogLink link = new DialogLink()
                                    {
                                        Text = "next...",
                                        NextPoint = nextPoint,
                                        Number = 0,
                                    };

                                    currDialogPoint.Links = new DialogLink[1] { link };

                                    state = ReadGameStates.ReadNothing;
                                }
                                else if (token.Type == BaseTokenType.rndBrktContent && state == ReadGameStates.ReadPointText)
                                {
                                    GameAction[] actions = ParseActions(token.Content);

                                    currDialogPoint.Actions = actions;

                                    state = ReadGameStates.ReadPointActions;
                                }
                                else if (token.Type == BaseTokenType.minus)
                                {
                                    linkList.Clear();

                                    currLink = new DialogLink()
                                    {
                                        Number = linkList.Count,
                                    };
                                    linkList.Add(currLink);
                                    state = ReadGameStates.ReadLinkStart;
                                }
                                else
                                    throw new SyntaxError($"Excpected actions, transitions or link start '-', found: '{token.Content}'"); // expected actions or transition or link start
                                break;
                            }

                        case ReadGameStates.ReadCondition:
                        case ReadGameStates.ReadLinkStart:
                            {
                                if (token.Type == BaseTokenType.rndBrktContent && state == ReadGameStates.ReadLinkStart)
                                {
                                    LinkCondition[] conditions = ParseCondition(token.Content);

                                    currLink.Conditions = conditions;

                                    state = ReadGameStates.ReadCondition;
                                }
                                else if (token.Type == BaseTokenType.text)
                                {
                                    currLink.Text = token.Content;

                                    state = ReadGameStates.ReadLinkText;
                                }
                                break;
                            }
                        case ReadGameStates.ReadLinkAction:
                        case ReadGameStates.ReadLinkText:
                            {
                                if (token.Type == BaseTokenType.rndBrktContent && state == ReadGameStates.ReadLinkText)
                                {
                                    GameAction[] actions = ParseActions(token.Content);

                                    currLink.Actions = actions;

                                    state = ReadGameStates.ReadLinkAction;
                                }
                                else if (token.Type == BaseTokenType.sqrBrktContent)
                                {
                                    bool parseResult = int.TryParse(token.Content, out int result);

                                    if (!parseResult)
                                    {
                                        // expected number
                                    }

                                    bool getResult = dialogPointDictionary.TryGetValue(result, out DialogPoint nextPoint);

                                    if (!getResult)
                                    {
                                        nextPoint = new DialogPoint()
                                        {
                                            ID = result
                                        };
                                        dialogPointDictionary.Add(nextPoint.ID, nextPoint);
                                    }

                                    currLink.NextPoint = nextPoint;

                                    state = ReadGameStates.ReadLinkTransition;
                                }
                                else
                                {
                                    throw new SyntaxError($"Expected next point number, found: '{token.Content}'");
                                }

                                break;
                            }
                    }
                }

                if (state != ReadGameStates.ReadNothing && state != ReadGameStates.ReadLinkTransition)
                {
                    throw new UnexpectedEndOfInputError($"Incorrect input, the state, when stop: '{state.ToString()}'");
                }

                if (state == ReadGameStates.ReadLinkTransition)
                {
                    currDialogPoint.Links = linkList.ToArray();
                }
            }

            AnalizeGameGraph(dialogPointDictionary);

            dialogPointDictionary.TryGetValue(0, out DialogPoint root);

            return root;
        }

        public DialogPoint ReadGameFromText(string gameText)
        {
            return ReadGame(gameText.GetEnumerator());
        }
    }
}

