using System;
using System.IO;
using System.Text;

namespace tkom.LexerN
{
    public class LexerTest
    {
        public void LexerTestString(string s)
        {
            using (var stream = new StringReader(s))
            {
                Lexer leks = new Lexer(stream);
                do
                {
                    try
                    {
                        leks.nextToken();
                    }
                    catch (LexicalException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    Console.WriteLine("Token type: " + leks.Token.Type + " value: " + leks.Token.Value + " line: " + leks.Token.position.Line + " col: " + leks.Token.position.Column);

                } while (leks.Token.Type != TokenType.EOT);
            }

        }

        public void LexerTestFile(string path)
        {
            if (!File.Exists(path))
                throw new SourceException("Exception: There is no file with path: " + path);
            using (var stream = new StreamReader(path))
            {
                Lexer leks = new Lexer(stream);
                do
                {
                    try
                    {
                        leks.nextToken();
                    }
                    catch (LexicalException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    Console.WriteLine("Token type: " + leks.Token.Type + " value: " + leks.Token.Value + " line: " + leks.Token.position.Line + " col: " + leks.Token.position.Column);

                } while (leks.Token.Type != TokenType.EOT);
            }
        }

    }
}