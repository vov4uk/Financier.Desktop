using Financier.Reports.Common;
using System;
using System.Collections.Generic;

namespace Financier.Reports.Forms
{
    public class ReportNode
    {
        public string Name { get; private set; }

        public string Type { get; private set; }

        public List<ReportNode> Child { get; set; }

        private ReportNode(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public ReportNode(string name)
          : this(name, string.Empty)
        {
        }

        public ReportNode(Type type)
        {
            Type = type.ToString();
            Name = ((HeaderAttribute)Attribute.GetCustomAttribute(type, typeof(HeaderAttribute))).Header;
        }
    }
}