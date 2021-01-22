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
         Return, Def,
        Comma, Semicolon, ParenthesesLeft, ParenthesesRight, BraceLeft, BraceRight,
        OrOperator, AndOperator, NegationOperator,
        Assignment, EqualityOperator, InequalityOperator, GreaterOperator, GreaterOrEqualOperator, LessOperator, LessOrEqualOperator,
        PlusOperator, MinusOperator, AsteriskOperator, SlashOperator
    }
    public class Token
    {

        public TokenType Type { private set; get; }
        public string Value { private set; get; }

        public Position position { get; set; }

        public Token(TokenType type, Position pos)
        {
            Type = type;
            position = pos;

        }

        public Token(TokenType type, string value, Position pos)
        {
            Type = type;
            Value = value;
            position = pos;
        }

    }
}