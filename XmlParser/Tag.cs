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
                var tagRegexp = new Regex(@"^\<\s*(\w+)(\s+.*)?\>");

                var tagMatch = tagRegexp.Match(text);

                if (tagMatch.Success)
                {
                    var tagName = tagMatch.Groups[1].Value;

                    var tag = new Tag() { Name = tagName };

                    //TODO: Тут возможна перезапись - нужно реализовывать объект
                    if (processedTags.Count>0)
                    {
                        var lastTag = processedTags.Peek();
                        lastTag.Subcontent = tag;
                    }
                    processedTags.Push(tag);
                    text = text.Substring(tagMatch.Groups[0].Value.Length);
                }
                else
                {

                    var closedTagRegexp = new Regex(@"^\<\/([\w\W]*?)\>");

                    //Если true, то в начале закрывающий тег и надо достать его из стека
                    //иначе в начале текст и его надо занести в последний тег
                    if (closedTagRegexp.IsMatch(text))
                    {
                        // Скопировано выше = оптимизировать

                        var tagNameMatch = closedTagRegexp.Match(text);

                        if (tagNameMatch.Success)
                        {
                            var tagName = tagNameMatch.Groups[1].Value;
                            var lastTag = processedTags.Peek();
                            if (lastTag.Name == tagName)
                            {
                                processedTags.Pop();
                                text = text.Substring(tagNameMatch.Groups[0].Value.Length);
                            }
                            else
                            {
                                throw new Exception("Неправильная структкра XML");
                            }
                        }
                    }
                    else
                    {
                        const string dataPattern = @"^(.*?)\<\/";
                        var dataExtractRegexp = new Regex(dataPattern);
                        var dataMatch = dataExtractRegexp.Match(text);
                        var lastTag = processedTags.Peek();

                        var data = dataMatch.Groups[1].Value;
                        lastTag.Data = data;
                        text = text.Substring(data.Length);
                    }

                }


            }

            //Находим название первого тега
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
