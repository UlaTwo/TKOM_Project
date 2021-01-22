using System;
using tkom.LexerN;
using tkom.ParserN;
using tkom.InterpreterN;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace tkom
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 )
            {
                LexerTest ltest = new LexerTest();
                ParserTest ptest = new ParserTest();
                InterpreterRun irun = new InterpreterRun();
                switch (args[0])
                {
                    case "-tlf": 
                        ltest.LexerTestFile(args[1]); 
                        break;
                    case "-tls": 
                        ltest.LexerTestString(args[1]); 
                        break;
                    case "-tpf": 
                        ptest.ParserTestFile(args[1]); 
                        break;
                    case "-tif": 
                         irun.InterpreterRunStart(args[1]); 
                        break;
                    default: Console.WriteLine("Podano niepoprawną falgę. "); break;
                }
            }
            else
            {
                Console.WriteLine("Podano niepoprawne argumenty. ");
            }

        }

    }
}

