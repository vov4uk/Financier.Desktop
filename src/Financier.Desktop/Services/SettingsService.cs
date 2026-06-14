using System.Text.Json.Serialization;
using Cogwheel;
using Financier.Common.Localization;

namespace Financier.Desktop.Services
{
    public partial class SettingsService()
        : SettingsBase(StartOptions.Current.SettingsPath, SerializerContext.Default)
    {
        public static SettingsService Current { get; } = new();

        public Language Language { get; set; }

        public bool IsAutoUpdateEnabled { get; set; } = true;

        public string DefaultBackupDir { get; set; }

        public string AppSettings { get; set; }
    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private partial class SerializerContext : JsonSerializerContext;
    }
}
