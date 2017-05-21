using System;

namespace Venz.Extensions
{
    public class XmlAttribute
    {
        public String Name { get; }
        public String Value { get; }

        public XmlAttribute(String name, String value)
        {
            Name = name;
            Value = value;
        }
    }
}
