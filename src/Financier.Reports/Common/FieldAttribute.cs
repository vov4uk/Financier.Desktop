using System;

namespace fcrd
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldAttribute(string fieldName) => this.Name = fieldName;
    }
}