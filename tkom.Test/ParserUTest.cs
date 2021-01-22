using System;
using Xunit;
using tkom.ParserN;
using tkom.ParserN.Structures;
using tkom.LexerN;
using System.IO;
using System.Linq;
namespace tkom.Test
{
    public class ParserUTest
    {
        ProgramStructure returnProgram(StringReader stream)
        {
            Lexer lekser = new Lexer(stream);
            Parser parser = new Parser(lekser);
            ProgramStructure program = new ProgramStructure();
            return parser.ParseProgram();
        }

        [Fact]
        public void EmptyStream_Parse_ReturnsEmptyProgram()
        {
            string s = "";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Empty(program.Functions);
            }
        }

        [Fact]
        public void EmptyFunction_Parse()
        {
            string s = "def int fun (){ }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Single(program.Functions);

                Assert_EmptyFunctionDeclaration("fun", program.Functions.First(), true);
            }
        }

        [Fact]
        public void EmptyTwoFunctions_Parse()
        {
            string s = "def int fun1 ()\n{\n }\ndef fun2 ()\n{\n }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Equal(2, program.Functions.Count);

                Assert_EmptyFunctionDeclaration("fun1", program.Functions.First(), true);
                Assert_EmptyFunctionDeclaration("fun2", program.Functions.Skip(1).First(), false);
            }
        }

        [Fact]
        public void EmptyFunc_Parse()
        {
            string s = "\n\ndef int fun1 ()\n{\n }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Single(program.Functions);

                Assert_EmptyFunctionDeclaration("fun1", program.Functions.First(), true);
            }
        }


        [Fact]
        public void SimpleFunctionWithVarDeclaration_Parse()
        {
            string s = "def main (string gif){\nint a;\n}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.NotEmpty(program.Functions);
                Assert.Equal("main", program.Functions.First().Identifier);
                //arguments
                Assert.NotEmpty(program.Functions.First().Arguments);
                Assert.Equal("gif", program.Functions.First().Arguments.First().Identifier);
                //instructions
                //first instruction
                Assert.NotEmpty(program.Functions.First().Instructions);
                Assert.IsType<IntVarDeclaration>(program.Functions.First().Instructions.First());
                IntVarDeclaration intVar = (IntVarDeclaration)program.Functions.First().Instructions.First();
                Assert.Equal("a", intVar.Identifier);
            }
        }


        [Fact]
        public void FunctionWithMoreArgumensAndIfStatement_Parse()
        {
            string s = "def funkcja (int i1, turtle t1){\nif( !(i1<3) ^ (i1==10) )\n{b=-6+fun()*a;};\n}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.NotEmpty(program.Functions);
                Assert.Equal("funkcja", program.Functions.First().Identifier);
                Assert.False(program.Functions.First().isIntReturned);
                //arguments
                //first argument
                Assert.NotEmpty(program.Functions.First().Arguments);
                Assert.Equal("i1", program.Functions.First().Arguments.First().Identifier);
                Assert.IsType<IntVarDeclaration>(program.Functions.First().Arguments.First());
                //second argument
                Assert.Equal("t1", program.Functions.First().Arguments.Skip(1).First().Identifier);
                Assert.IsType<TurtleVarDeclaration>(program.Functions.First().Arguments.Skip(1).First());
                //instructions
                //first instruction - STATEMENT
                Assert.NotEmpty(program.Functions.First().Instructions);
                Assert.IsType<Statement>(program.Functions.First().Instructions.First());
                Statement st = (Statement)program.Functions.First().Instructions.First();
                Assert.Equal("if", st.Type);

                //STATEMENT CONDITION
                // Condition cond = (Condition)st.Condition;
                IConditionQueueType[] condArray = st.Condition.ConditionONPQueue.ToArray();
                Assert.NotEmpty(condArray);
                Assert.Equal(3, condArray.Length);
                //w kolejce: [<Condition> : !(i1<3)][<Condition> : (i1==10) ][<Value> ^]
                Assert.IsType<ConditionSingle>(condArray[0]);
                ConditionSingle condS1 = (ConditionSingle)condArray[0];
                Assert.IsType<ConditionSingle>(condArray[1]);
                ConditionSingle condS2 = (ConditionSingle)condArray[1];
                Assert_CheckSimpleCondition(condS1, true, "i1", "<", 3);
                Assert_CheckSimpleCondition(condS2, false, "i1", "==", 10);
                Assert.IsType<Value>(condArray[2]);
                Assert_Value((Value)condArray[2], "^");

                // //STATEMENT INSTRUCTION
                Assert.Single(st.Instructions);
                Assert.IsType<InitValue>(st.Instructions[0]);
                InitValue instruction1 = (InitValue)st.Instructions[0];
                Assert.Equal("b", instruction1.Identifier);
                Assert.NotEmpty(instruction1.InitedValue.ExpressionONPQueue);
                IExpressionQueueType[] expressionA = instruction1.InitedValue.ExpressionONPQueue.ToArray();
                Assert.Equal(5, expressionA.Length);
                //expressionA : [6][fun()][a][*][+]
                Assert.IsType<ExpressionSingle>(expressionA[0]);
                Assert_ExpressionSingleIntegerValue((ExpressionSingle)expressionA[0], true, 6);
                Assert_ExpressionSingleFunctionCallWithoutArgument((ExpressionSingle)expressionA[1], false, "fun");
                Assert_ExpressionSingleValue((ExpressionSingle)expressionA[2], false, "a");
                Assert_Value((Value)expressionA[3], "*");
                Assert_Value((Value)expressionA[4], "+");

            }
        }

        [Fact]
        public void FunctionDeclarationWithObjectCall_Parse()
        {
            string s = "def int fun(){\nturtle t1;\nt1{\ncircle(4,red);\nsave();\n};\n}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Assert.NotEmpty(program.Functions);
                Assert.Equal("fun", program.Functions.First().Identifier);
                Assert.True(program.Functions.First().isIntReturned);

                //arguments
                Assert.Empty(program.Functions.First().Arguments);

                //instructions
                //first instruction - TurtleVarDeclaration
                Assert.NotEmpty(program.Functions.First().Instructions);
                Assert.IsType<TurtleVarDeclaration>(program.Functions.First().Instructions.First());
                TurtleVarDeclaration turVar = (TurtleVarDeclaration)program.Functions.First().Instructions.First();
                Assert.Equal("t1", turVar.Identifier);
                //second instruction - ObjectCall
                Assert.IsType<ObjectCall>(program.Functions.First().Instructions.Skip(1).First());
                ObjectCall objCall = (ObjectCall)program.Functions.First().Instructions.Skip(1).First();
                Assert.Equal("t1", objCall.ObjectName);
                Assert.NotEmpty(objCall.Functions);
                //first function in Object call - circle()
                Assert.Equal("circle", objCall.Functions[0].FunctionName);
                Assert.Equal(2, objCall.Functions[0].ArgumentValues.Count);
                Assert.IsType<Expression>(objCall.Functions[0].ArgumentValues[0]);
                Assert_ExpressionIsSingleIntegerValue(4, (Expression)objCall.Functions[0].ArgumentValues[0]);
                Assert.IsType<Expression>(objCall.Functions[0].ArgumentValues[1]);
                Assert_ExpressionIsSingleValue("red", (Expression)objCall.Functions[0].ArgumentValues[1]);

                //second function in Object call - save()
                Assert_FunctionCallWithoutArgument(objCall.Functions[1], "save");

            }
        }

        [Fact]
        public void ExceptionErrorWithProgramConstruction(){
            string s = "def int fun(){\nturtle t1;\nt1{\ncircle(4,red);\nsave();\n};\n} abcd";
            using (var stream = new StringReader(s))
            {
                bool exceptionThrown = false;
                try{
                 ProgramStructure program = returnProgram(stream);
                }
                catch(ParserException e){
                    exceptionThrown = true;
                    Assert.Equal("Exception:  Error with construction of the program. Excepted EOT after all function declarations. in line: 7 in column: 3",e.Message);
                }
                Assert.True(exceptionThrown);
            }
        }

        [Fact]
        public void ExceptionErrorMissingFunctionName_ParseFunction(){
            string s = "def (){\n}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: ParseFunction(). Error - missing identifier (function name). in line: 1 in column: 5" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingBraceLeft_ParseFunction(){
            string s = "def go){\n}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: ParseFunction(). Error - missing brace left. in line: 1 in column: 7" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingBraceRight_ParseFunction(){
            string s = "def go({\n}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: ParseFunction(). Error - missing brace right.  in line: 1 in column: 8" );
            }
        }


        [Fact]
        public void ExceptionErrorMissingIdentifierName_ParseArgument(){
            string s = "def go(){ int 456; }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseArgument(). Error - missing identifier name.  in line: 1 in column: 15" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingSemicolon_ParseInstructionBlock(){
            string s = "def go(){ int a }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseInstructionBlock(). Error - missing Semicolon.  in line: 1 in column: 17" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingParenthesesLeft_ParseInstructionBlock(){
            string s = "def go() int a; }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseInstructionBlock(). Error - missing Parentheses Left.  in line: 1 in column: 10" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingParenthesesRight_ParseInstructionBlock(){
            string s = "def go(){ int a; ";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseInstructionBlock(). Error - missing Parentheses Right.  in line: 1 in column: 17" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingBraceLeft_ParseStatement(){
            string s = "def go(){ if){};}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseStatement(). Error - missing Brace Left. in line: 1 in column: 13" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingCondition_ParseStatement(){
            string s = "def go(){ if(){};}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseStatement(). Error - missing condition in statement. in line: 1 in column: 14" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingBraceRight_ParseStatement(){
            string s = "def go(){ if({};}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseStatement(). Error - missing condition in statement. in line: 1 in column: 14" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingSemicolon_ParseStatement(){
            string s = "def go(){ if(a==3){}}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseStatement(). Error - missing Semicolon after function declaration. in line: 1 in column: 21" );
            }
        }

        [Fact]
        public void ExceptionErrorIncorrectCondition_ParseCondition(){
            string s = "def go(){ if(a==){}}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseCondition(). The condition is not correct.  in line: 1 in column: 17" );
            }
        }

        [Fact]
        public void ExceptionErrorSingleNegation_ParseConditionSingle(){
            string s = "def go(){ if(!){}}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseConditionSingle(). Error -single negation with no expression.  in line: 1 in column: 15" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingConditionAfterBraceLeft_ParseConditionSingle(){
            string s = "def go(){ if(!(){}}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseConditionSingle(). Error - missing condition after BraceLeft. in line: 1 in column: 16" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingExpression_InitValue(){
            string s = "def go(){ a=;}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseInitValue(string id). Error - missing expression in line:  in line: 1 in column: 13" );
            }
        }
        [Fact]
        public void ExceptionErrorIncorrectExpression_ParseExpression(){
            string s = "def go(){ a=1+;}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseExpression(). Error - the expression is not correct.  in line: 1 in column: 15" );
            }
        }
        [Fact]
        public void ExceptionErrorSingleMinus_ParseExpression(){
            string s = "def go(){ a=-;}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseExpressionSingle(). Error - missing expression (single minus). in line: 1 in column: 14" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingBraceRight_ParseExpression(){
            string s = "def go(){ a=(a+b;}";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseExpressionSingle(). Error - missing BraceRight. in line: 1 in column: 17" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingParenthasesRight_ParseObjectCall(){
            string s = "def go(){ a{; }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseObjectCall(string id). Error - missing Parentheses Right.  in line: 1 in column: 13" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingSemicolon_ParseObjectCall(){
            string s = "def go(){ a{ b() }; }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseObjectCall(string id). Error - missing Semicolon. in line: 1 in column: 18" );
            }
        }

        [Fact]
        public void ExceptionErrorMissingArgument_ParseFunctionCall(){
            string s = "def go(){ a(i,); }";
            using (var stream = new StringReader(s))
            {
                Assert_Exception( stream, "Exception: tryToParseArgumentsFunctionCall. Error - missing expression after comma. in line: 1 in column: 15" );
            }
        }


        private void Assert_Exception(StringReader stream, string expectedMessage ){
            bool exceptionThrown = false;
                try{
                 ProgramStructure program = returnProgram(stream);
                }
                catch(ParserException e){
                    exceptionThrown = true;
                    Assert.Equal(expectedMessage,e.Message);
                }
                Assert.True(exceptionThrown);
        }

        private void Assert_EmptyFunctionDeclaration(string name, FunctionDeclaration funDec, bool isInt)
        {
            Assert.Equal(name, funDec.Identifier);
            if (isInt)
                Assert.True(funDec.isIntReturned);
            else
                Assert.False(funDec.isIntReturned);
            Assert.Empty(funDec.Arguments);
            Assert.Empty(funDec.Instructions);
        }
        private void Assert_Value(Value val, string value)
        {
            Assert.Equal(value, val.Identifier);
        }
        private void Assert_ExpressionSingleValue(ExpressionSingle exp, bool isMinus, string value)
        {

            if (isMinus)
                Assert.True(exp.isMinus);
            else Assert.False(exp.isMinus);
            Assert.IsType<Value>(exp.expressionType);
            Value Val = (Value)exp.expressionType;
            Assert.Equal(value, Val.Identifier);
        }

        private void Assert_ExpressionSingleFunctionCallWithoutArgument(ExpressionSingle exp, bool isMinus, string funName)
        {
            if (isMinus)
                Assert.True(exp.isMinus);
            else Assert.False(exp.isMinus);
            Assert.IsType<FunctionCall>(exp.expressionType);
            FunctionCall funCal = (FunctionCall)exp.expressionType;
            Assert_FunctionCallWithoutArgument(funCal, funName);
        }

        private void Assert_FunctionCallWithoutArgument(FunctionCall funCal, string funName)
        {
            Assert.Equal(funName, funCal.FunctionName);
        }

        private void Assert_ExpressionSingleIntegerValue(ExpressionSingle exp, bool isMinus, int value)
        {
            if (isMinus)
                Assert.True(exp.isMinus);
            else Assert.False(exp.isMinus);
            Assert.IsType<IntegerValue>(exp.expressionType);
            IntegerValue intVal = (IntegerValue)exp.expressionType;
            Assert.Equal(value, intVal.value);
        }

        private void Assert_ExpressionIsSingleValue(string value, Expression exp)
        {
            IExpressionQueueType[] ExpA = exp.ExpressionONPQueue.ToArray();
            // w cond1condS1ExpA: [<Value> : value]
            Assert.Single(ExpA);
            Assert.IsType<ExpressionSingle>(ExpA[0]);
            ExpressionSingle exp1 = (ExpressionSingle)ExpA[0];
            Assert.False(exp1.isMinus);
            Assert.IsType<Value>(exp1.expressionType);
            Value val = (Value)exp1.expressionType;
            Assert.Equal(value, val.Identifier);
        }

        private void Assert_ExpressionIsSingleIntegerValue(int value, Expression exp)
        {
            IExpressionQueueType[] ExpA = exp.ExpressionONPQueue.ToArray();
            Assert.Single(ExpA);
            Assert.IsType<ExpressionSingle>(ExpA[0]);
            ExpressionSingle exp1 = (ExpressionSingle)ExpA[0];
            Assert.False(exp1.isMinus);
            Assert.IsType<IntegerValue>(exp1.expressionType);
            IntegerValue val = (IntegerValue)exp1.expressionType;
            Assert.Equal(value, val.value);
        }

        //sprawdza wyrażenie warunkowe w postaci: [!](value sign int)
        private void Assert_CheckSimpleCondition(ConditionSingle condS1, bool isNegation, string value, string sign, int integer)
        {
            //isNegation
            if (isNegation)
                Assert.True(condS1.isNegation);
            else Assert.False(condS1.isNegation);

            Assert.IsType<Condition>(condS1.expression);
            Condition cond1 = (Condition)condS1.expression;
            IConditionQueueType[] cond1Array = cond1.ConditionONPQueue.ToArray();
            Assert.Equal(3, cond1Array.Length);
            //w cond1Array: [<Expression> : value][<Expression> : integer][<Value> : sign]
            //value
            Assert.IsType<ConditionSingle>(cond1Array[0]);
            ConditionSingle cond1condS1 = (ConditionSingle)cond1Array[0];
            Assert.IsType<Expression>(cond1condS1.expression);
            Expression exp1 = (Expression)cond1condS1.expression;
            Assert_ExpressionIsSingleValue(value, exp1);

            //integer
            Assert.IsType<ConditionSingle>(cond1Array[1]);
            ConditionSingle cond1condS2 = (ConditionSingle)cond1Array[1];
            Assert.IsType<Expression>(cond1condS2.expression);
            Expression exp2 = (Expression)cond1condS2.expression;
            Assert_ExpressionIsSingleIntegerValue(integer, exp2);

            //sign
            Assert.IsType<Value>(cond1Array[2]);
            Value val3 = (Value)cond1Array[2];
            Assert.Equal(sign, val3.Identifier);

        }
    }
}