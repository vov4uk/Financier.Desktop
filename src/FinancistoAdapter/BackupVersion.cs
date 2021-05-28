using System;

namespace FinancistoAdapter
{
    public class BackupVersion
    {
        public string Package { get; set; }
        public int VersionCode { get; set; }
        public Version Version { get; set; }
        public int DatabaseVersion { get; set; }
    }
}
