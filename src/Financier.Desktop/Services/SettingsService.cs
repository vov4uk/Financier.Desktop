using System.Text.Json.Serialization;
using Cogwheel;
using Financier.Common.Localization;

namespace Financier.Desktop.Services
{
    public partial class SettingsService()
        : SettingsBase(StartOptions.Current.SettingsPath, SerializerContext.Default)
    {


        public Language Language { get; set; }


        public bool IsAutoUpdateEnabled { get; set; } = true;

    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private partial class SerializerContext : JsonSerializerContext;
    }
}
