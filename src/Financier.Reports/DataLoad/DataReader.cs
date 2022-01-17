using System.Collections.Generic;
using System.IO;

namespace Financier.Reports.DataLoad
{
    public class DataReader
    {
        public const string StartEntity = "$ENTITY:";
        public const string EndEntity = "$$";
        public const string Separator = ":";

        public void Start(string fileName)
        {
            string type = null;
            Dictionary<string, string> items = new Dictionary<string, string>();
            StreamReader streamReader = new StreamReader(fileName);
            string str;
            while ((str = streamReader.ReadLine()) != null)
            {
                if (str.StartsWith("$ENTITY:"))
                {
                    type = str.Substring("$ENTITY:".Length);
                    items = new Dictionary<string, string>();
                }
                else if (str.StartsWith("$$"))
                    RaiseEntityRead(type, items);
                else if (!string.IsNullOrEmpty(type) && str.Contains(":"))
                    items.Add(str.Substring(0, str.LastIndexOf(":")), str.Substring(str.LastIndexOf(":") + 1));
            }
            streamReader.Close();
        }

        public event EntityReadDelegate OnEntityRead;

        private void RaiseEntityRead(string type, Dictionary<string, string> items)
        {
            if (OnEntityRead == null)
                return;
            OnEntityRead(type, items);
        }

        public delegate void EntityReadDelegate(string type, Dictionary<string, string> items);
    }
}