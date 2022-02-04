using System;

namespace Financier.Common.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HeaderAttribute : System.Attribute
    {
        public string Header { get; set; }

        public HeaderAttribute(string header) => Header = header;
    }
}