using System.Collections.Generic;
using System;
using System.IO;

namespace tkom.LexerN
{
    public class Source
    {
        public char character { get; set; }
        public Position position { get; set; }
        public TextReader reader;
        public Source(TextReader source)
        {
            character = ' ';
            position = new Position(1, 0);
            reader = source;
        }
        public void Read()
        {
            int znak = reader.Read();

            if (znak == -1)
            {
                character = '\0';
            }
            else
            {
                character = Convert.ToChar(znak);

                if (character == (Int32)'\n')
                {
                    position.IncrementNewLine();
                }
                else
                {
                    position.IncrementNextInLine();
                }
            }
        }
    }
    class SourceException : Exception
    {
        public SourceException() { }
        public SourceException(string message) : base(message) { }
        public SourceException(string message, Exception inner) : base(message, inner) { }
    }
}
