using System;

namespace Financier.Common.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CellTemplateAttribute : System.Attribute
    {
        public string Key { get; set; }

        public CellTemplateAttribute(string header) => Key = header;
    }
}
