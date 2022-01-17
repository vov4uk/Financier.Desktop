using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace fcrd
{
    public class ReportNode
    {
        public string Name { get; private set; }

        public string Type { get; private set; }

        public ObservableCollection<ReportNode> Child { get; set; }

        private ReportNode(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        public ReportNode(string name)
          : this(name, string.Empty)
        {
        }

        public ReportNode(System.Type type)
        {
            this.Type = type.ToString();
            this.Name = ((HeaderAttribute)Attribute.GetCustomAttribute((MemberInfo)type, typeof(HeaderAttribute))).Header;
        }
    }
}