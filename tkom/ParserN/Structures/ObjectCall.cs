using System;
using System.Collections.Generic;

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

    }
}