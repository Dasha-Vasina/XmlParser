using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using XmlParser.Extensions;

namespace XmlParser
{
    public class Document
    {
        public static Document Parse(string text)
        {
            var versionRegex = new Regex(@"<\?xml.*\?>");
            var res = versionRegex.Match(text);
            //TODO: Извлечь атрибуты
            text = versionRegex.Replace(text, "").ToLine();

            var docRegexp = new Regex(@"^(<\w+>)(.*)(<\/\w+>)");
            var result = docRegexp.Matches(text);
            if (result.Count > 1)
            {
                throw new Exception("В документе не должно присутствовать больше одного корневого элемента");
            }

            Tag.Parse(text);

            return new Document();
        }
    }
}
