using Irony.Parsing;

namespace GameTextParsing.GLan
{
    public static class Trm
    {
        public static string Minus = "-";
        public static string Sharp = "#";
        public static string Comma = ",";
        public static string Union = "and";
        public static string Colon = ":";
        public static string And = "and";
        public static string Or = "or";
        public static string Find = "find";
        public static string Lose = "lose";
        public static string Switch = "switch";
        public static string Not = "not";
        public static string If = "if";
        public static string Else = "else";
        public static string Then = "then";
        public static string Other = "other";
        public static string Percent = "%";
    }

    public static class NTrm
    {
        public static string Game = "Game";
        public static string DialogPoint = "DialogPoint";
        public static string SwitchPoint = "SwitchPoint";
        public static string GamePoint = "GamePoint";
        public static string Answer = "Answer";
        public static string AnswerBlock = "AnswerBlock";
        public static string AnswerUnion = "AnswerUnion";
        public static string DialogPointMark = "DialogPointMark";
        public static string NextPointMark = "NextPointMark";
        public static string TextBlock = "TextBlock";
        public static string KeyIdentifier = "KeyIdentifier";
        public static string PointIdentifier = "PointIdentifier";
        public static string GotoBlock = "GotoBlock";
        public static string ActionBlock = "ActionBlock";
        public static string KeyList = "KeyList";
        public static string Key = "Key";
        public static string ConditionBlock = "ConditionBlock";
        public static string Condition = "Condition";
        public static string ElseIfList = "ElseIfList";
        public static string ElseBlock = "ElseBlock";
        public static string IfBlock = "IfBlock";
        public static string ElseIfBlock = "ElseIfBlock";
        public static string AnswerPoint = "AnswerPoint";
        public static string CaseBlock = "CaseBlock";
        public static string Case = "Case";
        public static string Probability = "Probability";
        public static string OtherCase = "OtherCase";
        public static string CaseCondition = "CaseCondition";
    }

    public class Glan : Grammar
    {
        public Glan()
        {
            #region terminals

            Terminal Name = new IdentifierTerminal("Name");
            NumberLiteral Number = new NumberLiteral("PercentNumber");
            Terminal Text = new StringLiteral("Text", "\"");
            Terminal LongName = new StringLiteral("LongName", "\'");
            // numbers as '1', '2.15', '0.10.2.3' and etc
            Terminal DottedNumber = new RegexBasedTerminal("DottedNumber", "((\\d)+.)*(\\d)+");

            Terminal Minus = ToTerm(Trm.Minus);
            Terminal Sharp = ToTerm(Trm.Sharp);
            Terminal Percent = ToTerm(Trm.Percent);
            Terminal Comma = ToTerm(Trm.Comma);
            Terminal Union = ToTerm(Trm.And);
            Terminal Colon = ToTerm(Trm.Colon);
            Terminal And = ToTerm(Trm.And);
            Terminal Or = ToTerm(Trm.Or);
            Terminal Find = ToTerm(Trm.Find);
            Terminal Lose = ToTerm(Trm.Lose);
            Terminal Switch = ToTerm(Trm.Switch);
            Terminal Not = ToTerm(Trm.Not);
            Terminal If = ToTerm(Trm.If);
            Terminal Else = ToTerm(Trm.Else);
            Terminal Then = ToTerm(Trm.Then);
            Terminal Other = ToTerm(Trm.Other);

            #endregion

            #region non terminals

            NonTerminal Game = new NonTerminal(NTrm.Game);
            NonTerminal DialogPoint = new NonTerminal(NTrm.DialogPoint);
            NonTerminal SwitchPoint = new NonTerminal(NTrm.SwitchPoint);
            NonTerminal GamePoint = new NonTerminal(NTrm.GamePoint);
            NonTerminal Answer = new NonTerminal(NTrm.Answer);
            NonTerminal AnswerBlock = new NonTerminal(NTrm.AnswerBlock);
            NonTerminal AnswerUnion = new NonTerminal(NTrm.AnswerUnion);
            NonTerminal DialogPointMark = new NonTerminal(NTrm.DialogPointMark);
            NonTerminal NextPointMark = new NonTerminal(NTrm.NextPointMark);
            NonTerminal TextBlock = new NonTerminal(NTrm.TextBlock);
            NonTerminal KeyIdentifier = new NonTerminal(NTrm.KeyIdentifier);
            NonTerminal PointIdentifier = new NonTerminal(NTrm.PointIdentifier);
            NonTerminal GotoBlock = new NonTerminal(NTrm.GotoBlock);
            NonTerminal ActionBlock = new NonTerminal(NTrm.ActionBlock);
            NonTerminal KeyList = new NonTerminal(NTrm.KeyList);
            NonTerminal Key = new NonTerminal(NTrm.Key);
            NonTerminal ConditionBlock = new NonTerminal(NTrm.ConditionBlock);
            NonTerminal Condition = new NonTerminal(NTrm.Condition);
            NonTerminal ElseIfList = new NonTerminal(NTrm.ElseIfList);
            NonTerminal ElseBlock = new NonTerminal(NTrm.ElseBlock);
            NonTerminal IfBlock = new NonTerminal(NTrm.IfBlock);
            NonTerminal ElseIfBlock = new NonTerminal(NTrm.ElseIfBlock);
            NonTerminal AnswerPoint = new NonTerminal(NTrm.AnswerPoint);
            NonTerminal CaseBlock = new NonTerminal(NTrm.CaseBlock);
            NonTerminal Case = new NonTerminal(NTrm.Case);
            NonTerminal Probability = new NonTerminal(NTrm.Probability);
            NonTerminal OtherCase = new NonTerminal(NTrm.OtherCase);
            NonTerminal CaseCondition = new NonTerminal(NTrm.CaseCondition);

            #endregion

            #region rules

            Game.Rule = MakePlusRule(Game, GamePoint);

            GamePoint.Rule = DialogPoint | SwitchPoint;

            TextBlock.Rule = MakePlusRule(TextBlock, Comma, Text);
            KeyIdentifier.Rule = Name | LongName;
            PointIdentifier.Rule = Name | LongName | DottedNumber;
            DialogPointMark.Rule = "(" + PointIdentifier + ")";
            NextPointMark.Rule = "[" + PointIdentifier + "]";

            Key.Rule = Sharp + KeyIdentifier;
            KeyList.Rule = MakePlusRule(KeyList, Comma, Key);

            ActionBlock.Rule =
                Find + KeyList |
                Find + KeyList + Lose + KeyList |
                Lose + KeyList |
                Lose + KeyList + Find + KeyList;

            DialogPoint.Rule =
                DialogPointMark + TextBlock + GotoBlock |
                DialogPointMark + TextBlock + ActionBlock + GotoBlock;

            GotoBlock.Rule = NextPointMark | AnswerBlock;

            AnswerBlock.Rule = MakePlusRule(AnswerBlock, AnswerPoint);

            AnswerPoint.Rule = Answer | ConditionBlock;

            Answer.Rule =
                Minus + Text + NextPointMark |
                Minus + Text + ActionBlock + NextPointMark;

            AnswerUnion.Rule = MakePlusRule(AnswerUnion, Union, Answer);

            ConditionBlock.Rule =
                IfBlock |
                IfBlock + ElseBlock |
                IfBlock + ElseIfList |
                IfBlock + ElseIfList + ElseBlock;

            IfBlock.Rule = If + Condition + Then + AnswerUnion;

            ElseBlock.Rule = Else + AnswerUnion;

            ElseIfList.Rule = MakePlusRule(ElseIfList, ElseIfBlock);

            ElseIfBlock.Rule = Else + If + Condition + Then + AnswerUnion;

            Condition.Rule =
                Not + Condition |
                "(" + Condition + ")" |
                Condition + And + Condition |
                Condition + Or + Condition |
                Key;
            
            SwitchPoint.Rule =
                DialogPointMark + Switch + CaseBlock + OtherCase |
                DialogPointMark + Switch + OtherCase;

            CaseBlock.Rule = MakePlusRule(CaseBlock, Case);

            Case.Rule =
                CaseCondition + Colon + ActionBlock + NextPointMark |
                CaseCondition + Colon + NextPointMark;

            CaseCondition.Rule = Condition | Probability;

            Probability.Rule = Number + Percent;

            OtherCase.Rule =
                Other + Colon + ActionBlock + NextPointMark |
                Other + Colon + NextPointMark;

            #endregion

            this.Root = Game;

            RegisterOperators(1, Or);
            RegisterOperators(2, And);
            RegisterOperators(3, Associativity.Neutral, Not);

            MarkPunctuation("(", ")", "[", "]", "-", "#", ":");
            MarkPunctuation(Other, Switch, Else, If, Then, Minus);

            MarkTransient(GamePoint, AnswerPoint, GotoBlock, Key, CaseCondition);
        }
    }
}
