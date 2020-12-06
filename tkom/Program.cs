using System;
using tkom.LexerN;
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
                switch(args[0]){
                case "-tlf": ltest.LexerTestFile(args[1]); break;
                case "-tls": ltest.LexerTestString(args[1]); break;
                default: Console.WriteLine("Podano niepoprawną falgę. "); break;
                }

            }
            else{
                Console.WriteLine("Podano niepoprawne argumenty. ");
            }

        }
        
    }
}

