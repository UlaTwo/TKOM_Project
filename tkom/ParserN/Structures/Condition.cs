using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{

    public class Condition : IConditionType
    {
        public Queue<IConditionQueueType> ConditionONPQueue = new Queue<IConditionQueueType>();
        public Condition()
        {
            ConditionONPQueue = new Queue<IConditionQueueType>();
        }
        public Condition(Queue<IConditionQueueType> conditionONPQueue)
        {
            ConditionONPQueue = conditionONPQueue;
        }
        public void ConsoleWrite()
        {
            Console.WriteLine("Condition in ONP: ");
            foreach (IConditionQueueType exp in ConditionONPQueue)
            {
                exp.ConsoleWrite();
            }
        }

        public bool CheckCondition(Scope scp, List<FunctionDeclaration> Functions)
        {
            Stack<bool> resultStack = new Stack<bool>();
            Stack<int> valStack = new Stack<int>();
            foreach (IConditionQueueType val in ConditionONPQueue)
            {
                //jeśli value z operatorem: +, -, *, / to liczenie dwóch wartości ze stosu
                if (val is Value)
                {
                    Value v = (Value)val;
                    string identifier = v.Identifier;
                    if (identifier == "^" || identifier == "~")
                    {
                        if (resultStack.Count < 2)
                            throw new InterpreterException("Error: Condition.CheckCondition() The condition is not correct. Operands ^ and ~ need bool value.");
                        bool bool2 = resultStack.Pop();
                        bool bool1 = resultStack.Pop();
                        switch (identifier)
                        {
                            case "^":
                                resultStack.Push(bool1 && bool2);
                                break;
                            case "~":
                                resultStack.Push(bool1 || bool2);
                                break;
                            default:
                                break;
                        }
                    }
                    if (identifier == "==" || identifier == "!=" ||
                        identifier == ">" || identifier == "<" ||
                        identifier == "<=" || identifier == ">=")
                    {
                        if (valStack.Count < 2)
                            throw new InterpreterException("Error: Condition.CheckCondition() The condition is not correct. Expecting expressions to compare.");
                        int int2 = valStack.Pop();
                        int int1 = valStack.Pop();
                        switch (identifier)
                        {
                            case "==":
                                resultStack.Push(int1 == int2);
                                break;
                            case "!=":
                                resultStack.Push(int1 != int2);
                                break;
                            case ">":
                                resultStack.Push(int1 > int2);
                                break;
                            case "<":
                                resultStack.Push(int1 < int2);
                                break;
                            case "<=":
                                resultStack.Push(int1 <= int2);
                                break;
                            case ">=":
                                resultStack.Push(int1 >= int2);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    //jeśli jest czymś innym
                    if (val is ConditionSingle)
                    {
                        ConditionSingle condS = (ConditionSingle)val;
                        if (condS.expression is Condition)
                        {
                            Condition newCondition = (Condition)condS.expression;
                            bool newResult = newCondition.CheckCondition(scp, Functions);
                            if (condS.isNegation) resultStack.Push(!newResult);
                            else
                                resultStack.Push(newResult);
                        }

                        if (condS.expression is IExpressionType)
                        {
                            IExpressionType expression = (IExpressionType)condS.expression;
                            int newInt = expression.CountInt(scp, Functions);
                            valStack.Push(newInt);
                        }
                    }
                    else
                    {
                        throw new InterpreterException("Error: Condition.CheckCondition() Not correct value in ConditionONPQueue.");
                    }
                }
            }
            if (resultStack.Count != 1)
                throw new InterpreterException("Error: Condition.CheckCondition() The condition is not correct. ");
            return resultStack.Pop();
        }
    }

    public class ConditionSingle : IConditionType, IConditionQueueType
    {
        // ["!"]( "(", condition_statement, ")" | expression ) ;
        public bool isNegation;
        public IConditionType expression;

        public void ConsoleWrite()
        {
            Console.WriteLine("isNegation: ");
            Console.WriteLine(this.isNegation);
            Console.WriteLine("Conditions: ");
            expression.ConsoleWrite();
        }

        public ConditionSingle(bool isNeg, IConditionType exp)
        {
            isNegation = isNeg;
            expression = exp;
        }
    }
}