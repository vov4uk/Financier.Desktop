using System.Text.Json.Serialization;
using Cogwheel;
using Financier.Desktop.Data;

namespace Financier.Desktop.Services
{
    public partial class SettingsService()
        : SettingsBase(StartOptions.Current.SettingsPath, SerializerContext.Default)
    {
        public static SettingsService Current { get; } = new();

        public SettingsDto Settings { get; set; }

        public string DefaultBackupDir { get; set; }

    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private sealed partial class SerializerContext : JsonSerializerContext;
    }
}
