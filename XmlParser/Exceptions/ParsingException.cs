using System;

namespace XmlParser.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string message, int position, Exception innerException = null) : base(message, innerException)
        {
            Position = position;
        }

        public int Position { get; set; }
    }
}
