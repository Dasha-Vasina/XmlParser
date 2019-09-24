using System;
using System.Collections.Generic;
using System.Text;

namespace XmlParser.Exceptions
{
    class AttributeParseException : Exception
    {
        public AttributeParseException(string message, Tag tag) : base(message)
        {
            Tag = tag;
        }

        public Tag Tag { get; set; }
    }
}
