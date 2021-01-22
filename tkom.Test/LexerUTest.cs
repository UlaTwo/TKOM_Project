using System;
using Xunit;
using tkom.LexerN;
using System.IO;

namespace tkom.Test
{
    public class LexerUTests
    {
        private static void ActAndValidate_ListOfTokens(Lexer lekser, params TokenType[] expectedTokenTypes)
        {
            foreach (var tokenType in expectedTokenTypes)
            {
                lekser.nextToken();
                Assert.Equal(tokenType, lekser.Token.Type);
            }
        }


        [Fact]
        public void EmptySource_TokenIsEof()
        {
            string s = "";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.EOT, lekser.Token.Type);
            }
        }

        [Fact]
        public void OnlyWhiteSigns_TokenIsEof()
        {
            string s = "     \n\n\n";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.EOT, lekser.Token.Type);
            }
        }

        [Fact]
        public void SourceValidateString1()
        {
            string s = "def funkcja(int i)";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);

                ActAndValidate_ListOfTokens(lekser,
                (TokenType.Def),
                 (TokenType.Identifier),
                 (TokenType.BraceLeft),
                 (TokenType.IntegerId),
                 (TokenType.Identifier),
                  (TokenType.BraceRight));
            }
        }

        [Fact]
        public void SourceValidateString2()
        {
            string s = "while  \n (x!=23){funkcja(x);}";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);

                ActAndValidate_ListOfTokens(lekser,
                (TokenType.While),
                 (TokenType.BraceLeft),
                 (TokenType.Identifier),
                 (TokenType.InequalityOperator),
                 (TokenType.Integer),
                 (TokenType.BraceRight),
                 (TokenType.ParenthesesLeft),
                 (TokenType.Identifier),
                 (TokenType.BraceLeft),
                 (TokenType.Identifier),
                 (TokenType.BraceRight),
                 (TokenType.Semicolon),
                 (TokenType.ParenthesesRight)
                 );
            }
        }

        [Fact]
        public void SourceValidateString3()
        {
            string s = "if (x<23\n ^ red+funkcja()\n )";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);

                ActAndValidate_ListOfTokens(lekser,
                (TokenType.If),
                 (TokenType.BraceLeft),
                 (TokenType.Identifier),
                 (TokenType.LessOperator),
                 (TokenType.Integer),
                 (TokenType.AndOperator),
                 (TokenType.Identifier),
                 (TokenType.PlusOperator),
                 (TokenType.Identifier),
                 (TokenType.BraceLeft),
                 (TokenType.BraceRight),
                 (TokenType.BraceRight)
                 );
            }
        }

        [Fact]
        public void TokenIsInteger()
        {
            string s = "123";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Integer, lekser.Token.Type);
                Assert.Equal("123", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsIdentifier()
        {
            string s = "nazwa123";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Identifier, lekser.Token.Type);
                Assert.Equal("nazwa123", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsUndefined()
        {
            string s = "$";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Undefined, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsWhile()
        {
            string s = "while";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.While, lekser.Token.Type);
                Assert.Equal("while", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsIf()
        {
            string s = "if";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.If, lekser.Token.Type);
                Assert.Equal("if", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsTurtle()
        {
            string s = "turtle";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Turtle, lekser.Token.Type);
                Assert.Equal("turtle", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsIntegerId()
        {
            string s = "int";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.IntegerId, lekser.Token.Type);
                Assert.Equal("int", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsStringId()
        {
            string s = "string";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.StringId, lekser.Token.Type);
                Assert.Equal("string", lekser.Token.Value);
            }
        }


        [Fact]
        public void TokenIsDef()
        {
            string s = "def";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Def, lekser.Token.Type);
                Assert.Equal("def", lekser.Token.Value);
            }
        }

        [Fact]
        public void TokenIsComma()
        {
            string s = ",";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Comma, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsSemicolon()
        {
            string s = ";";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.Semicolon, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsParenthesesLeft()
        {
            string s = "{";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.ParenthesesLeft, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsParenthesesRight()
        {
            string s = "}";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.ParenthesesRight, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsBraceLeft()
        {
            string s = "(";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.BraceLeft, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsBraceRight()
        {
            string s = ")";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.BraceRight, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsOrOperator()
        {
            string s = "~";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.OrOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsAndOperator()
        {
            string s = "^";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.AndOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsNegationOperator()
        {
            string s = "!";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);

                lekser.nextToken();
                Assert.Equal(TokenType.NegationOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsAssignment()
        {
            string s = "=";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);

                lekser.nextToken();
                Assert.Equal(TokenType.Assignment, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsEqualityOperator()
        {
            string s = "==";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.EqualityOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsInequalityOperator()
        {
            string s = "!=";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.InequalityOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsGreaterOperator()
        {
            string s = ">";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.GreaterOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsGreaterOrEqualOperator()
        {
            string s = ">=";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.GreaterOrEqualOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsLessOperator()
        {
            string s = "<";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.LessOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsLessOrEqualOperator()
        {
            string s = "<=";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.LessOrEqualOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsPlusOperator()
        {
            string s = "+";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.PlusOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsMinusOperator()
        {
            string s = "-";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.MinusOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsAsteriskOperator()
        {
            string s = "*";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.AsteriskOperator, lekser.Token.Type);
            }
        }

        [Fact]
        public void TokenIsSlashOperator()
        {
            string s = "/";
            using (var stream = new StringReader(s))
            {
                Lexer lekser = new Lexer(stream);
                lekser.nextToken();
                Assert.Equal(TokenType.SlashOperator, lekser.Token.Type);
            }
        }
    }
}
