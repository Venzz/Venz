using System;

namespace SQLite
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute: Attribute
    {
        public String Value { get; private set; }
        public DefaultValueAttribute(String value) { Value = value; }
    }
}
