using System;

namespace XmlParser.Exceptions
{
    public class TagParseExceprion : ParsingException
    {
        public TagParseExceprion(string message, int position) : base(message, position)
        {
        }
    }
}
