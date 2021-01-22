using Xunit;
using tkom.ParserN;
using tkom.ParserN.Structures;
using tkom.LexerN;
using tkom.InterpreterN;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
namespace tkom.Test
{
    public class StructuresEvaluateUTest
    {
        ProgramStructure returnProgram(StringReader stream)
        {
            Lexer lekser = new Lexer(stream);
            Parser parser = new Parser(lekser);
            ProgramStructure program = new ProgramStructure();
            return parser.ParseProgram();
        }

        [Fact]
        public void InitValue_Evaluate()
        {
            string s = "def go(){ int a; a=2+3*4; }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                List<FunctionDeclaration> Functions = new List<FunctionDeclaration> ();
                Assert.IsType<InitValue>(program.Functions.First().Instructions.Skip(1).First());
                InitValue initVal = (InitValue) program.Functions.First().Instructions.Skip(1).First();
                IntegerValue inited = (IntegerValue) initVal.InitedValue.Evaluate(scp, Functions, "int");
                Assert.Equal(14,inited.value);
            }
        }

        [Fact]
        public void InitValue2_Evaluate()
        {
            string s = "def go(){ int a; a=(2+3)*4; }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                List<FunctionDeclaration> Functions = new List<FunctionDeclaration> ();
                Assert.IsType<InitValue>(program.Functions.First().Instructions.Skip(1).First());
                InitValue initVal = (InitValue) program.Functions.First().Instructions.Skip(1).First();
                IntegerValue inited = (IntegerValue) initVal.InitedValue.Evaluate(scp, Functions, "int");
                Assert.Equal(20,inited.value);
            }
        }

        [Fact]
        public void Condition_Evaluate()
        {
            string s = "def go(){ int a; a=5; if((a>4)^(a<9)){};}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                IntegerValue a = new IntegerValue(5);
                scp.AddCall();
                scp.AddVar("a", a);
                List<FunctionDeclaration> Functions = new List<FunctionDeclaration> ();
                Assert.IsType<Statement>(program.Functions.First().Instructions.Skip(2).First());
                Statement statement = (Statement) program.Functions.First().Instructions.Skip(2).First();
                bool condition = statement.Condition.CheckCondition(scp, Functions);
                Assert.True(condition);
            }
        }

        [Fact]
        public void Condition2_Evaluate()
        {
            string s = "def go(){ int a; a=5; if((a>6)~(a<3)){};}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                IntegerValue a = new IntegerValue(5);
                scp.AddCall();
                scp.AddVar("a", a);
                List<FunctionDeclaration> Functions = new List<FunctionDeclaration> ();
                Assert.IsType<Statement>(program.Functions.First().Instructions.Skip(2).First());
                Statement statement = (Statement) program.Functions.First().Instructions.Skip(2).First();
                bool condition = statement.Condition.CheckCondition(scp, Functions);
                Assert.False(condition);
            }
        }

        [Fact]
        public void ConditionWithFunctionCall_Evaluate()
        {
            string s = "def int f(){return(3);} def go(){ int a; a=5; if((a>6)~(a<f())){};}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                IntegerValue a = new IntegerValue(5);
                scp.AddCall();
                scp.AddVar("a", a);
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<Statement>(program.Functions.Skip(1).First().Instructions.Skip(2).First());
                Statement statement = (Statement) program.Functions.Skip(1).First().Instructions.Skip(2).First();
                bool condition = statement.Condition.CheckCondition(scp, Functions);
                Assert.False(condition);
            }
        }

        [Fact]
        public void InitValueWithFunctionCall_Evaluate()
        {
            string s = " def int f(int c){return(c*2);} def go(){ int a; a=f(a)+3*4; }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                IntegerValue a = new IntegerValue(5);
                scp.AddCall();
                scp.AddVar("a", a);///a is 5 in the scope
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<InitValue>(program.Functions.Skip(1).First().Instructions.Skip(1).First());
                InitValue initVal = (InitValue) program.Functions.Skip(1).First().Instructions.Skip(1).First();
                IntegerValue inited = (IntegerValue) initVal.InitedValue.Evaluate(scp, Functions, "int");
                Assert.Equal(22,inited.value);
            }
        }

        [Fact]
        public void InitValueWithRecursiveFunctionCallFactorial_Evaluate()
        {
            string s = " def int f(int n){if (n <= 1){return(1); };if(n>1){ return (n*f(n-1) ); }; } def go(){ int a; a=f(a); }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);

                Scope scp = new Scope();
                IntegerValue a = new IntegerValue(5);
                scp.AddCall();
                scp.AddVar("a", a);  ///a is 5 in the scope
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<InitValue>(program.Functions.Skip(1).First().Instructions.Skip(1).First());
                InitValue initVal = (InitValue) program.Functions.Skip(1).First().Instructions.Skip(1).First();
                IntegerValue inited = (IntegerValue) initVal.InitedValue.Evaluate(scp, Functions, "int");
                Assert.Equal(120,inited.value);
            }
        }
    }
}