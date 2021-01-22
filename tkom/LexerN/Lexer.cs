using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace tkom.LexerN
{
    public class Lexer
    {
        public Source source { get; private set; }
        public Lexer(TextReader reader)
        {
            this.source = new Source(reader);
            this.KeywordsMapInitialization();
        }
        private void KeywordsMapInitialization()
        {
            KeywordsMap = new Dictionary<string, TokenType>();

            KeywordsMap["while"] = TokenType.While;
            KeywordsMap["if"] = TokenType.If;

            KeywordsMap["turtle"] = TokenType.Turtle;
            KeywordsMap["int"] = TokenType.IntegerId;
            KeywordsMap["string"] = TokenType.StringId;

            KeywordsMap["def"] = TokenType.Def;
        }
        private Dictionary<string, TokenType> KeywordsMap;
        private const int MAX_LENGTH = 50;
        private const int MAX_NUMBER_LENGTH = 9;
        public Token Token { get; private set; }

        public void nextToken()
        {
            //pominięcie białych znków
            while (Char.IsWhiteSpace(source.character) ) { source.Read(); }

            Position pos = source.position;

            if (source.character == '\0')
            {
                Token = new Token(TokenType.EOT, pos);
                return;
            }

            if (tryToBuildIdentifierOrKeyword())
                return;
            if (tryToBuildNumber())
                return;
            if (tryToBuildSingleOrDoubleCharToken())
                return;

            Token = new Token(TokenType.Undefined, pos);
            source.Read();
            return;
        }

        private bool tryToBuildIdentifierOrKeyword()
        {
            Position pos = new Position(source.position.Line, source.position.Column);
            if (Char.IsLetter(source.character))
            {
                var buf = new StringBuilder();
                while (char.IsLetterOrDigit(source.character))
                {
                    buf.Append((char)source.character);
                    source.Read();
                    if (buf.ToString().Length > MAX_LENGTH)
                        throw new LexicalException("Exception: Too long identifier, in line: " + source.position.Line);
                }
                if (KeywordsMap.ContainsKey(buf.ToString()))
                    Token = new Token(KeywordsMap[buf.ToString()], buf.ToString(), pos);
                else
                    Token = new Token(TokenType.Identifier, buf.ToString(), pos);
                return true;
            }
            else return false;
        }
        private bool tryToBuildNumber()
        {
            Position pos = new Position(source.position.Line, source.position.Column);
            if (Char.IsDigit(source.character))
            {
                var buf = new StringBuilder();
                //sprawdzenie, czy zaczynając się od 0 nie ma za sobą kolejnych liczb
                //błędne np. 0325
                //sprawdzenie, czy liczba nie jest za duża
                //Wprowadzone ograniczenie:  maksymalna długość int'a to 9?
                if (source.character == '0')
                {
                    source.Read();
                    if (!char.IsDigit(source.character))
                    {
                        Token = new Token(TokenType.Integer, "0", pos);
                        return true;
                    }
                    else
                        throw new LexicalException("Exception: Integer can't start from 0, in line: " + source.position.Line + " column: " + source.position.Column);
                }
                while (char.IsDigit(source.character))
                {
                    buf.Append((char)source.character);
                    source.Read();
                    if (buf.ToString().Length > MAX_NUMBER_LENGTH)
                        throw new LexicalException("Exception: Too long integer, in line: " + source.position.Line);
                }
                Token = new Token(TokenType.Integer, buf.ToString(), pos);
                return true;

            }
            else return false;
        }

        private bool tryToBuildSingleOrDoubleCharToken()
        {
            Position pos = new Position(source.position.Line, source.position.Column);
            switch (source.character)
            {
                case '=':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.EqualityOperator, "==", pos); source.Read(); }
                    else Token = new Token(TokenType.Assignment, "=", pos);
                    return true;
                case '>':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.GreaterOrEqualOperator, ">=", pos); source.Read(); }
                    else Token = new Token(TokenType.GreaterOperator, ">", pos);
                    return true;
                case '<':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.LessOrEqualOperator, "<=", pos); source.Read(); }
                    else Token = new Token(TokenType.LessOperator, "<", pos);
                    return true;
                case '!':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.InequalityOperator, "!=", pos); source.Read(); }
                    else Token = new Token(TokenType.NegationOperator, "!", pos);
                    return true;
                case '{': source.Read(); Token = new Token(TokenType.ParenthesesLeft, "{", pos); return true;
                case '}': source.Read(); Token = new Token(TokenType.ParenthesesRight, "}", pos); return true;
                case '(': source.Read(); Token = new Token(TokenType.BraceLeft, "(", pos); return true;
                case ')': source.Read(); Token = new Token(TokenType.BraceRight, ")", pos); return true;
                case '+': source.Read(); Token = new Token(TokenType.PlusOperator, "+", pos); return true;
                case '-': source.Read(); Token = new Token(TokenType.MinusOperator, "-", pos); return true;
                case '*': source.Read(); Token = new Token(TokenType.AsteriskOperator, "*", pos); return true;
                case '/': source.Read(); Token = new Token(TokenType.SlashOperator, "/", pos); return true;
                case ',': source.Read(); Token = new Token(TokenType.Comma, ",", pos); return true;
                case ';': source.Read(); Token = new Token(TokenType.Semicolon, ";", pos); return true;
                case '^': source.Read(); Token = new Token(TokenType.AndOperator, "^", pos); return true;
                case '~': source.Read(); Token = new Token(TokenType.OrOperator, "~", pos); return true;
                default: return false;
            }
        }
    }

    class LexicalException : Exception
    {
        public LexicalException() { }
        public LexicalException(string message) : base(message) { }
        public LexicalException(string message, Exception inner) : base(message, inner) { }
    }
}