using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace Financier.Common.Localization;

/// <summary>
/// Singleton localization service that resolves translated strings from .resx
/// resource files and notifies WPF bindings when the culture changes.
/// </summary>
/// <remarks>
/// Register via DI as a singleton, or reference via <see cref="Instance"/>.
/// The <c>Item[]</c> property-changed notification refreshes all active indexer
/// bindings without requiring an IValueConverter.
/// </remarks>
public sealed class LocalizationService : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationService> _lazy =
        new(() => new LocalizationService(), isThreadSafe: true);

    private static readonly ResourceManager _resourceManager =
        new("Financier.Common.Localization.Resources", typeof(LocalizationService).Assembly);

    private CultureInfo _currentCulture = CultureInfo.GetCultureInfo("uk");
    private CultureInfo _defaultCulture = CultureInfo.GetCultureInfo("en");

    private LocalizationService() { }

    /// <summary>Gets the process-wide singleton instance.</summary>
    public static LocalizationService Instance => _lazy.Value;

    /// <summary>
    /// Gets or sets the active culture.  Setting a new value fires
    /// <see cref="PropertyChanged"/> for <c>Item[]</c>, which refreshes every
    /// bound <c>{local:Translate}</c> extension simultaneously.
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (Equals(_currentCulture, value))
                return;

            _currentCulture = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            // Raise Item[] to refresh every active indexer binding at once.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }

    /// <summary>
    /// Returns the localized string for <paramref name="key"/> in the
    /// <see cref="CurrentCulture"/>.  Falls back to the neutral (English)
    /// resources; returns <c>[key]</c> if the key is missing entirely.
    /// </summary>
    public string this[string key]
    {
        get
        {
            var result = _resourceManager.GetString(key, _currentCulture);
            if (string.IsNullOrEmpty(result) && !Debugger.IsAttached)
            {
                result = _resourceManager.GetString(key, _defaultCulture);
            }
            return result ?? $"[{key}]";
        }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
}
