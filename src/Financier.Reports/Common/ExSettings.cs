using Financier.Reports.Reports.Properties;
using System;

namespace Financier.Reports.Common
{
    public static class ExSettings
    {
        public static string LastBackupDir
        {
            get => Settings.Default.LastBackupDir;
            set
            {
                Settings.Default.LastBackupDir = value;
                Settings.Default.Save();
            }
        }

        public static string DbPath => AppDomain.CurrentDomain.BaseDirectory + "db\\fcrd.db3";

        public static bool MondayFirstDay
        {
            get => Settings.Default.MondayFirstDay;
            set
            {
                Settings.Default.MondayFirstDay = value;
                Settings.Default.Save();
            }
        }

        public static bool AutoDataLoad
        {
            get => Settings.Default.AutoDataLoad;
            set
            {
                Settings.Default.AutoDataLoad = value;
                Settings.Default.Save();
            }
        }
    }
}