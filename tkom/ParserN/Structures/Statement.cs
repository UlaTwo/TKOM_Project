using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class Statement : IInstruction
    {
        public string Type; //while or if
        public Condition Condition { set; get; }
        public List<IInstruction> Instructions { set; get; }

        public Statement()
        {
            Instructions = new List<IInstruction>();
        }
        public Statement(string type, Condition cond, List<IInstruction> ins)
        {
            Type = type;
            Condition = cond;
            Instructions = ins;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("StatementType: " + this.Type);
            Console.WriteLine("Condition: ");
            this.Condition.ConsoleWrite();
            Console.WriteLine("Instructions: ");
            foreach (IInstruction a in this.Instructions) a.ConsoleWrite();
        }

        public void Execute(Scope scp, List<FunctionDeclaration> Functions)
        {
            if (Type == "if")
            {
                if (Condition.CheckCondition(scp, Functions))
                {
                    foreach (IInstruction i in Instructions) i.Execute(scp, Functions);
                }
            }
            if (Type == "while")
            {
                while (Condition.CheckCondition(scp, Functions))
                {
                    foreach (IInstruction i in Instructions) i.Execute(scp, Functions);
                }
            }
        }

    }
}