using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

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

    private LocalizationService()
    {
        Thread.CurrentThread.CurrentCulture = _currentCulture;
        Thread.CurrentThread.CurrentUICulture = _currentCulture;

        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
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
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            // Raise Item[] to refresh every active indexer binding at once.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XmlLanguage)));
        }
    }

    public XmlLanguage XmlLanguage
    {
        get
        {
            var l = XmlLanguage.GetLanguage("uk-UA");
            return l;
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

    private string Get([CallerMemberName] string? key = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return this[key];
    }
    public string drag_and_drop => Get();
    public string download => Get();
    public string settings => Get();
    public string accounts => Get();
    public string blotter => Get();
    public string currencies => Get();
    public string categories => Get();
    public string projects => Get();
    public string title => Get();
    public string payee => Get();
    public string edit => Get();
    public string delete => Get();
    public string project => Get();
    public string transaction => Get();
    public string locations => Get();

    public string reports => Get();

    public string duplicate => Get();

    public string entities => Get();
    public string payees => Get();

    public string add_transfer => Get();

    public string info => Get();

    public string exchange_rates => Get();

    public string address => Get();
    public string name => Get();
    public string active => Get();
    public string id => Get();
    public string rule => Get();
    public string add => Get();
    public string main => Get();
    public string checkForUpdates => Get();
    public string general => Get();
    public string add_template => Get();
    public string add_expense => Get();
    public string pko_pdf => Get();
    public string privat_xlsx => Get();
    public string pireus_pdf => Get();
    public string pumb_pdf => Get();
    public string pdf => Get();
    public string csv => Get();
    public string xlsx => Get();
    public string monobank_csv => Get();
    public string a_bank => Get();
    public string import => Get();
    public string rules => Get();
    public string exit => Get();
    public string setDefaultDirectory => Get();
    public string defaultBackupDirectory => Get();
    public string saveAsDb => Get();
    public string saveBackup => Get();
    public string openBackup => Get();

    public string transaction_details => Get();
    public string account => Get();
    public string category => Get();
    public string currency => Get();
    public string same_as_account => Get();
    public string unsplit_amount => Get();
    public string open_recipes_wizard => Get();
    public string add_split_transfers => Get();
    public string add_split_transactions => Get();
    public string location => Get();
    public string note => Get();
    public string save => Get();
    public string cancel => Get();

    public string refresh => Get();
    public string clear => Get();
    public string time => Get();
    public string amount => Get();
    public string balance => Get();

    public string description => Get();
    public string total_amount => Get();
    public string include => Get();
    public string last_transaction => Get();

    public string symbol => Get();
    public string is_default => Get();

    public string location_details => Get();
    public string is_active => Get();
    public string rule_details => Get();
    public string conditions => Get();
    public string actions => Get();
    public string what_should_happen => Get();
    public string check_for_updates_on_start => Get();
    public string providers => Get();
    public string app_id => Get();
    public string update_exchange_rates_on_start => Get();
    public string details => Get();
    public string transfer_details => Get();
    public string from_account => Get();
    public string to_account => Get();
    public string rate => Get();
    public string from => Get();
    public string to => Get();
    public string date => Get();
    public string created => Get();
    public string condition => Get();

    // Wizard common
    public string back => Get();
    public string finish => Get();
    public string next_step => Get();

    // MonoWizard Page1
    public string select_account => Get();

    // MonoWizard Page2
    public string please_select_transaction => Get();
    public string operation_amount => Get();
    public string exchange_rate => Get();
    public string commission => Get();
    public string cashback => Get();

    // MonoWizard Page3
    public string please_select_category_account => Get();
    public string new_rule => Get();
    public string delete_row => Get();
    public string clear_notes => Get();
    public string add_rule_tooltip => Get();
    public string delete_row_tooltip => Get();
    public string clear_notes_tooltip => Get();
    public string mcc => Get();
    public string original => Get();

    // RecipesWizard Page1
    public string recipes_instructions_line1 => Get();
    public string recipes_instructions_line2 => Get();
    public string paste => Get();
    public string highlight => Get();
    public string paste_tooltip => Get();
    public string highlight_tooltip => Get();
    public string reciept_wizard_total_format => Get();

    // RecipesWizard Page2
    public string please_select_category_project => Get();
    public string total_label => Get();
    public string add_row => Get();
    public string order => Get();
    public string calculate_totals => Get();
    public string add_new_transaction => Get();

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

    // Delete Confirmation Messages
    public string confirm_delete_transaction => Get();

    // Dialog Messages
    public string split_transfers_currency_not_supported => Get();
    public string not_supported => Get();

    // Reports
    public string income_expense_period => Get();
    public string structure => Get();
    public string dynamics => Get();
}
