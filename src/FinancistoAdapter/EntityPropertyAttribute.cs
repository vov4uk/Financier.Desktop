using FinancistoAdapter.Converters;
using System;

namespace FinancistoAdapter
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EntityPropertyAttribute : Attribute
    {
        private Type _converter = DefaultConverter;

        public string Key { get; private set; }

        public static Type DefaultConverter
        {
            get { return typeof (DefaultConverter); }
        }

        public Type Converter
        {
            get { return _converter; }
            set
            {
                if (value != null && !typeof(IPropertyConverter).IsAssignableFrom(value))
                    throw new ArgumentException("Converter type must implement IPropertyConverter.", "value");
                _converter = value;
            }
        }

        public EntityPropertyAttribute(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", "key");
            Key = key;
        }
    }
}
