using System;
using System.IO;
using tkom.LexerN;
using tkom.ParserN.Structures;
using tkom.ParserN;
using System.Drawing;

namespace tkom.InterpreterN
{
    public class InterpreterRun
    {
        public void InterpreterRunStart(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new SourceException("Exception: There is no file with path: " + path);
                using (var stream = new StreamReader(path))
                {
                    try
                    {
                        Lexer leks = new Lexer(stream);
                        Parser parser = new Parser(leks);
                        ProgramStructure program = new ProgramStructure();
                        program = parser.ParseProgram();

                        createFile();
                        Interpreter interpreter = new Interpreter(program);
                        interpreter.Run();


                    }
                    catch (LexicalException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    catch (ParserException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    catch (InterpreterException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                }
            }
            catch (SourceException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
        private void createFile()
        {
            Bitmap bmp = new Bitmap(800, 600);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, 0, 0, 800, 600);
            g.Dispose();
            bmp.Save("Image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            bmp.Dispose();
        }
    }
}