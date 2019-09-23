using System;
using System.Text.RegularExpressions;

namespace XmlParser.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Формирует одну строку из многострочного текста, удаляя лишние пробелы, переводы строки и т.д.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>Находит свободное пространство между концов тегов и очищает его</remarks>
        public static string ToLine(this string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            const string tagPattern = @"[\r\n]+\s*\<";
            var regex = new Regex(tagPattern);
            string target = "<";
            string result = regex.Replace(text, target);

            return result;
        }
    }
}
