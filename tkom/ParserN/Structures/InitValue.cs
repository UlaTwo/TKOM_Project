using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public class InitValue : IInstruction
    {
        public string Identifier { set; get; }
        public Expression InitedValue { set; get; }

        public InitValue(string id, Expression val)
        {
            Identifier = id;
            InitedValue = val;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("InitValue: " + this.Identifier);
            Console.WriteLine("InitedValue: ");
            InitedValue.ConsoleWrite();
        }

    }
}