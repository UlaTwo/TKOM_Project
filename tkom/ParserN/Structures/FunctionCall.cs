using System;
using System.Collections.Generic;
using tkom.InterpreterN;

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

        public void Execute(Scope scp, List<FunctionDeclaration> Functions)
        {
            //lista typów argumentów, których oczekuje dana funkcja
            List<string> argumentsTypes = new List<string>();

            // dla danej nazwy funkcji zwrócenie listy typów argumentów
            if (Functions.Exists(x => x.Identifier == FunctionName))
            {
                FunctionDeclaration fun = Functions.Find(x => (x.Identifier == FunctionName));
                argumentsTypes = fun.GetArgumentsType();
                //sprawdzenie, czy zgadza się długość
                if (argumentsTypes != null)
                {
                    if ((ArgumentValues == null && argumentsTypes.Count != 0))
                        throw new InterpreterException("Error: FunctionCall.Execute() Number of arguments passed to function call <" + FunctionName + "> does not match number of arguments in function declaration (expected "+argumentsTypes.Count +" arguments)");
                    else if (ArgumentValues!= null && argumentsTypes.Count != ArgumentValues.Count)
                    {
                        throw new InterpreterException("Error: FunctionCall.Execute() Number of arguments passed to function call <" + FunctionName + "> does not match number of arguments in function declaration  (expected "+argumentsTypes.Count +" arguments)");
                    }
                }
                if(argumentsTypes ==null && ArgumentValues!=null)
                    throw new InterpreterException("Error: FunctionCall.Execute() Number of arguments passed to function call <" + FunctionName + "> does not match number of arguments in function declaration  (expected no arguments)");
            }
            else
            {
                //sprawdzenie, czy jest to funkcja wbudowana
                FunctionsBuiltIn funBuilt = new FunctionsBuiltIn();
                bool funIsBuiltIn = funBuilt.checkFunction(FunctionName);
                if (funIsBuiltIn == false)
                    throw new InterpreterException("Error: FunctionCall.Execute() There is no definition in the program of function called " + FunctionName);
                argumentsTypes = funBuilt.ArgsTypesFunction(FunctionName);
            }


            //zbudowanie argumentów funkcji
            if (this.ArgumentValues != null)
                for (int i = 0; i < this.ArgumentValues.Count; i++)
                {
                    VarDeclaration var = this.ArgumentValues[i].Evaluate(scp, Functions, argumentsTypes[i]);
                    scp.AddVarFunCall(i, var);
                }

            //uruchomienie funkcji, jeśli jest funkcją wbudowaną
            FunctionsBuiltIn funs = new FunctionsBuiltIn();
            bool ifFunBuiltIn = funs.runFunction(scp, FunctionName);
            if (ifFunBuiltIn == false)
            {
                //odszukuje funkcji, i jej uruchomienie
                if (Functions.Exists(x => x.Identifier == FunctionName))
                {
                    FunctionDeclaration fun = Functions.Find(x => (x.Identifier == FunctionName));
                    fun.Execute(scp, Functions);
                }
                else
                    throw new InterpreterException("Error: there is no function named " + FunctionName + " in program ");
            }

            //usunięcie wszystkich elementów z listy argumentów
            scp.DeleteAllFunCall();
        }

        public VarDeclaration Evaluate(Scope scp, List<FunctionDeclaration> Functions, string type)
        {
            return null;
        }

        public int CountInt(Scope scp, List<FunctionDeclaration> Functions)
        {
            if (Functions.Exists(x => x.Identifier == FunctionName))
            {
                bool returnInt = true;
                FunctionDeclaration fun = Functions.Find(x => (x.Identifier == FunctionName));
                returnInt = fun.isIntReturned;
                if (returnInt == false) throw new InterpreterException("Error: FunctionCall.CountInt() In expression is called a function " + FunctionName + " that doesn't return int.");

                Execute(scp, Functions);
                return scp.GetFunctionIntValue();
            }
            else
            {
                throw new InterpreterException("Error: FunctionCall.CountInt() There is no function named <" + FunctionName + "> in program.");
            }
        }

    }
}