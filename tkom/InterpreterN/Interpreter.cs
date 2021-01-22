using System;
using System.Collections.Generic;
using tkom.LexerN;
using tkom.ParserN.Structures;

namespace tkom.InterpreterN
{
    public class Interpreter
    {
        ProgramStructure program;
        public Scope scp;

        public Interpreter(ProgramStructure Program){
            program = Program;
            scp = new Scope();
        }
        
        public void Run(){
            program.Execute(scp);
        }

    }
}