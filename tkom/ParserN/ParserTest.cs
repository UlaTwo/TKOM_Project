using System;
using System.IO;
using tkom.LexerN;
using tkom.ParserN.Structures;

namespace tkom.ParserN
{
    public class ParserTest
    {
        public void ParserTestFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new SourceException("Exception: There is no file with path: " + path);
                using (var stream = new StreamReader(path))
                {
                    Lexer leks = new Lexer(stream);
                    Parser parser = new Parser(leks);
                    ProgramStructure program = new ProgramStructure();
                    try
                    {
                        program = parser.ParseProgram();
                    }
                    catch (ParserException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    Console.WriteLine("Przeprasowano! :D ");
                    Console.WriteLine("Oto program: ");
                    Console.WriteLine("| "); Console.WriteLine("| ");
                    program.ConsoleWrite();
                }
            }
            catch (SourceException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}