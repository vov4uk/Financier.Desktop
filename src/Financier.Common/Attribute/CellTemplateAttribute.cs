namespace Financier.Common.Attribute
{
    public class CellTemplateAttribute : System.Attribute
    {
        public string Key { get; set; }

        public CellTemplateAttribute(string header) => Key = header;
    }
}
