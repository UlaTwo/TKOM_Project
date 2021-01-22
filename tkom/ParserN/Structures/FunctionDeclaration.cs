using System;
using System.Collections.Generic;
using tkom.InterpreterN;

namespace tkom.ParserN.Structures
{
    public class FunctionDeclaration 
    {
        public string Identifier { get; set; }
        public bool isIntReturned { get; set; }
        public List<VarDeclaration> Arguments { get; set; }

        public List<IInstruction> Instructions { get; set; }

        public FunctionDeclaration()
        {
            Arguments = new List<VarDeclaration>();
            Instructions = new List<IInstruction>();
        }

        public FunctionDeclaration(bool isIntR, string identifier, List<VarDeclaration> arguments, List<IInstruction> block)
        {
            Identifier = identifier;
            isIntReturned = isIntR;
            Arguments = arguments;
            Instructions = block;
        }

        public void ConsoleWrite()
        {
            Console.WriteLine("Nazwa funkcji: " + this.Identifier + " isIntReturned:" + this.isIntReturned);
            Console.WriteLine("  argumenty: ");
            foreach (VarDeclaration a in this.Arguments) a.ConsoleWrite();
            Console.WriteLine("  instrukcje: ");
            foreach (IInstruction i in this.Instructions) i.ConsoleWrite();
        }

        public void Execute(Scope scp, List<FunctionDeclaration> Functions)
        {
            if (Identifier != "go")
            {
                List<VarDeclaration> argsList = new List<VarDeclaration>();
                Dictionary<string, TurtleCoordinates> turtlesArguments = new Dictionary<string, TurtleCoordinates>();

                //zachowanie potrzebnych zmiennych z poprzedniego elementu stosu - argumentów funkcji oraz współrzędnych przekazanego żółwia
                for (int i = 0; i < Arguments.Count; i++)
                {
                    VarDeclaration value = scp.GetFunCallVar(i);
                    argsList.Add(value);
                    //jeśli zmienną jest żółw, to pobieramy jego współrzędne i dodajemy do słownika
                    if (Arguments[i] is TurtleVarDeclaration)
                    {
                        if (scp.checkIfTurtleHasCoordinates(value.Identifier))
                        {
                            TurtleCoordinates newTCoor = scp.GetTurtleCoordinates(value.Identifier);
                            turtlesArguments.Add(Arguments[i].Identifier, newTCoor);
                        }
                    }

                }

                scp.AddCall();
                foreach (VarDeclaration a in this.Arguments) scp.AddVar(a.Identifier, a);
                foreach (KeyValuePair<string, TurtleCoordinates> t in turtlesArguments) scp.AddTurtle(t.Key, t.Value);

                //dodanie argumentów przekazanych w wywołaniu funkcji
                for (int i = 0; i < Arguments.Count; i++)
                {
                    scp.UpdateVar(Arguments[i].Identifier, argsList[i]);
                }

                //wykonianie po kolei instrukcji funkcji
                foreach (IInstruction i in this.Instructions) i.Execute(scp, Functions);

                //sprawdzenie, czy funkcja zwraca int'a
                int returnedValue = 0;
                if (isIntReturned) { returnedValue = scp.GetFunctionIntValue(); }
                scp.DeleteCall();
                scp.SetFunctionIntValue(returnedValue);
            }

            //uruchomienie głównej funkcji programu "go"
            else
            {
                scp.AddCall();
                //funkcja go nie powinna mieć argumentów
                if(this.Arguments.Count != 0) 
                    throw new InterpreterException("Error: FunctionDeclaration.Execute Function <go> should have no arguments");
                foreach (IInstruction i in this.Instructions) i.Execute(scp, Functions);
                scp.DeleteCall();
            }

        }

        public List<string> GetArgumentsType()
        {
            List<string> argumentTypes = new List<string>();
            foreach (VarDeclaration varDeclared in this.Arguments)
            {
                string type = ""; //typ jaki chcemy dostać, czy int czy string
                if (varDeclared is Value || varDeclared is StringVarDeclaration) type = "string";
                if(varDeclared is TurtleVarDeclaration) type = "turtle";
                if (varDeclared is IntVarDeclaration || varDeclared is IntegerValue) type = "int";
                argumentTypes.Add(type);
            }

            return argumentTypes;
        }
    }
}