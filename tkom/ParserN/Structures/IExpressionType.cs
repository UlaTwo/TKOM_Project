using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public interface IExpressionType : IConditionType
    {
        new void ConsoleWrite();
    }

}