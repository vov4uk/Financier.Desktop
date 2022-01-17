using System;

namespace Financier.Reports.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldAttribute(string fieldName) => Name = fieldName;
    }
}