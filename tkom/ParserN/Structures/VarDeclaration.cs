using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public class VarDeclaration : IInstruction, IClassInstruction
    {
        public virtual string Identifier { get; set; }
        public virtual void ConsoleWrite() { }

    }

    public class IntVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }

        public IntVarDeclaration(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("IntVarDeclaration: " + this.Identifier);
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
    }

    public class TurtleVarDeclaration : VarDeclaration
    {
        public override string Identifier { get; set; }

        public TurtleVarDeclaration(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("TurtleVarDeclaration: " + this.Identifier);
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
    }

    public class Value : VarDeclaration, IExpressionType, IExpressionQueueType, IConditionQueueType
    {
        public override string Identifier { get; set; }

        public Value(string id) { Identifier = id; }

        public override void ConsoleWrite()
        {
            Console.WriteLine("Value: " + this.Identifier);
        }
    }
}