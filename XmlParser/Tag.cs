using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XmlParser.Exceptions;

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
            int position = 0;

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
                                throw new TagParseExceprion("Открывающий и закрывающий теги имеют одинаковое имя в разном регистре", position);
                            }
                            throw new TagParseExceprion("Неправильная структкра XML", position);
                        }
                    }
                    else
                    {
                        var tag = new Tag() { Name = tagName };

                        if (tagName.ToUpper().StartsWith("XML"))
                        {
                            throw new TagParseExceprion("Имя тега не должно начинаться с XML", position);
                        }

                        var atributesString = tagMatch.Groups[TAG_ATRIBUTES_GROUP].Value;

                        try
                        {
                            AddAtributes(tag, atributesString);
                        }
                        catch (AttributeParseException e)
                        {
                            throw new ParsingException("Ошибка при разборе документа.", position, e);
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
                position += textScope.Length;
            }
        }

        private static void AddAtributes(Tag tag, string atributesString)
        {
            var atributesRegexp = new Regex(@"(\w+)\s*=\s*[""'](.*?)[""']");

            var rawAtributesCollection = atributesRegexp.Matches(atributesString);

            foreach (Match atributeText in rawAtributesCollection)
            {
                Atribute attribute;
                try
                {
                    attribute = Atribute.Parse(atributeText.Value);
                }
                catch (Exception e)
                {
                    throw new AttributeParseException(e.Message, tag);
                }

                atributesString = atributesString.Replace(atributeText.Value, "").Trim();

                var sameStoredAttribute = tag.Atributes.FirstOrDefault(a => a.Name == attribute.Name);

                if (sameStoredAttribute != null)
                {
                    throw new AttributeParseException("Тег уже имеет атрибут с таким же названием", tag);
                }
                tag.Atributes.Add(attribute);
            }

            if (atributesString != "")
            {
                throw new AttributeParseException("Тег имеет непраильную структуту атрибутов", tag);
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
