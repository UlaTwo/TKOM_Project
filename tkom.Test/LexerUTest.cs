using System;
using Xunit;
using tkom.LexerN;
using System.IO;

namespace tkom.Test{
    public class LexerUTests{

        private static Lexer PrepareLexer(string s)
        {
            var source = new StringSource(s);
            var lexer = new Lexer(source);
            return lexer;
        }

        private static void ActAndValidate_ListOfTokens( Lexer lekser, params TokenType[] expectedTokenTypes)
        {
            foreach (var tokenType in expectedTokenTypes)
            {
                lekser.nextToken();
                Assert.Equal(tokenType, lekser.Token.Type);
            }
        }


        [Fact]
        public void EmptySource_TokenIsEof(){
            var lekser = PrepareLexer("");
            lekser.nextToken();
            Assert.Equal(TokenType.EOT ,lekser.Token.Type );
        }

        [Fact]
        public void OnlyWhiteSigns_TokenIsEof(){
            var lekser = PrepareLexer("     \n\n\n");
            lekser.nextToken();
            Assert.Equal(TokenType.EOT ,lekser.Token.Type );
        }

        [Fact]
        public void SourceValidateString1(){
            var lekser = PrepareLexer("def funkcja(int i)");
            ActAndValidate_ListOfTokens(lekser, 
            (TokenType.Def),
             (TokenType.Identifier), 
             (TokenType.BraceLeft), 
             (TokenType.IntegerId), 
             (TokenType.Identifier),
              (TokenType.BraceRight));
        }

        [Fact]
        public void SourceValidateString2(){
            var lekser = PrepareLexer("while  \n (x!=23){funkcja(x);}");
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

         [Fact]
        public void SourceValidateString3(){
            var lekser = PrepareLexer("if (x<23\n ^ red+funkcja()\n )");
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

        [Fact]
        public void TokenIsInteger(){
            var lekser = PrepareLexer("123");
            lekser.nextToken();
            Assert.Equal(TokenType.Integer ,lekser.Token.Type );
            Assert.Equal("123" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsIdentifier(){
            var lekser = PrepareLexer("nazwa123");
            lekser.nextToken();
            Assert.Equal(TokenType.Identifier ,lekser.Token.Type );
            Assert.Equal("nazwa123" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsUndefined(){
            var lekser = PrepareLexer("$");
            lekser.nextToken();
            Assert.Equal(TokenType.Undefined ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsWhile(){
            var lekser = PrepareLexer("while");
            lekser.nextToken();
            Assert.Equal(TokenType.While ,lekser.Token.Type );
            Assert.Equal("while" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsIf(){
            var lekser = PrepareLexer("if");
            lekser.nextToken();
            Assert.Equal(TokenType.If ,lekser.Token.Type );
            Assert.Equal("if" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsTurtle(){
            var lekser = PrepareLexer("turtle");
            lekser.nextToken();
            Assert.Equal(TokenType.Turtle ,lekser.Token.Type );
            Assert.Equal("turtle" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsIntegerId(){
            var lekser = PrepareLexer("int");
            lekser.nextToken();
            Assert.Equal(TokenType.IntegerId ,lekser.Token.Type );
            Assert.Equal("int" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsStringId(){
            var lekser = PrepareLexer("string");
            lekser.nextToken();
            Assert.Equal(TokenType.StringId ,lekser.Token.Type );
            Assert.Equal("string" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsClass(){
            var lekser = PrepareLexer("class");
            lekser.nextToken();
            Assert.Equal(TokenType.Class ,lekser.Token.Type );
            Assert.Equal("class" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsReturn(){
            var lekser = PrepareLexer("return");
            lekser.nextToken();
            Assert.Equal(TokenType.Return ,lekser.Token.Type );
            Assert.Equal("return" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsDef(){
            var lekser = PrepareLexer("def");
            lekser.nextToken();
            Assert.Equal(TokenType.Def ,lekser.Token.Type );
            Assert.Equal("def" , lekser.Token.Value );
        }

        [Fact]
        public void TokenIsComma(){
            var lekser = PrepareLexer(",");
            lekser.nextToken();
            Assert.Equal(TokenType.Comma ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsSemicolon(){
            var lekser = PrepareLexer(";");
            lekser.nextToken();
            Assert.Equal(TokenType.Semicolon ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsParenthesesLeft(){
            var lekser = PrepareLexer("{");
            lekser.nextToken();
            Assert.Equal(TokenType.ParenthesesLeft ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsParenthesesRight(){
            var lekser = PrepareLexer("}");
            lekser.nextToken();
            Assert.Equal(TokenType.ParenthesesRight ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsBraceLeft(){
            var lekser = PrepareLexer("(");
            lekser.nextToken();
            Assert.Equal(TokenType.BraceLeft ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsBraceRight(){
            var lekser = PrepareLexer(")");
            lekser.nextToken();
            Assert.Equal(TokenType.BraceRight ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsOrOperator(){
            var lekser = PrepareLexer("~");
            lekser.nextToken();
            Assert.Equal(TokenType.OrOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsAndOperator(){
            var lekser = PrepareLexer("^");
            lekser.nextToken();
            Assert.Equal(TokenType.AndOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsNegationOperator(){
            var lekser = PrepareLexer("!");
            lekser.nextToken();
            Assert.Equal(TokenType.NegationOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsAssignment(){
            var lekser = PrepareLexer("=");
            lekser.nextToken();
            Assert.Equal(TokenType.Assignment ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsEqualityOperator(){
            var lekser = PrepareLexer("==");
            lekser.nextToken();
            Assert.Equal(TokenType.EqualityOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsInequalityOperator(){
            var lekser = PrepareLexer("!=");
            lekser.nextToken();
            Assert.Equal(TokenType.InequalityOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsGreaterOperator(){
            var lekser = PrepareLexer(">");
            lekser.nextToken();
            Assert.Equal(TokenType.GreaterOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsGreaterOrEqualOperator(){
            var lekser = PrepareLexer(">=");
            lekser.nextToken();
            Assert.Equal(TokenType.GreaterOrEqualOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsLessOperator(){
            var lekser = PrepareLexer("<");
            lekser.nextToken();
            Assert.Equal(TokenType.LessOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsLessOrEqualOperator(){
            var lekser = PrepareLexer("<=");
            lekser.nextToken();
            Assert.Equal(TokenType.LessOrEqualOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsPlusOperator(){
            var lekser = PrepareLexer("+");
            lekser.nextToken();
            Assert.Equal(TokenType.PlusOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsMinusOperator(){
            var lekser = PrepareLexer("-");
            lekser.nextToken();
            Assert.Equal(TokenType.MinusOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsAsteriskOperator(){
            var lekser = PrepareLexer("*");
            lekser.nextToken();
            Assert.Equal(TokenType.AsteriskOperator ,lekser.Token.Type );
        }

        [Fact]
        public void TokenIsSlashOperator(){
            var lekser = PrepareLexer("/");
            lekser.nextToken();
            Assert.Equal(TokenType.SlashOperator ,lekser.Token.Type );
        }
    }
}