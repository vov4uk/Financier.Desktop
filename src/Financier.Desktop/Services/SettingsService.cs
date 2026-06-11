using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Financier.Desktop.Localization;

namespace Financier.Desktop.Services
{
    [ObservableObject]
    public partial class SettingsService()
        : SettingsBase(StartOptions.Current.SettingsPath, SerializerContext.Default)
    {

        [ObservableProperty]
        public partial Language Language { get; set; }

        [ObservableProperty]
        public partial bool IsAutoUpdateEnabled { get; set; } = true;

    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private partial class SerializerContext : JsonSerializerContext;
    }
}
