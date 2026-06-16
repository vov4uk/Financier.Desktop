using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Financier.Common.Entities;

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

    private CultureInfo _currentCulture;
    private CultureInfo _defaultCulture = CultureInfo.GetCultureInfo("en");

    private LocalizationService()
    {
    }

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

    public void ApplyLanguage(Language language)
    {
        var culture = language switch
        {
            Language.English => CultureInfo.GetCultureInfo("en"),
            Language.Ukrainian => CultureInfo.GetCultureInfo("uk"),
            Language.Polish => CultureInfo.GetCultureInfo("pl"),
            _ => _defaultCulture,
        };

        if (culture != CurrentCulture)
        {
            CurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;
            DbManual.ResetManuals(nameof(DbManual.MCCEnums));

            var xmlLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            if (Application.Current != null)
            {
                foreach (Window window in Application.Current.Windows)
                    window.Language = xmlLang;
            }
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

#nullable enable
    public event PropertyChangedEventHandler? PropertyChanged;
#nullable disable

    private string Get([CallerMemberName] string key = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return this[key];
    }

    public string pumb => Get();
    public string privat => Get();
    public string pko => Get();
    public string pireus => Get();
    public string monobank => Get();
    public string a_bank => Get();
    public string import => Get();
    public string settings => Get();
    public string delete => Get();
    public string transaction => Get();
    public string rule => Get();
    public string location => Get();
    // RecipesWizard Page1
    public string reciept_wizard_total_format => Get();
    // MainWindow Messages
    public string backup_done => Get();
    public string import_result => Get();
    public string import_result_with_duplicates => Get();
    public string saved_message => Get();
    public string latest_version => Get();
    public string update_available => Get();
    public string update_available_question => Get();
    public string downloading_update => Get();
    public string update_downloaded => Get();
    public string update_failed => Get();
    public string exchange_rates_updated => Get();
    public string exchange_rates_exist => Get();
    public string exchange_rates_not_updated => Get();
    public string exchange_rates_provider_not_configured => Get();
    public string settings_corrupted => Get();
    public string entities_loaded => Get();
    public string sub_transaction => Get();

    // Delete Confirmation Messages
    public string confirm_delete_transaction => Get();

    // Dialog Messages
    public string split_transfers_currency_not_supported => Get();
    public string not_supported => Get();

    public string transfer => Get();

    public string rule_title_category => Get();
    public string rule_title_location => Get();
    public string rule_title_payee => Get();
    public string rule_title_project => Get();
    public string rule_title_and => Get();
    public string please_select_categories => Get();
    public string please_select_account => Get();
    public string please_select_transaction_title => Get();
}
