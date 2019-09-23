using System;
using System.Text.RegularExpressions;

namespace XmlParser
{
    internal class Atribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        internal static Atribute Parse(string text)
        {
            //var atributesRegexp = new Regex(@"(\w+)\s*=\s*[""'](.*?)[""']");
            var atributesRegexp = new Regex(@"(?<name>\w+)\s*=\s*(?<startBound>[""'])(?<value>.*?)(?<endBound>[""'])");
            var matchResult = atributesRegexp.Match(text);
            if (!matchResult.Success)
            {
                throw new Exception("Ошибка парсинга атрибута");
            }

            #region Name processing

            var name = matchResult.Groups["name"].Value;
            var nameRegexp = new Regex(@"^\s*([а-яА-Яa-zA-Z_][\w.\-_]*)\s*$");
            var nameMatch = nameRegexp.Match(name);
            if (!nameMatch.Success)
            {
                throw new Exception("Имя атрибута не соответствует правилам");
            }

            #endregion

            #region Value bounds processing

            var startBound = matchResult.Groups["startBound"].Value;
            var endBound = matchResult.Groups["endBound"].Value;
            if (startBound != endBound)
            {
                throw new Exception("Значение атрибута должно быть внутри одинаковых двойных или одинарных ковычек");
            }

            #endregion

            #region Value processing

            var value = matchResult.Groups["value"].Value;
            if (value.Contains('<') || value.Contains('&') || value.Contains(startBound))
            {
                throw new Exception("Значение атрибута не должно содержать символы <, & или обрамляющих ковычек");
            }

            #endregion

            return new Atribute { Name = nameMatch.Groups[1].Value, Value = value };
        }
    }
}