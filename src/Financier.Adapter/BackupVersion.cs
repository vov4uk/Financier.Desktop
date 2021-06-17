using System;

namespace Financier.Adapter
{
    public class BackupVersion
    {
        public string Package { get; set; }
        public int VersionCode { get; set; }
        public Version Version { get; set; }
        public int DatabaseVersion { get; set; }
    }
}
