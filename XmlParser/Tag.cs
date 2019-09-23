using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XmlParser
{
    class Tag : ITagContent
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public ITagContent Subcontent { get; set; }

        public List<Atribute> Atributes { get; set; }

        public string Data { get; set; }

        public Tag()
        {
            Atributes = new List<Atribute>();
        }

        public static void Parse(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            text = text.Trim();

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
                // 0 группа - полный текст
                const int FULL_TEXT_GROUP = 0;
                // 1 группа - закрывающая черта
                const int CLOSING_SLASH_GROUP = 1;
                // 2 группа - название тега
                const int TAG_NAME_GROUP = 2;
                // 3 группа - атрибуты
                const int TAG_ATRIBUTES_GROUP = 3;

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
                            if (lastTag.Name.ToUpper() == tagName.ToUpper())
                            {
                                throw new Exception("Открывающий и закрывающий теги имеют одинаковое имя в разном регистре");
                            }
                            throw new Exception("Неправильная структкра XML");
                        }
                    }
                    else
                    {
                        if (tagName.ToUpper().StartsWith("XML"))
                        {
                            throw new Exception("Имя тега не должно начинаться с XML");
                        }

                        var tag = new Tag() { Name = tagName };

                        var atributesRegexp = new Regex(@"(\w+)\s*=\s*[""'](.*?)[""']");

                        var atributesMatch = tagMatch.Groups[TAG_ATRIBUTES_GROUP].Value;

                        var rawAtributesCollection = atributesRegexp.Matches(atributesMatch);

                        foreach (Match atributeText in rawAtributesCollection)
                        {
                            var atribute = Atribute.Parse(atributeText.Value);

                            var sameStoredAttribute = tag.Atributes.FirstOrDefault(a => a.Name == atribute.Name);

                            if (sameStoredAttribute != null)
                            {
                                throw new Exception("Тег уже имеет атрибут с таким же названием");
                            }
                            tag.Atributes.Add(atribute);
                        }


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
    }
}
