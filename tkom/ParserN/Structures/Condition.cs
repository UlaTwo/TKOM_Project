using System;
using System.Collections.Generic;

namespace tkom.ParserN.Structures
{

    public class Condition : IConditionType
    {
        public Stack<Value> SignStack = new Stack<Value>();
        public Queue<IConditionQueueType> ConditionONPQueue = new Queue<IConditionQueueType>();
        public Condition()
        {
            SignStack = new Stack<Value>();
            ConditionONPQueue = new Queue<IConditionQueueType>();
        }
        public void ConsoleWrite()
        {
            Console.WriteLine("Condition in ONP: ");
            foreach (IConditionQueueType exp in ConditionONPQueue)
            {
                exp.ConsoleWrite();
            }

        }
    }

    public class ConditionSingle : IConditionType, IConditionQueueType
    {
        //F =  ["-"] ( integer | fun_call | color | name | "(", expression , ")" ) ;
        public bool isNegation;
        public IConditionType expression;

        public void ConsoleWrite()
        {
            Console.WriteLine("isNegation: ");
            Console.WriteLine(this.isNegation);
            Console.WriteLine("Conditions: ");
            expression.ConsoleWrite();
        }
    }
}