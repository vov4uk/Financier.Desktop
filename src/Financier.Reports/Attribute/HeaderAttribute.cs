using System;

namespace Financier.Reports.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HeaderAttribute : Attribute
    {
        public string Header { get; set; }

        public HeaderAttribute(string header) => Header = header;
    }
}