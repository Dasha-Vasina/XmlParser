using System;
using System.IO;

namespace XmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (StreamReader sr = new StreamReader("ex.txt"))
            {
                var text = sr.ReadToEnd();
                Tag.Parse(text);

            }
        }
    }
}
