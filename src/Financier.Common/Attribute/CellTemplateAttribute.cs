using System;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Attribute
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class CellTemplateAttribute : System.Attribute
    {
        public string Key { get; set; }

        public CellTemplateAttribute(string header) => Key = header;
    }
}
