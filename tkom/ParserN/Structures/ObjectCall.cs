using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class ObjectCall : IInstruction
    {
        public string ObjectName { set; get; }
        public List<FunctionCall> Functions { set; get; }

        public ObjectCall()
        {
            Functions = new List<FunctionCall>();
        }
        public ObjectCall(string id, List<FunctionCall> funs)
        {
            ObjectName = id;
            Functions = funs;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("ObjectCall: " + this.ObjectName);
            Console.WriteLine("Functions: ");
            foreach (FunctionCall a in this.Functions) a.ConsoleWrite();
        }

        public void Execute(Scope scp, List<FunctionDeclaration> Functions)
        {
            if ( (scp.CheckIfVarExists(ObjectName) == false) || !(scp.GetVar(ObjectName) is TurtleVarDeclaration) )
            {
                throw new InterpreterException("Error: ObjectCall.Execute() There is no declared turtle named "+ObjectName);
            }
            scp.SetUsedTurtle(ObjectName);

            foreach (FunctionCall a in this.Functions) a.Execute(scp,Functions);
            scp.SetUsedTurtle("");
        }

    }
}