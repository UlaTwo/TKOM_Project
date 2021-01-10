using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public class ClassDeclaration
    {
        //class_declaration = "class", name , "{", {class_instruction_block} , "}" ;
        //class_instruction_block = var_declaration | function_declaration ;
        public string Name { get; set; }
        public List<IClassInstruction> ClassInstructions { get; set; }

        public ClassDeclaration()
        {
            List<IClassInstruction> ClassInstructions = new List<IClassInstruction>();
        }
        public ClassDeclaration(string n, List<IClassInstruction> instructions)
        {
            ClassInstructions = instructions;
            Name = n;
        }
        public void ConsoleWrite()
        {
            Console.WriteLine("Nazwa klasy: " + this.Name);
            Console.WriteLine("  instrukcje klasy: ");
            foreach (IClassInstruction i in this.ClassInstructions) i.ConsoleWrite();
        }
    }
        
}