using System;
using System.Collections.Generic;
using tkom.LexerN;

namespace tkom.ParserN.Structures
{
    public class ProgramStructure{
        public List<FunctionDeclaration> Functions;
        public List <ClassDeclaration> Classes;

        public ProgramStructure(){
            Functions = new List<FunctionDeclaration>();
            Classes = new  List<ClassDeclaration>();
        }

        public void ConsoleWrite(){
            Console.WriteLine("FunctionDeclarations in Program: ");
            foreach(FunctionDeclaration f in this.Functions) f.ConsoleWrite();
            Console.WriteLine("Classes in Program: ");
            foreach(ClassDeclaration c in this.Classes) c.ConsoleWrite();
        }
    } 

}