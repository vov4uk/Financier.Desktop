namespace Financier.Adapter
{
    public readonly struct Line
    {
        public Line(string rawLine)
        {
            if (!string.IsNullOrEmpty(rawLine))
            {
                int idx = rawLine.IndexOf(':');
                if (idx >= 0)
                {
                    Key = rawLine[..idx];
                    Value = rawLine[(idx + 1)..];
                }
                else
                {
                    Key = rawLine;
                }
            }
        }

        public string Key { get; }
        public string Value { get; }
    }
}
