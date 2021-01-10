using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{
    public class Expression : IExpressionType
    {
        //potrzebne do onp
        public Stack<Value> SignStack = new Stack<Value>();
        public Queue<IExpressionQueueType> ExpressionONPQueue = new Queue<IExpressionQueueType>();

        public Expression()
        {
            SignStack = new Stack<Value>();
            ExpressionONPQueue = new Queue<IExpressionQueueType>();
        }
        public void ConsoleWrite()
        {
            Console.WriteLine("Expression in ONP: ");
            foreach(IExpressionQueueType exp in ExpressionONPQueue){
                exp.ConsoleWrite();
            }

        }
    }

    public class ExpressionSingle : IExpressionType, IExpressionQueueType
    {
        //F =  ["-"] ( integer | fun_call | color | name | "(", expression , ")" ) ;
        public bool isMinus;
        public IExpressionType expressionType;

        public void ConsoleWrite()
        {
            Console.WriteLine("isMinus: ");
            Console.WriteLine(this.isMinus);
            Console.WriteLine("ExpressionsF with sign: ");
            expressionType.ConsoleWrite();
        }
    }
    public class IntegerValue : IExpressionType
    {
        public int value;
        public IntegerValue(int i) { value = i; }
        public void ConsoleWrite()
        {
            Console.WriteLine("Int value: ");
            Console.WriteLine(this.value);
        }

    }
}
