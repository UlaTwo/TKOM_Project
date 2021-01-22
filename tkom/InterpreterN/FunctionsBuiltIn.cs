using System;
using System.Collections.Generic;
using tkom.ParserN;
using tkom.ParserN.Structures;
using System.Drawing;
using System.Drawing.Imaging;

namespace tkom.InterpreterN
{
    public class FunctionsBuiltIn
    {
        Dictionary<string, Pen> colorPens;
        List<string> functionsNames;
        Dictionary<string, List<string>> functionsAtributes;
        string fileName = "Image.jpg";
        public FunctionsBuiltIn()
        {
            colorPens = new Dictionary<string, Pen>();
            functionsAtributes = new Dictionary<string, List<string>>();
            ColorPensInitialization();
            functionsNamesInitialize();
            functionsAtributesInitialize();
        }
        private void ColorPensInitialization()
        {
            colorPens["black"] = new Pen(Color.Black, 3);
            colorPens["yellow"] = new Pen(Color.Yellow, 3);
            colorPens["red"] = new Pen(Color.Red, 3);
            colorPens["blue"] = new Pen(Color.Blue, 3);
            colorPens["orange"] = new Pen(Color.Orange, 3);
            colorPens["green"] = new Pen(Color.Green, 3);
        }
        private void functionsNamesInitialize()
        {
            functionsNames = new List<string> { "printS", "printI", "return", "create",
            "printCoordinates", "ellipse", "circle", "forward", "rectangle", "triangle", "square", "change" };
        }

        private void functionsAtributesInitialize()
        {
            functionsAtributes["printS"] = new List<string> { "string" };
            functionsAtributes["printI"] = new List<string> { "int" };
            functionsAtributes["return"] = new List<string> { "int" };
            functionsAtributes["create"] = new List<string> { "int", "int" };
            functionsAtributes["printCoordinates"] = new List<string>();
            functionsAtributes["ellipse"] = new List<string>() { "int", "int", "string" };
            functionsAtributes["circle"] = new List<string>() { "int", "string" };
            functionsAtributes["forward"] = new List<string>() { "int", "int", "string" };
            functionsAtributes["rectangle"] = new List<string>() { "int", "int", "string" };
            functionsAtributes["triangle"] = new List<string>() { "int", "int", "int", "int", "string" };
            functionsAtributes["square"] = new List<string>() { "int", "string" };
            functionsAtributes["change"] = new List<string>() { "int", "int" };
        }

        public bool runFunction(Scope scp, string name)
        {
            switch (name)
            {
                case "printS":
                    printS(scp);
                    return true;
                case "printI":
                    printI(scp);
                    return true;
                case "return":
                    returnFun(scp);
                    return true;
                case "create":
                    create(scp);
                    return true;
                case "printCoordinates":
                    printCoordinates(scp);
                    return true;
                case "ellipse":
                    ellipse(scp);
                    return true;
                case "circle":
                    circle(scp);
                    return true;
                case "forward":
                    forward(scp);
                    return true;
                case "rectangle":
                    rectangle(scp);
                    return true;
                case "triangle":
                    triangle(scp);
                    return true;
                case "square":
                    square(scp);
                    return true;
                case "change":
                    change(scp);
                    return true;
                default:
                    return false;

            }
        }

        public bool checkFunction(string name)
        {
            return functionsNames.Contains(name);
        }

        public List<string> ArgsTypesFunction(string name)
        {
            List<string> var = new List<string>();
            if (functionsAtributes.TryGetValue(name, out var))
                return var;
            else
                throw new InterpreterException(" Error:  FunctionsBuiltIn.ArgsTypesFunction() trying to get arguments of function, that not exists named: " + name);
        }

        private Pen getPenFromColorPens(string name)
        {
            Pen var = new Pen(Color.White);
            if (colorPens.TryGetValue(name, out var))
                return var;
            else
                throw new InterpreterException(" Error:  FunctionsBuiltIn.getPenFromColorPens() trying to get color pen, that not exists named: " + name);
        }

        private void printS(Scope scp)
        {
            checkNumberOfArguments(scp, 1, "printS()");
            Value val = getStringFromArguments(scp, 0);
            Console.WriteLine(val.Identifier);
        }

        private void printI(Scope scp)
        {
            checkNumberOfArguments(scp, 1, "printI()");
            IntegerValue val = getIntegerFromArguments(scp, 0);
            Console.WriteLine(val.value);
        }

        private void returnFun(Scope scp)
        {
            checkNumberOfArguments(scp, 1, "return()");
            IntegerValue val = getIntegerFromArguments(scp, 0);
            scp.SetFunctionIntValue(val.value);
        }

        private void create(Scope scp)
        {
            checkNumberOfArguments(scp, 2, "create()");
            IntegerValue x = getIntegerFromArguments(scp, 0);
            IntegerValue y = getIntegerFromArguments(scp, 1);
            //teraz trzeba dodać żółwia
            TurtleCoordinates tCor = new TurtleCoordinates(x.value, y.value);
            string turtleName = scp.GetUsedTurtle();
            if (turtleName == "") throw new InterpreterException("Error: FunctionsBuiltIn.create() Trying to use turtle function not in turtle funtion block.");
            scp.AddTurtle(turtleName, tCor);
        }

        private void printCoordinates(Scope scp)
        {
            checkNumberOfArguments(scp, 0, "printCoordinates()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            Console.WriteLine("Turtle " + turtleName + " coordinates: x -> " + tCor.X + "; y -> " + tCor.Y);
        }

        private void ellipse(Scope scp)
        {
            checkNumberOfArguments(scp, 3, "ellipse()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue width = getIntegerFromArguments(scp, 0);
            IntegerValue height = getIntegerFromArguments(scp, 1);
            if(width.value < 1 || height.value <1 )
                throw new InterpreterException("Error: FunctionsBuiltIn.ellipse() All passed integers should be greater than 0.");
            Value color = getStringFromArguments(scp, 2);
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    Rectangle rect = new Rectangle(tCor.X, tCor.Y - (width.value / 2), width.value, height.value);
                    graphic.DrawEllipse(pen, rect);
                }
                image.Save(fileName);
            }
        }

        private void circle(Scope scp)
        {
            checkNumberOfArguments(scp, 2, "circle()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue radius = getIntegerFromArguments(scp, 0);
            if(radius.value < 1 )
                throw new InterpreterException("Error: FunctionsBuiltIn.circle() All passed integers should be greater than 0.");
            Value color = getStringFromArguments(scp, 1);
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    Rectangle rect = new Rectangle(tCor.X, tCor.Y - (radius.value / 2), radius.value, radius.value);
                    graphic.DrawEllipse(pen, rect);
                }
                image.Save(fileName);
            }
        }

        private void forward(Scope scp)
        {
            checkNumberOfArguments(scp, 3, "forward()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue distance = getIntegerFromArguments(scp, 0);
            IntegerValue angle = getIntegerFromArguments(scp, 1);
            if (!(angle.value >= 0 && angle.value < 360))
                throw new InterpreterException("Error:  FunctionsBuiltIn.forward() The angle passed to function forward() is not correct.");
            if(distance.value < 1 )
                throw new InterpreterException("Error: FunctionsBuiltIn.forward() Distance argument should be greater than 0.");
            Value color = getStringFromArguments(scp, 2);
            //policzenie nowej współrzędnej
            double angleInRadians = (Math.PI / 180) * angle.value;
            double angleSin = Math.Sin(angleInRadians);
            double angleCos = Math.Cos(angleInRadians);
            int y = (int)(distance.value * angleSin) + tCor.Y;
            int x = (int)(distance.value * angleCos) + tCor.X;
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    graphic.DrawLine(pen, tCor.X, tCor.Y, x, y);
                }
                image.Save(fileName);
            }
            //ustawienie współrzędnych
            tCor.X = x;
            tCor.Y = y;
            scp.UpdateTurtle(turtleName, tCor);
        }

        private void rectangle(Scope scp)
        {
            checkNumberOfArguments(scp, 3, "rectangle()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue width = getIntegerFromArguments(scp, 0);
            IntegerValue height = getIntegerFromArguments(scp, 1);
            if(width.value < 1 || height.value <1 )
                throw new InterpreterException("Error: FunctionsBuiltIn.rectangle() All passed integers should be greater than 0.");
            Value color = getStringFromArguments(scp, 2);
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    graphic.DrawRectangle(pen, tCor.X, tCor.Y, width.value, height.value);
                }
                image.Save(fileName);
            }
        }

        private void triangle(Scope scp)
        {
            checkNumberOfArguments(scp, 5, "triangle()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue x1 = getIntegerFromArguments(scp, 0);
            IntegerValue y1 = getIntegerFromArguments(scp, 1);
            IntegerValue x2 = getIntegerFromArguments(scp, 2);
            IntegerValue y2 = getIntegerFromArguments(scp, 3);
            Value color = getStringFromArguments(scp, 4);
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    Point point1 = new Point(tCor.X, tCor.Y);
                    Point point2 = new Point(x1.value, y1.value);
                    Point point3 = new Point(x2.value, y2.value);
                    Point[] curvePoints = { point1, point2, point3 };
                    // Draw polygon to screen.
                    graphic.DrawPolygon(pen, curvePoints);
                }
                image.Save(fileName);
            }
        }

        private void square(Scope scp)
        {
            checkNumberOfArguments(scp, 2, "square()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue side = getIntegerFromArguments(scp, 0);
            if(side.value < 1 )
                throw new InterpreterException("Error: FunctionsBuiltIn.square() All passed integers should be greater than 0.");
            Value color = getStringFromArguments(scp, 1);
            using (Image image = Image.FromFile(fileName))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Pen pen = getPenFromColorPens(color.Identifier);
                    graphic.DrawRectangle(pen, tCor.X, tCor.Y, side.value, side.value);
                }
                image.Save(fileName);
            }
        }

        private void change(Scope scp)
        {
            checkNumberOfArguments(scp, 2, "change()");
            string turtleName = getAndCheckTurtle(scp);
            TurtleCoordinates tCor = scp.GetTurtleCoordinates(turtleName);
            IntegerValue x = getIntegerFromArguments(scp, 0);
            IntegerValue y = getIntegerFromArguments(scp, 1);
            //ustawienie współrzędnych
            tCor.X = x.value;
            tCor.Y = y.value;
            scp.UpdateTurtle(turtleName, tCor);

        }

        private string getAndCheckTurtle(Scope scp)
        {
            string turtleName = scp.GetUsedTurtle();
            if (turtleName == "")
                throw new InterpreterException("Error: FunctionsBuiltIn.getAndCheckTurtle() Trying to use turtle function not in turtle funtion block.");
            if (!scp.checkIfTurtleHasCoordinates(turtleName))
                throw new InterpreterException("Error: FunctionsBuiltIn.getAndCheckTurtle() Trying to use function for turtle <" + turtleName + "> that doesn't have daclared coordinates.");
            return turtleName;
        }
        private IntegerValue getIntegerFromArguments(Scope scp, int i)
        {
            VarDeclaration varI = scp.GetFunCallVar(i);
            if (varI is IntegerValue)
            {
                IntegerValue intVal = (IntegerValue)varI;
                return intVal;
            }
            else
            {
                VarDeclaration varIS = scp.GetVar(varI.Identifier);
                if (varIS is IntegerValue)
                {
                    IntegerValue intVal = (IntegerValue)varIS;
                    return intVal;
                }
                throw new InterpreterException("Error: FunctionsBuiltIn.getIntegerFromArguments() The variable <"+varI.Identifier+"> doesn't have declared value in scope.");
            }
        }
        private Value getStringFromArguments(Scope scp, int i)
        {
            VarDeclaration var = scp.GetFunCallVar(i);
            if (scp.CheckIfVarExists(var.Identifier))
            {
                VarDeclaration varS = scp.GetVar(var.Identifier);
                if (var is Value) return (Value)var;
            }
            else
            {
                if (var is Value) return (Value)var;
            }
            throw new InterpreterException("Error: FunctionsBuiltIn.getStringFromArguments() The variable <"+var.Identifier+"> doesn't have declared value in scope.");
        }

        private void checkNumberOfArguments(Scope scp, int i, string name)
        {
            int numberArguments = scp.GetNumberOfArgumentsInFunCall();
            if (numberArguments != i)
            {
                throw new InterpreterException("Error: FunctionsBuiltIn." + name + " The number of passed arguments schould be "+i+". Instead it is: " + numberArguments);
            }
        }
    }
}
