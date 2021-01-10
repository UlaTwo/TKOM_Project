using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{ 
    public class FunctionDeclaration : IClassInstruction{
        public string Identifier {get; set;}
        public bool isIntReturned {get; set; }
        public List<VarDeclaration> Arguments {get; set; }

        public List<IInstruction> Instructions {get; set; }

        public FunctionDeclaration(){
            Arguments = new List<VarDeclaration>();
            Instructions = new  List<IInstruction>();
        }

        public FunctionDeclaration( bool isIntR,string identifier, List<VarDeclaration> arguments,List<IInstruction> block){
            Identifier = identifier;
            isIntReturned = isIntR;
            Arguments = arguments;
            Instructions = block;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("Nazwa funkcji: "+this.Identifier+" isIntReturned:"+this.isIntReturned);
            Console.WriteLine("  argumenty: ");
            foreach(VarDeclaration a in this.Arguments) a.ConsoleWrite();
            Console.WriteLine("  instrukcje: ");
            foreach(IInstruction i in this.Instructions) i.ConsoleWrite();
        }
    }
}