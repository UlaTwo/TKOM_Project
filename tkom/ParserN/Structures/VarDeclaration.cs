using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class VarDeclaration : IInstruction
    {
        public virtual string Identifier { get; set; }
        public virtual void ConsoleWrite() { }
        public virtual void Execute(Scope env, List<FunctionDeclaration> Functions) { }

    }

    public class IntVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }

        public IntVarDeclaration(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("IntVarDeclaration: " + this.Identifier);
        }

        public override void Execute(Scope env, List<FunctionDeclaration> Functions)
        {
            env.AddVar(Identifier, this);
        }
    }

    public class StringVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }

        public StringVarDeclaration(string id) { Identifier = id; }
        public override void ConsoleWrite()
        {
            Console.WriteLine("StringVarDeclaration: " + this.Identifier);
        }

        public override void Execute(Scope env, List<FunctionDeclaration> Functions)
        {
            env.AddVar(Identifier, this);
        }
    }

    public class TurtleVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }

        public TurtleVarDeclaration(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("TurtleVarDeclaration: " + this.Identifier);
        }

        public override void Execute(Scope env, List<FunctionDeclaration> Functions)
        {
            env.AddVar(Identifier, this);
        }
    }

    public class ClassVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }
        public string ClassName { get; set; }

        public ClassVarDeclaration(string id, string name_c) { Identifier = id; ClassName = name_c; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("ClassVarDeclaration: " + this.ClassName + this.Identifier);
        }
        public override void Execute(Scope env, List<FunctionDeclaration> Functions)
        {
            env.AddVar(Identifier, this);
        }
    }

    public class Value : VarDeclaration, IExpressionType, IExpressionQueueType, IConditionQueueType
    {
        public override string Identifier { get; set; }

        public Value(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("Value: " + this.Identifier);
        }
        public override void Execute(Scope scp, List<FunctionDeclaration> Functions)
        {
            scp.AddVar(Identifier, this);
        }

        public VarDeclaration Evaluate(Scope scp, List<FunctionDeclaration> Functions, string type)
        {
            if (scp.CheckIfVarExists(Identifier))
            {
                VarDeclaration varS = scp.GetVar(Identifier);
                if (varS is TurtleVarDeclaration && type == "turtle") { return varS; }
                if ( (varS is Value || varS is StringVarDeclaration) && type == "string") { return varS; }
                else
                {
                    if (type == "string")
                    {
                        throw new InterpreterException("Error: Value.Evaluate() The variable named:" + Identifier + " exists in scope but is not string value.");
                    }
                    else
                    {
                        throw new InterpreterException("Error: Value.Evaluate() The variable named:" + Identifier + " exists in scope but is not turtle value.");
                    }
                }
            }
            else
            {
                return this;
            }
        }

        public int CountInt(Scope scp, List<FunctionDeclaration> Functions)
        {
            // sprawdzenie, czy jest to zmienna int zadeklarowana w scope
            if (scp.CheckIfVarExists(Identifier))
            {
                VarDeclaration varIS = scp.GetVar(Identifier);
                if (varIS is IntegerValue)
                {
                    IntegerValue intVal = (IntegerValue)varIS;
                    return intVal.value;
                }
            }
            throw new InterpreterException("Error: Value.CountInt() The variable named: " + Identifier + " is not integer.");
        }

    }
}