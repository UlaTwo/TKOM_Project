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
                Assert.Empty(program.Classes);
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
                Assert.Empty(program.Classes);

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
                Assert.Empty(program.Classes);

                Assert_EmptyFunctionDeclaration("fun1", program.Functions.First(), true);
                Assert_EmptyFunctionDeclaration("fun2", program.Functions.Skip(1).First(), false);
            }
        }

        [Fact]
        public void EmptyClass_Parse()
        {
            string s = "class klasa1\n{\n }\n";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Empty(program.Functions);
                Assert.Single(program.Classes);

                Assert_EmptyClassDeclaration("klasa1", program.Classes[0]);
            }
        }

        [Fact]
        public void EmptyFunctionAndEmptyClass_Parse()
        {
            string s = "\n class klasa1\n{\n }\ndef int fun1 ()\n{\n }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.Single(program.Functions);
                Assert.Single(program.Classes);

                Assert_EmptyFunctionDeclaration("fun1", program.Functions.First(), true);
                Assert_EmptyClassDeclaration("klasa1", program.Classes[0]);
            }
        }


        [Fact]
        public void SimpleFunctionWithVarDeclaration_Parse()
        {
            string s = "def main (string gif){\nint a;\nclass nazwaKlasy b;\n}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Assert.NotEmpty(program.Functions);
                Assert.Empty(program.Classes);
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
                //second instruction
                Assert.IsType<ClassVarDeclaration>(program.Functions.First().Instructions.Skip(1).First());
                ClassVarDeclaration classVar = (ClassVarDeclaration)program.Functions.First().Instructions.Skip(1).First();
                Assert.Equal("b", classVar.Identifier);
                Assert.Equal("nazwaKlasy", classVar.ClassName);
            }
        }

        [Fact]
        public void SimpleClass_Parse()
        {
            string s = "class nazwaKlasy {\nint a;\ndef fun(){a=6;};\n}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Assert.Empty(program.Functions);
                Assert.NotEmpty(program.Classes);
                // //class
                Assert.Equal("nazwaKlasy", program.Classes[0].Name);
                Assert.IsType<IntVarDeclaration>(program.Classes[0].ClassInstructions[0]);
                Assert.IsType<FunctionDeclaration>(program.Classes[0].ClassInstructions[1]);

                IntVarDeclaration intVar = (IntVarDeclaration)program.Classes[0].ClassInstructions[0];
                Assert.Equal("a", intVar.Identifier);

                FunctionDeclaration funDec = (FunctionDeclaration)program.Classes[0].ClassInstructions[1];
                Assert.Equal("fun", funDec.Identifier);
                Assert.False(funDec.isIntReturned);
                Assert.Empty(funDec.Arguments);
                Assert.NotEmpty(funDec.Instructions);

                Assert.IsType<InitValue>(funDec.Instructions[0]);
                InitValue initVal = (InitValue)funDec.Instructions[0];
                Assert.Equal("a", initVal.Identifier);

                Assert.NotEmpty(initVal.InitedValue.ExpressionONPQueue);
                IExpressionQueueType[] expressionA = initVal.InitedValue.ExpressionONPQueue.ToArray();
                Assert.Single(expressionA);
                //expressionA : [6]
                Assert.IsType<ExpressionSingle>(expressionA[0]);
                Assert_ExpressionSingleIntegerValue((ExpressionSingle)expressionA[0], false, 6);
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
                Assert.Empty(program.Classes);
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
                Assert.Empty(program.Classes);
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

        private void Assert_EmptyClassDeclaration(string name, ClassDeclaration classDec)
        {
            Assert.Equal(name, classDec.Name);
            Assert.Empty(classDec.ClassInstructions);
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