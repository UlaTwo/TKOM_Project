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
    public class ScopeUTest
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
            string s = "def go(){}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Interpreter interpreter = new Interpreter(program);
                interpreter.Run();
                Assert.Empty(interpreter.scp.CallStack);
            }
        }

        [Fact]
        public void StackIntVarDeclaration_Interprete()
        {
            string s = "def go(){int a;}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                // Interpreter interpreter = new Interpreter(program);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<IntVarDeclaration>(program.Functions.First().Instructions.First());
                IntVarDeclaration intVar = (IntVarDeclaration)program.Functions.First().Instructions.First();
                intVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntVarDeclaration>(scp.GetVar("a"));
            }
        }

        [Fact]
        public void StackStringVarDeclaration_Interprete()
        {
            string s = "def go(){string a;}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<StringVarDeclaration>(program.Functions.First().Instructions.First());
                StringVarDeclaration stringVar = (StringVarDeclaration)program.Functions.First().Instructions.First();
                stringVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<StringVarDeclaration>(scp.GetVar("a"));
            }
        }

        [Fact]
        public void StackTurtleVarDeclaration_Interprete()
        {
            string s = "def go(){turtle tu;}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<TurtleVarDeclaration>(program.Functions.First().Instructions.First());
                TurtleVarDeclaration stringVar = (TurtleVarDeclaration)program.Functions.First().Instructions.First();
                stringVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("tu"));
                Assert.IsType<TurtleVarDeclaration>(scp.GetVar("tu"));
            }
        }

        [Fact]
        public void StackIntVarInit_Interprete()
        {
            string s = "def go(){int a; a = 5;}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;

                Assert.IsType<IntVarDeclaration>(program.Functions.First().Instructions.First());
                IntVarDeclaration intVar = (IntVarDeclaration)program.Functions.First().Instructions.First();
                intVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntVarDeclaration>(scp.GetVar("a"));

                Assert.IsType<InitValue>(program.Functions.First().Instructions.Skip(1).First());
                InitValue init = (InitValue)program.Functions.First().Instructions.Skip(1).First();
                init.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntegerValue>(scp.GetVar("a"));
                IntegerValue intV = (IntegerValue)scp.GetVar("a");
                Assert.Equal(5, intV.value);

            }
        }

        [Fact]
        public void StackStringVarInit_Interprete()
        {
            string s = "def go(){string a; a=nazwa;}";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;

                Assert.IsType<StringVarDeclaration>(program.Functions.First().Instructions.First());
                StringVarDeclaration intVar = (StringVarDeclaration)program.Functions.First().Instructions.First();
                intVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<StringVarDeclaration>(scp.GetVar("a"));

                Assert.IsType<InitValue>(program.Functions.First().Instructions.Skip(1).First());
                InitValue init = (InitValue)program.Functions.First().Instructions.Skip(1).First();
                init.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<Value>(scp.GetVar("a"));
                Value intV = (Value)scp.GetVar("a");
                Assert.Equal("nazwa", intV.Identifier);

            }
        }

        [Fact]
        public void StackTurtleVarInitCoordinates_Interprete()
        {
            string s = "def go(){turtle tu; tu{ create(0,0); }; }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;
                Assert.IsType<TurtleVarDeclaration>(program.Functions.First().Instructions.First());
                TurtleVarDeclaration stringVar = (TurtleVarDeclaration)program.Functions.First().Instructions.First();
                stringVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("tu"));
                Assert.IsType<TurtleVarDeclaration>(scp.GetVar("tu"));

                Assert.IsType<ObjectCall>(program.Functions.First().Instructions.Skip(1).First());
                ObjectCall objectCall = (ObjectCall)program.Functions.First().Instructions.Skip(1).First();
                objectCall.Execute(scp, Functions);

                bool checkTurtle = scp.checkIfTurtleHasCoordinates("tu");
                TurtleCoordinates tuCor = scp.GetTurtleCoordinates("tu");

                Assert.True(checkTurtle);
                Assert.Equal(0, tuCor.X);
                Assert.Equal(0, tuCor.Y);
            }
        }

        [Fact]
        public void PassIntVarToFunction_Interprete()
        {
            string s = "def f(int b){} def go(){int a; a = 5; f(a); }";
            using (var stream = new StringReader(s))
            {
                ProgramStructure program = returnProgram(stream);
                Scope scp = new Scope();

                scp.AddCall();
                //first instruction
                List<FunctionDeclaration> Functions = program.Functions;

                Assert.IsType<IntVarDeclaration>(program.Functions.Skip(1).First().Instructions.First());
                IntVarDeclaration intVar = (IntVarDeclaration)program.Functions.Skip(1).First().Instructions.First();
                intVar.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntVarDeclaration>(scp.GetVar("a"));

                Assert.IsType<InitValue>(program.Functions.Skip(1).First().Instructions.Skip(1).First());
                InitValue init = (InitValue)program.Functions.Skip(1).First().Instructions.Skip(1).First();
                init.Execute(scp, Functions);
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntegerValue>(scp.GetVar("a"));
                IntegerValue intV = (IntegerValue)scp.GetVar("a");
                Assert.Equal(5, intV.value);

                Assert.IsType<FunctionCall>(program.Functions.Skip(1).First().Instructions.Skip(2).First());
                FunctionCall funCall = (FunctionCall) program.Functions.Skip(1).First().Instructions.Skip(2).First();
                funCall.Execute(scp,Functions);

                //after function call
                Assert.NotEmpty(scp.CallStack);
                Assert.True(scp.CheckIfVarExists("a"));
                Assert.IsType<IntegerValue>(scp.GetVar("a"));
                 intV = (IntegerValue)scp.GetVar("a");
                Assert.Equal(5, intV.value);
            }
        }
    }
}