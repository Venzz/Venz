using System;
using Windows.Data.Xml.Dom;

namespace Venz.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static XmlElement FindFirst(this XmlDocument source, String elementName)
        {
            var xmlNodes = source.GetElementsByTagName(elementName);
            return (xmlNodes.Count == 0) ? null : (xmlNodes[0] as XmlElement);
        }

        public static XmlElement FindFirst(this XmlDocument source, String elementName, XmlAttribute attribute)
        {
            foreach (var xmlNode in source.GetElementsByTagName(elementName))
                if ((xmlNode is XmlElement) && (((XmlElement)xmlNode).GetAttribute(attribute.Name) == attribute.Value))
                    return (XmlElement)xmlNode;
            return null;
        }
    }
}
