using Financier.Common.Attribute;
using System;
using System.Collections.Generic;

namespace Financier.Common.Model
{
    public class TreeNode
    {
        public string Name { get; private set; }

        public string Type { get; private set; }

        public List<TreeNode> Child { get; set; }

        private TreeNode(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public TreeNode(string name)
          : this(name, string.Empty)
        {
        }

        public TreeNode(Type type)
        {
            Type = type.ToString();
            Name = ((HeaderAttribute)System.Attribute.GetCustomAttribute(type, typeof(HeaderAttribute))).Header;
        }
    }
}
