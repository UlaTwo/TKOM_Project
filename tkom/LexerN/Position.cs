using System.Collections.Generic;
using System;

namespace tkom.LexerN
{
    public class Position
    {
        public int Line { set; get; }
        public int Column { set; get; }

        public void setLineColumn(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public Position(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public void IncrementNewLine()
        {
            this.Line += 1;
            this.Column = 0;
        }

        public void IncrementNextInLine()
        {
            this.Column += 1;
        }

    }
}