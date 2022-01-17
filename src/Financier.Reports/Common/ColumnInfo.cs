namespace fcrd
{
    public class ColumnInfo
    {
        public string ColName { get; private set; }

        public string Type { get; private set; }

        public ColumnInfo(string name, string type)
        {
            this.ColName = name;
            this.Type = type;
        }
    }
}