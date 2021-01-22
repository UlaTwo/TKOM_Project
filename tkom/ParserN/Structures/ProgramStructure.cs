using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class ProgramStructure
    {
        public List<FunctionDeclaration> Functions;

        public ProgramStructure()
        {
            Functions = new List<FunctionDeclaration>();
        }

         public ProgramStructure(List<FunctionDeclaration> fun)
        {
            Functions = fun;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("FunctionDeclarations in Program: ");
            foreach (FunctionDeclaration f in this.Functions) f.ConsoleWrite();
        }

        public void Execute(Scope env)
        {
            //uruchomienie głównej funkcji "go"
            if (this.Functions.Exists(x => x.Identifier == "go"))
            {
                FunctionDeclaration main = this.Functions.Find(x => (x.Identifier == "go"));
                main.Execute(env, this.Functions);
            }
            else
            {
                throw new InterpreterException("Error: there is no function named <go> in program");
            }
        }
    }

}