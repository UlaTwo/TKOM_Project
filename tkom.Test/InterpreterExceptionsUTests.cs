using System;
using Xunit;
using tkom.ParserN;
using tkom.InterpreterN;
using tkom.ParserN.Structures;
using tkom.LexerN;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace tkom.Test
{
    public class InterpreteExceptionsUTests
    {
        ProgramStructure returnProgram(StringReader stream)
        {
            Lexer lekser = new Lexer(stream);
            Parser parser = new Parser(lekser);
            ProgramStructure program = new ProgramStructure();
            return parser.ParseProgram();
        }

        [Fact]
        public void EmptyStack_Interprete()
        {
            string s = "def main(){}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,"Error: there is no function named <go> in program");

            }
        }

        [Fact]
        public void ValueCountInt_NotInteger_Interprete()
        {
            string s = "def go(){int a; a=b;}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,"Error: Value.CountInt() The variable named: b is not integer.");

            }
        }

        [Fact]
        public void ValueEvaluate_NotString_Interprete()
        {
            string s = "def go(){string a; int c; c=2; a=c;}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,"Error: Value.Evaluate() The variable named:c exists in scope but is not string value.");
            }
        }

        [Fact]
        public void ObjectCall_NoTurtleInScope_Interprete()
        {
            string s = "def go(){string a; int c; c=2; a{ create(0,0 ); };}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,"Error: ObjectCall.Execute() There is no declared turtle named a");
            }
        }

        [Fact]
        public void FunctionDeclaration_ArgumentsFunctionGo_Interprete()
        {
            string s = "def go(string a){}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,"Error: FunctionDeclaration.Execute Function <go> should have no arguments");
            }
        }

        [Fact]
        public void FunctionCall_IncorrectArgumentsNumber_Interprete()
        {
            string s = "def f(int s){} def go(){f();}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionCall.Execute() Number of arguments passed to function call <f> does not match number of arguments in function declaration (expected 1 arguments)");
            }
        }

        [Fact]
        public void FunctionCall_TooManyArguments_Interprete()
        {
            string s = "def f(int s){} def go(){f(a, b);}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionCall.Execute() Number of arguments passed to function call <f> does not match number of arguments in function declaration  (expected 1 arguments)");
            }
        }

        [Fact]
        public void FunctionCall_NoFunctionDefinition_Interprete()
        {
            string s = "def go(){f(a, b);}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionCall.Execute() There is no definition in the program of function called f");
            }
        }

        [Fact]
        public void FunctionCall_CountInt_FunctionNotReturnInt_Interprete()
        {
            string s = "def f(){} def go(){int a; a=f();}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionCall.CountInt() In expression is called a function f that doesn't return int.");
            }
        }

        [Fact]
        public void FunctionCall_CountInt_NoFunctionInProgram_Interprete()
        {
            string s = "def go(){int a; a=f();}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionCall.CountInt() There is no function named <f> in program.");
            }
        }

        [Fact]
        public void Expression_StringEvaluate_TooLongQueue_Interprete()
        {
            string s = "def go(){string a; a=3+2;}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: Expression.Evaluate() ExpressionONPQueue longer then 1. Expecting expression for string variable. Change to single value.");
            }
        }

        [Fact]
        public void Expression_StringEvaluate_NotValue_Interprete()
        {
            string s = "def go(){string a; a=3;}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: Expression.Evaluate() Not Value in ExpressionSingle in ExpressionONPQueue. Expecting expression for string variable. Change to single value.");
            }
        }

        [Fact]
        public void Expression_StringEvaluate_IncorrectMinus_Interprete()
        {
            string s = "def go(){string a; a=-c;}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: Expression.Evaluate() Minus before Value. Expecting expression for string variable. Change to single value.");
            }
        }

        [Fact]
        public void Condition_NotCorrect_Interprete()
        {
            string s = "def go(){ if(3^2 ){}; }";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: Condition.CheckCondition() The condition is not correct. Operands ^ and ~ need bool value.");
            }
        }

        [Fact]
        public void BuildInFunctions_NotDeclaredCoordinates_Interprete()
        {
            string s = "def go(){ turtle tu; tu{circle(2,red); };}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionsBuiltIn.getAndCheckTurtle() Trying to use function for turtle <tu> that doesn't have daclared coordinates.");
            }
        }

        [Fact]
        public void BuildInFunctions_IncorrectArguments_Interprete()
        {
            string s = "def go(){ turtle tu; tu{create(); };}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionsBuiltIn.create() The number of passed arguments schould be 2. Instead it is: 0");
            }
        }

        [Fact]
        public void BuildInFunctions_TurtleFunctionNotInBlock_Interprete()
        {
            string s = "def go(){ turtle tu; create(0,0); }";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error: FunctionsBuiltIn.create() Trying to use turtle function not in turtle funtion block.");
            }
        }

        [Fact]
        public void BuildInFunctions_TurtleForwardAngle_Interprete()
        {
            string s = "def go(){ turtle tu; tu{create(0,0);forward(4, -3,red); };}";
            using (var stream = new StringReader(s))
            {
                Assert_InterpreterException(stream,
                "Error:  FunctionsBuiltIn.forward() The angle passed to function forward() is not correct.");
            }
        }

        private void Assert_InterpreterException(StringReader stream, string expectedMessage)
        {
            bool exceptionThrown = false;
            try
            {
                ProgramStructure program = returnProgram(stream);
                Interpreter interpreter = new Interpreter(program);
                interpreter.Run();
            }
            catch (InterpreterException e)
            {
                exceptionThrown = true;
                Assert.Equal(expectedMessage, e.Message);
            }
            Assert.True(exceptionThrown);
        }

    }
}