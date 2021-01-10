using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public class FunctionCall : IInstruction, IExpressionType
    {
        public string FunctionName { set; get; }
        public List<IExpressionType> ArgumentValues { set; get; }

        public FunctionCall()
        {
            ArgumentValues = new List<IExpressionType>();
        }
        public FunctionCall(string id, List<IExpressionType> arguments)
        {
            FunctionName = id;
            ArgumentValues = arguments;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("FunctionCall: " + this.FunctionName);
            Console.WriteLine("Arguments: ");
            if (this.ArgumentValues != null) foreach (IExpressionType a in this.ArgumentValues) a.ConsoleWrite();
        }

    }
}