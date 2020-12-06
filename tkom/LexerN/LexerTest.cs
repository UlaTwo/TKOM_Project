using System;
using System.IO;
using System.Text;

namespace tkom.LexerN
{
    public class LexerTest
    {
        public void LexerTestString(string s)
        {
            ISource source = new StringSource(s);
            Lexer leks = new Lexer(source);
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
                Console.WriteLine("Token type: " + leks.Token.Type + " value: " + leks.Token.Value + " line: " + leks.Token.Line + " col: " + leks.Token.Column);

            } while (leks.Token.Type != TokenType.EOT);

        }

        public void LexerTestFile(string path)
        {

            string path_new = path.Insert(path.Length - 4, "_testLexer");
            try
            {
                if (File.Exists(path_new))
                {
                    File.Delete(path_new);
                }

                // Create a new file     
                using (FileStream fs = File.Create(path_new))
                {
                    ISource source = new FileSource(path);

                    Lexer leks = new Lexer(source);
                    do
                    {

                        leks.nextToken();
                        Byte[] text = new UTF8Encoding(true).GetBytes("Token type: " + leks.Token.Type + "\nvalue: " + leks.Token.Value + "\nline: " + leks.Token.Line + "\ncol: " + leks.Token.Column + "\n\n");
                        fs.Write(text, 0, text.Length);
                    } while (leks.Token.Type != TokenType.EOT);
                }
            }
            catch (LexicalException e)
            {
                using (FileStream fs = new FileStream(path_new, FileMode.Open) )
                {   long endPoint=fs.Length;
                    fs.Seek(endPoint, SeekOrigin.Begin);
                    Byte[] text = new UTF8Encoding(true).GetBytes(e.Message);
                    fs.Write(text, 0, text.Length);
                }
                Console.WriteLine(e.Message);
                return;
            }
            catch (SourceException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}