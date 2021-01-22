using System;
using System.Collections.Generic;
using tkom.LexerN;
using tkom.ParserN.Structures;

namespace tkom.InterpreterN
{
    public class Scope
    {
        public Stack<CallScope> CallStack;

        public Scope()
        {
            CallStack = new Stack<CallScope>();
        }

        public void DeleteCall()
        {
            CallStack.Pop();
        }

        public void AddCall()
        {
            CallStack.Push(new CallScope());
        }

        public void AddVar(string name, VarDeclaration var)
        {
            CallScope scope = CallStack.Pop();
            scope.AddVariable(name, var);
            CallStack.Push(scope);
        }

        public void UpdateVar(string name, VarDeclaration var)
        {
            CallScope scope = CallStack.Pop();
            scope.UpdateVariable(name, var);
            CallStack.Push(scope);
        }

        public VarDeclaration GetVar(string name)
        {
            CallScope scope = CallStack.Pop();
            VarDeclaration var = scope.GetVariable(name);
            CallStack.Push(scope);
            return var;
        }

        public bool CheckIfVarExists(string name)
        {
            CallScope scope = CallStack.Pop();
            bool i = scope.CheckIfVariableExists(name);
            CallStack.Push(scope);
            return i;
        }

        public void AddVarFunCall(int number, VarDeclaration var)
        {
            CallScope scope = CallStack.Pop();
            scope.AddFunCallVariable(number, var);
            CallStack.Push(scope);
        }

        public VarDeclaration GetFunCallVar(int number)
        {
            CallScope scope = CallStack.Pop();
            VarDeclaration var = scope.GetFunCallVariable(number);
            CallStack.Push(scope);
            return var;
        }
        public void DeleteAllFunCall()
        {
            CallScope scope = CallStack.Pop();
            scope.DeleteAllFunCall();
            CallStack.Push(scope);
        }

        public int GetNumberOfArgumentsInFunCall()
        {
            CallScope scope = CallStack.Pop();
            int var = scope.GetNumberOfArgumentsInFunCall();
            CallStack.Push(scope);
            return var;
        }

        public void ConsoleWriteScope()
        {
            CallScope scope = CallStack.Pop();
            scope.ConsoleWriteScope();
            CallStack.Push(scope);
        }

        //IntValue

        public void SetFunctionIntValue(int value)
        {
            CallScope scope = CallStack.Pop();
            scope.functionReturnedValue = value;
            CallStack.Push(scope);

        }

        public int GetFunctionIntValue()
        {
            CallScope scope = CallStack.Pop();
            int value = scope.functionReturnedValue;
            CallStack.Push(scope);
            return value;
        }

        //obsługa żółwia
        public void SetUsedTurtle(string value)
        {
            CallScope scope = CallStack.Pop();
            scope.usedTurtleName = value;
            CallStack.Push(scope);

        }

        public string GetUsedTurtle()
        {
            CallScope scope = CallStack.Pop();
            string value = scope.usedTurtleName;
            CallStack.Push(scope);
            return value;
        }

        public void AddTurtle(string name, TurtleCoordinates tCor)
        {
            CallScope scope = CallStack.Pop();
            scope.AddTurtle(name, tCor);
            CallStack.Push(scope);
        }
        public void UpdateTurtle(string name, TurtleCoordinates tCor)
        {
            CallScope scope = CallStack.Pop();
            scope.UpdateTurtle(name, tCor);
            CallStack.Push(scope);
        }

        public TurtleCoordinates GetTurtleCoordinates(string name)
        {
            CallScope scope = CallStack.Pop();
            TurtleCoordinates var = scope.GetTurtleCoordinates(name);
            CallStack.Push(scope);
            return var;
        }

        public bool checkIfTurtleHasCoordinates(string name)
        {
            CallScope scope = CallStack.Pop();
            bool var = scope.checkIfTurtleHasCoordinates(name);
            CallStack.Push(scope);
            return var;
        }

    }

    public class CallScope
    {
        public CallScope()
        {
            mapVar = new Dictionary<string, VarDeclaration>();
            funCallVar = new Dictionary<int, VarDeclaration>();
            functionReturnedValue = 0;
            usedTurtleName = "";
            turtles = new Dictionary<string, TurtleCoordinates>();
        }
        Dictionary<string, VarDeclaration> mapVar;
        Dictionary<int, VarDeclaration> funCallVar; 
        public int functionReturnedValue;
        public string usedTurtleName;
        Dictionary<string, TurtleCoordinates> turtles;
        public void ConsoleWriteScope()
        {
            Console.WriteLine("mapVar: ");

            foreach (KeyValuePair<string, VarDeclaration> v in mapVar)
            {
                if (v.Value is IntegerValue) { IntegerValue i = (IntegerValue)v.Value; Console.WriteLine("Key: {0}, Value: {1}", v.Key, i.value); }
                else Console.WriteLine("Key: {0}, Value: {1}",
                v.Key, v.Value.Identifier);
            }

            Console.WriteLine("funCallVar: ");
            foreach (KeyValuePair<int, VarDeclaration> v in funCallVar)
            {
                if (v.Value is IntegerValue) { IntegerValue i = (IntegerValue)v.Value; Console.WriteLine("Key: {0}, Value: {1}", v.Key, i.value); }
                else Console.WriteLine("Key: {0}, Value: {1}",
                v.Key, v.Value.Identifier);
            }

            Console.WriteLine("turtles: ");
            foreach (KeyValuePair<string, TurtleCoordinates> v in turtles)
            {
                // if (v.Value is IntegerValue) { IntegerValue i = (IntegerValue)v.Value; Console.WriteLine("Key: {0}, Value: {1}", v.Key, i.value); }
                Console.WriteLine("Key: {0}, Value: {1} {2}",
                v.Key, v.Value.X, v.Value.Y);
            }
        }
        public void AddVariable(string name, VarDeclaration var)
        {
            if (mapVar.ContainsKey(name))
            {
                throw new InterpreterException(" Error: AddVariable() the variable named " + name + "already exists in scope. ");
            }
            else
                mapVar.Add(name, var);
        }

        public void UpdateVariable(string name, VarDeclaration var)
        {
            mapVar[name] = var;
        }

        public VarDeclaration GetVariable(string name)
        {
            VarDeclaration var;
            if (mapVar.TryGetValue(name, out var))
                return var;
            else
                throw new InterpreterException(" Error:  GetVariable() trying to get variable, that not exists named " + name);
        }

        public bool CheckIfVariableExists(string name)
        {
            VarDeclaration var;
            if (mapVar.TryGetValue(name, out var))
                return true;
            else return false;
        }


        public void AddFunCallVariable(int number, VarDeclaration var)
        {
            if (funCallVar.ContainsKey(number))
            {
                throw new InterpreterException(" Error: AddFunCallVariable() the variable with number " + number + "already exists in scope. ");
            }
            else
                funCallVar.Add(number, var);
        }

        public VarDeclaration GetFunCallVariable(int number)
        {
            VarDeclaration var;
            if (funCallVar.TryGetValue(number, out var))
                return var;
            else
                throw new InterpreterException(" Error: GetFunCallVariable() trying to get variable, that not exists number " + number);
        }

        public void DeleteAllFunCall()
        {
            funCallVar.Clear();
        }

        //obsługa dictionary turtles

        public void AddTurtle(string name, TurtleCoordinates tCor)
        {
            if (turtles.ContainsKey(name))
            {
                throw new InterpreterException(" Error: AddTurtle() the turtle named " + name + "already exists in scope. ");
            }
            else
                turtles.Add(name, tCor);
        }

        public void UpdateTurtle(string name, TurtleCoordinates tCor)
        {
            turtles[name] = tCor;
        }

        public TurtleCoordinates GetTurtleCoordinates(string name)
        {
            TurtleCoordinates var;
            if (turtles.TryGetValue(name, out var))
                return var;
            else
                throw new InterpreterException(" Error:  GetTurtleCoordinates() trying to get turtle, that not exists named " + name);
        }

        public int GetNumberOfArgumentsInFunCall()
        {
            return funCallVar.Count;
        }

        public bool checkIfTurtleHasCoordinates(string name)
        {
            TurtleCoordinates var;
            if (turtles.TryGetValue(name, out var))
                return true;
            return false;
        }

    }
}