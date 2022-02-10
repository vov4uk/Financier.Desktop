using System;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Attribute
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class)]
    public class HeaderAttribute : System.Attribute
    {
        public string Header { get; set; }

        public HeaderAttribute(string header) => Header = header;
    }
}