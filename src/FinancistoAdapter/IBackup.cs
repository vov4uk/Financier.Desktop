using System;

namespace FinancistoAdapter
{
    public interface IBackup
    {
        string Package { get; }
        int VersionCode { get; }
        Version Version { get; }
        int DatabaseVersion { get; }
    }
}
