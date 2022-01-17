using System;

namespace fcrd
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HeaderAttribute : Attribute
    {
        public string Header { get; set; }

        public HeaderAttribute(string header) => this.Header = header;
    }
}