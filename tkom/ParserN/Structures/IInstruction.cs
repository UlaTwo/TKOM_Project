using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public interface IInstruction
    {
        void ConsoleWrite() { }
        void Execute(Scope env, List<FunctionDeclaration> Functions) {}
    }
}