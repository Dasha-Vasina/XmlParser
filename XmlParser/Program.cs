using System;
using System.IO;

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
