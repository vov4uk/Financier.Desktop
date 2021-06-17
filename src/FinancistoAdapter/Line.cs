namespace FinancistoAdapter
{
    public class Line
    {
        public Line(string rawLine)
        {
            if (!string.IsNullOrEmpty(rawLine))
            {
                string[] splitted = rawLine.Split(new[] { ':' }, 2);
                Key = splitted[0];
                if (splitted.Length > 1)
                    Value = splitted[1];
            }
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}