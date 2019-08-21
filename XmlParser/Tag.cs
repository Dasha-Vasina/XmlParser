using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace XmlParser
{
    class Tag : ITagContent
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public ITagContent Subcontent { get; set; }

        public string Data { get; set; }

        public static void Parse(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            text = text.Trim();

            text = NormalizeText(text);

            using (StreamWriter sw = new StreamWriter("out.txt"))
            {
                sw.WriteLine(text);
            }


            var processedTags = new Stack<Tag>();
            while (text.Length > 0)
            {
                // Область текста, котору обрабатываем
                string textScope;

                const string tagPattern = @"^\<(\s*|\/)?(\w[\w\W]+?)(\s.*?)?\>";
                // 1 группа - закрывающая черта
                const int FULL_TEXT_GROUP = 0;
                // 2 группа - название тега
                const int CLOSING_SLASH_GROUP = 1;
                // 3 группа - атрибуты
                const int TAG_NAME_GROUP = 2;

                var tagRegexp = new Regex(tagPattern);

                var tagMatch = tagRegexp.Match(text);

                if (tagMatch.Success)
                {
                    // В начале стоит тег

                    var tagName = tagMatch.Groups[TAG_NAME_GROUP].Value;
                    textScope = tagMatch.Groups[FULL_TEXT_GROUP].Value;
                    var closingTag = !string.IsNullOrWhiteSpace(tagMatch.Groups[CLOSING_SLASH_GROUP].Value);
                    if (closingTag)
                    {
                        var lastTag = processedTags.Peek();
                        if (lastTag.Name == tagName)
                        {
                            processedTags.Pop();

                            //TODO: заносить в структуру документа
                        }
                        else
                        {
                            throw new Exception("Неправильная структкра XML");
                        }
                    }
                    else
                    {
                        //TODO: Обработка атрибутов

                        var tag = new Tag() { Name = tagName };

                        //TODO: Тут возможна перезапись - нужно реализовывать объект
                        if (processedTags.Count > 0)
                        {
                            var lastTag = processedTags.Peek();
                            lastTag.Subcontent = tag;
                        }

                        processedTags.Push(tag);
                    }
                }
                else
                {
                    // В начале стоит контент
                    var data = ExtractData(text);
                    var lastTag = processedTags.Peek();
                    lastTag.Data = data;
                    textScope = data;
                }

                text = text.Substring(textScope.Length);
            }
        }

        private static string ExtractData(string text)
        {
            const string dataPattern = @"^(.*?)\<\/";
            var dataExtractRegexp = new Regex(dataPattern);
            var dataMatch = dataExtractRegexp.Match(text);
            var data = dataMatch.Groups[1].Value;
            return data;
        }

        /// <summary>
        /// Формирует одну строку из многострочного текста, удаляя лишние пробелы, переводы строки и т.д.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>Находит свободное пространство между концов тегов и очищает его</remarks>
        private static string NormalizeText(string text)
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
