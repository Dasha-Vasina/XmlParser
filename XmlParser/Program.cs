using System;
using System.IO;
using XmlParser.Exceptions;

namespace XmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите путь до xml файла: ");
            var path = Console.ReadLine();
            string text;
            using (var sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }
            try
            {
                Document.Parse(text);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Документ прошел проверку");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (TagParseExceprion e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка парсинга тега");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Ошибка {e.Message}");
                Console.WriteLine($"Позиция {e.Position}");
            }
            catch(ParsingException parsingEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(parsingEx.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Позиция {parsingEx.Position}");
                if (parsingEx.InnerException != null)
                {
                    Console.WriteLine(parsingEx.InnerException.Message);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
