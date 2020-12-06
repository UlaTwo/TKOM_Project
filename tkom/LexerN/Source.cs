using System.Collections.Generic;
using System;
using System.IO;

namespace tkom.LexerN
{

    public interface ISource
    {
        char character { get; }
        bool isEnd { get; }
        int Line { get; }
        int Column { get; }
        void Read();
    }


    public class StringSource : ISource
    {
        private string myString;
        private int position;
        public StringSource(string s)
        {
            Line = 1;
            character = ' ';
            Column = 0;
            myString = s;
            position = 0;
            isEnd = false;
        }
        public char character { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
        public bool isEnd { get; private set; }
        public void Read()
        {
            if (myString.Length <= position)
            {
                character = ' ';
                isEnd = true;
            }
            else
            {
                character = myString[position];
                position++;
            }
            if (character == '\n')
            {
                Line += 1;
                Column = 0;
            }
            else
            {
                Column += 1;
            }
        }
    }

    public class FileSource : ISource
    {
        private StreamReader reader;

        public char character { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
        public bool isEnd { get; private set; }

        public FileSource(string path)
        {
            if (File.Exists(path))
                reader = new StreamReader(path);
            else
                throw new SourceException("Exception: There is no file with path: " + path);
            Line = 1;
            character = ' ';
            Column = 0;
            isEnd = false;
        }


        public void Read()
        {
            if (reader.EndOfStream){
                 character = ' ';
                isEnd = true;
            }
            else character =  Convert.ToChar(reader.Read());

            if (character == '\n')
            {
                Line += 1;
                Column = 0;
            }
            else
            {
                Column += 1;
            }

        }
    }

        class SourceException : Exception
    {
        public SourceException() {}
        public SourceException(string message) : base(message) { }
        public SourceException(string message, Exception inner) : base(message,inner) { }
    }
}
