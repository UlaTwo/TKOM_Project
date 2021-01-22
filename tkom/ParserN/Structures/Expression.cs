using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class Expression : IExpressionType
    {
        public Queue<IExpressionQueueType> ExpressionONPQueue = new Queue<IExpressionQueueType>();

        public Expression()
        {
            ExpressionONPQueue = new Queue<IExpressionQueueType>();
        }

        public Expression(Queue<IExpressionQueueType> expressionONPQueue)
        {
            ExpressionONPQueue = expressionONPQueue;
        }
        public void ConsoleWrite()
        {
            Console.WriteLine("Expression in ONP: ");
            foreach (IExpressionQueueType exp in ExpressionONPQueue)
            {
                exp.ConsoleWrite();
            }

        }

        public VarDeclaration Evaluate(Scope scp,List<FunctionDeclaration> Functions,string type)
        {
            if (type == "string" || type == "turtle")
            {
                if (ExpressionONPQueue.Count != 1)
                {
                    throw new InterpreterException("Error: Expression.Evaluate() ExpressionONPQueue longer then 1. Expecting expression for string variable. Change to single value.");
                }
                IExpressionQueueType exp = ExpressionONPQueue.Peek();
                if (!(exp is ExpressionSingle))
                    throw new InterpreterException("Error: Expression.Evaluate() Not ExpressionSingle in ExpressionONPQueue. Expecting expression for string variable. Change to single value.");
                ExpressionSingle expS = (ExpressionSingle)exp;
                if (!(expS.expressionType is Value))
                    throw new InterpreterException("Error: Expression.Evaluate() Not Value in ExpressionSingle in ExpressionONPQueue. Expecting expression for string variable. Change to single value.");
                if (expS.isMinus == true)
                    throw new InterpreterException("Error: Expression.Evaluate() Minus before Value. Expecting expression for string variable. Change to single value.");
                Value val = (Value)expS.expressionType;
                return val.Evaluate(scp, Functions, type);
            }

            if (type == "int")
            {
                int result = CountInt(scp, Functions);
                IntegerValue intVal = new IntegerValue(result);
                return intVal;
            }
            return null;
        }

        public int CountInt(Scope scp, List<FunctionDeclaration> Functions)
        {
            Stack<int> resultStack = new Stack<int>();
            foreach (IExpressionQueueType val in ExpressionONPQueue)
            {
                //jeśli value z operatorem: +, -, *, / to liczenie dwóch wartości ze stosu
                if (val is Value)
                {
                    Value v = (Value)val;
                    string identifier = v.Identifier;
                    if (identifier == "+" || identifier == "-" || identifier == "*" || identifier == "/")
                    {
                        int int2 = resultStack.Pop();
                        int int1 = resultStack.Pop();
                        switch (identifier)
                        {
                            case "+":
                                resultStack.Push(int1 + int2);
                                break;
                            case "-":
                                resultStack.Push(int1 - int2);
                                break;
                            case "*":
                                resultStack.Push(int1 * int2);
                                break;
                            case "/":
                                if(int2 == 0)
                                    throw new InterpreterException("Error: Expression.CountInt() Incorrect: delivery with 0.");
                                resultStack.Push(int1 / int2);
                                
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    //jeśli jest czymś innym
                    if (val is ExpressionSingle)
                    {
                        ExpressionSingle exp = (ExpressionSingle) val;
                        int result = exp.CountInt(scp, Functions);
                        resultStack.Push(result);
                    }
                    else{
                        throw new InterpreterException("Error: Expression. Count() Not correct value in ExpressionONPQueue.");
                    }
                }
            }
            return resultStack.Pop();
        }
    }

    public class ExpressionSingle : IExpressionType, IExpressionQueueType
    {
        //F =  ["-"] ( integer | fun_call | color | name | "(", expression , ")" ) ;
        public bool isMinus;
        public IExpressionType expressionType;

        public ExpressionSingle(bool isminus, IExpressionType expType){
            isMinus = isminus;
            expressionType = expType;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("isMinus: ");
            Console.WriteLine(this.isMinus);
            Console.WriteLine("ExpressionsF with sign: ");
            expressionType.ConsoleWrite();
        }
        public VarDeclaration Evaluate(Scope scp,List<FunctionDeclaration> Functions, string type)
        {
            return null;
        }

        public int CountInt(Scope scp, List<FunctionDeclaration> Functions){
            int result = expressionType.CountInt(scp, Functions);
            if(isMinus){
                return result*(-1);
            }
            return result;
        }
    }
    public class IntegerValue : VarDeclaration, IExpressionType
    {
        public int value;
        public IntegerValue(int i) { value = i; }
        public override void ConsoleWrite()
        {
            Console.WriteLine("Int value: ");
            Console.WriteLine(this.value);
        }

        public VarDeclaration Evaluate(Scope scp,List<FunctionDeclaration> Functions, string type)
        {
            return this;
        }

        public int CountInt(Scope scp, List<FunctionDeclaration> Functions){
            return value;
        }

    }
}