using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace tkom.LexerN
{
    public class Lexer
    {
        public ISource source { get; private set; }
        public Lexer(ISource source)
        {
            this.source = source;
            KeywordsMap = new Dictionary<string, TokenType>();

            KeywordsMap["while"] = TokenType.While;
            KeywordsMap["if"] = TokenType.If;

            KeywordsMap["turtle"] = TokenType.Turtle;
            KeywordsMap["int"] = TokenType.IntegerId;
            KeywordsMap["string"] = TokenType.StringId;

            KeywordsMap["class"] = TokenType.Class;
            KeywordsMap["return"] = TokenType.Return;
            KeywordsMap["def"] = TokenType.Def;
        }

        private Dictionary<string, TokenType> KeywordsMap;
        private const int MAX_LENGTH = 50;
        private const int MAX_NUMBER_LENGTH = 9;
        public Token Token { get; private set; }

        public void nextToken()
        {
            //pominięcie białych znków
            while (Char.IsWhiteSpace(source.character) && source.isEnd == false) { source.Read(); }

            if (source.isEnd)
            {
                Token = new Token(TokenType.EOT, source.Line, source.Column);
                return;
            }

            if (tryToBuildIdentifierOrKeyword())
                return;
            if (tryToBuildNumber())
                return;
            if (tryToBuildSingleOrDoubleCharToken())
                return;

            Token = new Token(TokenType.Undefined, source.Line, source.Column);
            source.Read();
            return;
        }

        private bool tryToBuildIdentifierOrKeyword()
        {

            if (Char.IsLetter(source.character))
            {
                var buf = new StringBuilder();
                while (char.IsLetterOrDigit(source.character))
                {
                    buf.Append((char)source.character);
                    source.Read();
                    if (buf.ToString().Length > MAX_LENGTH)
                        throw new LexicalException("Exception: Too long identifier, in line: " + source.Line);
                }
                if (KeywordsMap.ContainsKey(buf.ToString()))
                    Token = new Token(KeywordsMap[buf.ToString()], buf.ToString(), source.Line, source.Column - buf.ToString().Length);
                else
                    Token = new Token(TokenType.Identifier, buf.ToString(), source.Line, source.Column - buf.ToString().Length);
                return true;
            }
            else return false;
        }
        private bool tryToBuildNumber()
        {

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
                        Token = new Token(TokenType.Integer, "0", source.Line, source.Column);
                        return true;
                    }
                    else
                        throw new LexicalException("Exception: Integer can't start from 0, in line: " + source.Line + " column: " + source.Column);
                }
                while (char.IsDigit(source.character))
                {
                    buf.Append((char)source.character);
                    source.Read();
                    if (buf.ToString().Length > MAX_NUMBER_LENGTH)
                        throw new LexicalException("Exception: Too long integer, in line: " + source.Line);
                }
                Token = new Token(TokenType.Integer, buf.ToString(), source.Line, source.Column - buf.ToString().Length);
                return true;

            }
            else return false;
        }

        private bool tryToBuildSingleOrDoubleCharToken()
        {
            int line = source.Line;
            int col = source.Column;
            switch (source.character)
            {
                case '=':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.EqualityOperator, "==", line, col); source.Read(); }
                    else Token = new Token(TokenType.Assignment, "=", line, col);
                    return true;
                case '>':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.GreaterOrEqualOperator, ">=", line, col); source.Read(); }
                    else Token = new Token(TokenType.GreaterOperator, ">", line, col);
                    return true;
                case '<':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.LessOrEqualOperator, "<=", line, col); source.Read(); }
                    else Token = new Token(TokenType.LessOperator, "<", line, col);
                    return true;
                case '!':
                    source.Read();
                    if (source.character == '=') { Token = new Token(TokenType.InequalityOperator, "!=", line, col); source.Read(); }
                    else Token = new Token(TokenType.NegationOperator, "!", line, col);
                    return true;
                case '{': source.Read(); Token = new Token(TokenType.ParenthesesLeft, "{", line, col); return true;
                case '}': source.Read(); Token = new Token(TokenType.ParenthesesRight, "}",line, col); return true;
                case '(': source.Read(); Token = new Token(TokenType.BraceLeft, "(", line, col); return true;
                case ')': source.Read(); Token = new Token(TokenType.BraceRight, ")", line, col); return true;
                case '+': source.Read(); Token = new Token(TokenType.PlusOperator, "+",line, col); return true;
                case '-': source.Read(); Token = new Token(TokenType.MinusOperator, "-", line, col); return true;
                case '*': source.Read(); Token = new Token(TokenType.AsteriskOperator, "*", line, col); return true;
                case '/': source.Read(); Token = new Token(TokenType.SlashOperator, "/", line, col); return true;
                case ',': source.Read(); Token = new Token(TokenType.Comma, ",", line, col); return true;
                case ';': source.Read(); Token = new Token(TokenType.Semicolon, ";", line, col); return true;
                case '^': source.Read(); Token = new Token(TokenType.AndOperator, "^", line, col); return true;
                case '~': source.Read(); Token = new Token(TokenType.OrOperator, "~", line, col); return true;
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