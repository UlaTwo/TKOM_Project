using System.Collections.Generic;
using System;

namespace tkom.LexerN
{
        public enum TokenType
        {
            EOT,
            Identifier,
            Undefined,
            While, If,
            Turtle, IntegerId, Integer, StringId,
            Class, Return, Def,
            Comma, Semicolon, ParenthesesLeft, ParenthesesRight, BraceLeft, BraceRight,
            OrOperator, AndOperator, NegationOperator,
            Assignment, EqualityOperator, InequalityOperator, GreaterOperator, GreaterOrEqualOperator, LessOperator, LessOrEqualOperator,
            PlusOperator, MinusOperator, AsteriskOperator, SlashOperator

            //Red, Blue, Yellow, Black, White
        }
    public class Token
    {

        public TokenType Type { private set; get; }
        public string Value { private set; get; }
        public int Line { private set; get; }
        public int Column { private set; get; }

        public Token(TokenType type, int line, int column)
        {
            Type = type;
            Column = column;
            Line = line;
        }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Column = column;
            Line = line;
        }

    }
}