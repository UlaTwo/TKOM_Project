using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public interface IExpressionType : IConditionType
    {
        new void ConsoleWrite();
        VarDeclaration Evaluate(Scope scp,List<FunctionDeclaration> Functions, string type);
        int CountInt(Scope scp, List<FunctionDeclaration> Functions);
    }

}