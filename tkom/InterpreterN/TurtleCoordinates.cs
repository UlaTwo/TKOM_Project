using System;
using System.Collections.Generic;
using tkom.ParserN;
using tkom.ParserN.Structures;

namespace tkom.InterpreterN
{
    public class TurtleCoordinates
    {
        public int X;
        public int Y;
        public TurtleCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void setCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}