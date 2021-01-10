using System;
using tkom.LexerN;
using tkom.ParserN;
using System.IO;
using System.Text;

namespace tkom
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length==2){
                LexerTest ltest = new LexerTest();
                ParserTest ptest = new ParserTest();
                switch(args[0]){
                case "-tlf": ltest.LexerTestFile(args[1]); break;
                case "-tls": ltest.LexerTestString(args[1]); break;
                case "-tpf": ptest.ParserTestFile(args[1]); break;
                default: Console.WriteLine("Podano niepoprawną falgę. "); break;
                }

            }
            else{
                Console.WriteLine("Podano niepoprawne argumenty. ");
            }

        }
        
    }
}

