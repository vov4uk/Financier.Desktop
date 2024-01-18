using System;

namespace Financier.Adapter
{
    public class BackupVersion
    {
        public string Package { get; set; }
        public int VersionCode { get; set; }
        public string Version { get; set; }
        public int DatabaseVersion { get; set; }
    }
}
