using System;
using System.Collections.Generic;
using tkom.InterpreterN;

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

        public void Execute(Scope scp, List<FunctionDeclaration> Functions){

            ///w evaluate chcemy jeszcze wiedzieć, czy ma być zwrócony string czy int
            VarDeclaration varDeclared = scp.GetVar(Identifier); 
            string type = ""; //typ jaki chcemy dostać
            if(varDeclared is Value || varDeclared is StringVarDeclaration || varDeclared is TurtleVarDeclaration ) type ="string";
            if(varDeclared is IntVarDeclaration || varDeclared is IntegerValue) type = "int";
            VarDeclaration inited = InitedValue.Evaluate(scp,Functions, type);
            scp.UpdateVar(Identifier, inited);
        }
    }
}