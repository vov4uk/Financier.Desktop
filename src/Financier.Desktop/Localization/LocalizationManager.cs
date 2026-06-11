using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Financier.Desktop.Data;
using PowerKit;
using PowerKit.Extensions;

namespace Financier.Desktop.Localization;

public partial class LocalizationManager : ObservableObject, IDisposable
{
    private readonly IDisposable _eventSubscription;

    public LocalizationManager(SettingsGeneralDTO settingsService)
    {
        _eventSubscription = Disposable.Merge(
            settingsService.WatchProperty(o => o.Language, v => Language = v, true),
            this.WatchProperty(
                o => o.Language,
                _ =>
                {
                    foreach (var propertyName in EnglishLocalization.Keys)
                        OnPropertyChanged(propertyName);
                }
            )
        );
    }

    [ObservableProperty]
    public partial Language Language { get; set; }

    private string Get([CallerMemberName] string? key = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        var localization = Language switch
        {
            Language.System =>
                CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLowerInvariant() switch
                {
                    "ukr" => UkrainianLocalization,
                    "pol" => PolishLocalization,
                    _ => EnglishLocalization,
                },
            Language.Ukrainian => UkrainianLocalization,
            Language.Polish => PolishLocalization,
            _ => EnglishLocalization,
        };

        if (
            localization.TryGetValue(key, out var value)
            // English is used as a fallback
            || EnglishLocalization.TryGetValue(key, out value)
        )
        {
            return value;
        }

        return $"Missing localization for '{key}'";
    }

    public void Dispose() => _eventSubscription.Dispose();
}

public partial class LocalizationManager
{
    public string settings => Get();
    public string accounts => Get();
    public string blotter => Get();
    public string currencies => Get();
    public string categories => Get();
    public string budgets => Get();
    public string projects => Get();
    public string ok => Get();
    public string reset => Get();
    public string clear => Get();
    public string save => Get();
    public string cancel => Get();
    public string close => Get();
    public string edit_account => Get();
    public string add_account => Get();
    public string title => Get();
    public string payee => Get();
    public string currency => Get();
    public string add_currency => Get();
    public string opening_amount => Get();
    public string limit_amount => Get();
    public string view => Get();
    public string edit => Get();
    public string delete => Get();
    public string add_transaction => Get();
    public string account => Get();
    public string category => Get();
    public string add_category => Get();
    public string income => Get();
    public string expense => Get();
    public string amount => Get();
    public string date => Get();
    public string time => Get();
    public string parent => Get();
    public string total => Get();
    public string filter => Get();
    public string no_filter => Get();
    public string transfer => Get();
    public string confirmation => Get();
    public string delete_category_dialog => Get();
    public string currency_delete_alert => Get();
    public string yes => Get();
    public string no => Get();
    public string error => Get();
    public string include_subcategories => Get();
    public string include_subcategories_summary => Get();
    public string start_date => Get();
    public string end_date => Get();
    public string budget => Get();
    public string no_limit => Get();
    public string period => Get();
    public string period_from => Get();
    public string period_to => Get();
    public string period_recur => Get();
    public string preferences => Get();
    public string amount_picker => Get();
    public string integer_part_title => Get();
    public string use_hundredth_title => Get();
    public string project => Get();
    public string select_account => Get();
    public string select_currency => Get();
    public string select_category => Get();
    public string autocomplete_filter_hint => Get();
    public string select_project => Get();
    public string warning => Get();
    public string no_accounts => Get();
    public string no_budgets => Get();
    public string loading => Get();
    public string no_transactions => Get();
    public string no_categories => Get();
    public string no_currencies => Get();
    public string no_projects => Get();
    public string any_projects => Get();
    public string no_recur => Get();
    public string no_suggestion => Get();
    public string account_from => Get();
    public string account_to => Get();
    public string amount_from => Get();
    public string amount_to => Get();
    public string rate => Get();
    public string no_rate => Get();
    public string transaction => Get();
    public string transaction_title => Get();
    public string transfer_title => Get();
    public string about => Get();
    public string about_copyright2 => Get();
    public string menu => Get();
    public string location => Get();
    public string locations => Get();
    public string no_locations => Get();
    public string use_gps => Get();
    public string use_gps_summary => Get();
    public string use_my_location => Get();
    public string use_my_location_summary => Get();
    public string select_location => Get();
    public string no_fix => Get();
    public string attribute => Get();
    public string attributes => Get();
    public string parent_attributes => Get();
    public string change_attributes => Get();
    public string calculator => Get();
    public string no_attributes => Get();
    public string add_attribute => Get();
    public string new_attribute => Get();

    public string attribute_type => Get();
    public string attribute_name => Get();
    public string attribute_values => Get();
    public string attribute_values_hint => Get();
    public string attribute_default_value => Get();
    public string checkbox_values => Get();
    public string checkbox_values_yes => Get();
    public string checkbox_values_no => Get();
    public string checkbox_values_hint => Get();
    public string location_name => Get();
    public string satellite_on => Get();
    public string satellite_off => Get();
    public string my_location => Get();
    public string move_marker_to_center => Get();
    public string move_go_to_marker => Get();
    public string tap_location => Get();
    public string note => Get();
    public string note_text_containing => Get();
    public string note_text_containing_value => Get();
    public string service_is_not_available => Get();
    public string resolve_address => Get();
    public string resolving_address => Get();
    public string report => Get();
    public string reports => Get();
    public string no_reports => Get();
    public string select_report => Get();
    public string overall => Get();
    public string report_by_period => Get();
    public string report_by_period_summary => Get();
    public string report_by_category => Get();
    public string report_by_category_summary => Get();
    public string report_by_location => Get();
    public string report_by_location_summary => Get();
    public string report_by_project => Get();
    public string report_by_project_summary => Get();

    public string report_by_account_by_period => Get();
    public string report_by_account_by_period_summary => Get();
    public string report_by_category_by_period => Get();
    public string report_by_category_by_period_summary => Get();
    public string report_by_location_by_period => Get();
    public string report_by_location_by_period_summary => Get();
    public string report_by_project_by_period => Get();
    public string report_by_project_by_period_summary => Get();
    public string report_by_account_balance_by_period => Get();
    public string report_by_account_balance_by_period_summary => Get();
    public string max_result_label => Get();
    public string min_result_label => Get();
    public string mean_result_label => Get();
    public string mean_line_label => Get();
    public string sum_result_label => Get();
    public string report_init_res => Get();
    public string no_data_to_report => Get();
    public string report_preferences_not_set => Get();
    public string currency_not_set => Get();
    public string period_not_set => Get();
    public string report_no_category => Get();
    public string report_no_project => Get();
    public string report_no_account => Get();
    public string report_no_location => Get();
    public string no_account => Get();
    public string filter_transfer => Get();
    public string filter_transfer_exclude => Get();

    public string period_today => Get();
    public string period_yesterday => Get();
    public string period_this_week => Get();
    public string period_this_month => Get();
    public string period_this_year => Get();
    public string period_last_week => Get();
    public string period_last_month => Get();
    public string period_last_year => Get();
    public string currency_name => Get();
    public string currency_symbol => Get();
    public string currency_code => Get();
    public string currency_code_hint => Get();
    public string is_default => Get();
    public string update_exchange_rate => Get();
    public string is_included_into_totals => Get();
    public string is_included_into_totals_summary => Get();
    public string decimals => Get();
    public string decimal_separator => Get();
    public string group_separator => Get();
    public string field_separator => Get();
    public string include_header => Get();
    public string include_tx_status => Get();
    public string delete_account_confirm => Get();
    public string delete_transaction_confirm => Get();
    public string delete_budget_confirm => Get();
    public string delete_budget_recurring_select => Get();
    public string delete_budget_recurring_confirm => Get();
    public string delete_budget_one_entry => Get();
    public string delete_budget_all_entries => Get();
    public string edit_recurring_budget => Get();

    public string account_type => Get();
    public string card_issuer => Get();
    public string account_type_cash => Get();
    public string account_type_bank => Get();
    public string account_type_debit_card => Get();
    public string account_type_credit_card => Get();
    public string account_type_asset => Get();
    public string account_type_liability => Get();
    public string account_type_other => Get();
    public string card_type => Get();
    public string card_number => Get();
    public string card_number_hint => Get();
    public string issuer => Get();
    public string icon_text => Get();
    public string accent_color => Get();
    public string select_color => Get();
    public string cash => Get();
    public string sort_order => Get();
    public string force_gps_location => Get();
    public string whats_new => Get();
    public string attribute_delete_alert => Get();
    public string user_interface => Get();
    public string sort_accounts => Get();
    public string sort_accounts_summary => Get();
    public string blur_balances => Get();
    public string blur_balances_summary => Get();

    public string sort_locations => Get();
    public string sort_locations_summary => Get();

    public string sort_templates => Get();
    public string sort_templates_summary => Get();

    public string secure_window => Get();
    public string secure_window_summary => Get();
    public string more_products => Get();
    public string restore => Get();
    public string backup_database => Get();
    public string backup_database_inprogress => Get();
    public string backup_database_gdocs => Get();
    public string backup_database_gdocs_inprogress => Get();
    public string csv_export => Get();
    public string csv_export_inprogress => Get();
    public string restore_database => Get();
    public string restore_database_inprogress => Get();
    public string restore_database_gdocs => Get();
    public string restore_database_inprogress_gdocs => Get();
    public string restore_database_inprogress_dropbox => Get();
    public string restore_database_success => Get();
    public string gdocs_connection_failed => Get();
    public string gdocs_folder_not_found => Get();
    public string gdocs_credentials_not_configured => Get();
    public string gdocs_folder_not_configured => Get();
    public string gdocs_backup_failed => Get();
    public string gdocs_restore_failed => Get();

    public string gdocs_folder_error => Get();
    public string gdocs_service_error => Get();
    public string gdocs_io_error => Get();

    public string success => Get();
    public string fail => Get();
    public string duplicate => Get();
    public string duplicate_keep_time => Get();
    public string duplicate_keep_date_time => Get();
    public string change_to_transfer => Get();
    public string change_to_transaction => Get();
    public string no_suitable_account_for_transfer => Get();
    public string select_from_account => Get();
    public string select_to_account => Get();
    public string update_balance => Get();
    public string close_account => Get();
    public string close_account_confirm => Get();
    public string reopen_account => Get();
    public string difference => Get();
    public string new_balance => Get();
    public string duplicate_success => Get();
    public string duplicate_success_keep_time => Get();
    public string duplicate_success_keep_date_time => Get();
    public string duplicate_success_with_multiplier => Get();
    public string no_category => Get();
    public string no_project => Get();
    public string no_period => Get();
    public string current_location => Get();
    public string protection => Get();
    public string transaction_screen => Get();
    public string transaction_screen_summary => Get();
    public string screen_layout => Get();
    public string show_location => Get();
    public string show_location_summary => Get();
    public string show_location_order => Get();
    public string show_location_order_summary => Get();
    public string show_payee => Get();
    public string show_payee_summary => Get();
    public string show_payee_order => Get();
    public string show_payee_order_summary => Get();
    public string show_note => Get();
    public string show_note_summary => Get();
    public string show_note_order => Get();
    public string show_note_order_summary => Get();
    public string show_project => Get();
    public string show_project_summary => Get();
    public string show_project_order => Get();
    public string show_project_order_summary => Get();
    public string remember_selection => Get();
    public string remember_last_account => Get();
    public string remember_last_account_summary => Get();
    public string remember_last_location => Get();
    public string remember_last_location_summary => Get();
    public string remember_last_project => Get();
    public string remember_last_project_summary => Get();
    public string position_move_top => Get();
    public string position_move_up => Get();
    public string position_move_down => Get();
    public string position_move_bottom => Get();
    public string add_sibling => Get();
    public string add_child => Get();
    public string interval => Get();
    public string recur_interval => Get();
    public string recur_interval_no_recur => Get();
    public string recur_interval_every_x_day => Get();
    public string recur_interval_every_x_week => Get();
    public string recur_interval_every_x_month => Get();
    public string recur_interval_every_x_year => Get();
    public string recur_interval_daily => Get();
    public string recur_interval_weekly => Get();
    public string recur_interval_monthly => Get();
    public string recur_interval_semi_monthly => Get();
    public string recur_interval_yearly => Get();
    public string recur_stops_on_date => Get();
    public string recur_stops_on_date_summary => Get();
    public string recur_indefinitely => Get();
    public string recur_exactly_n_times => Get();
    public string recur_exactly_n_times_summary => Get();
    public string recur_period_every => Get();
    public string recur_period_days => Get();
    public string recur_period_semi_monthly => Get();
    public string recur_repeat_indefinitely => Get();
    public string recur_repeat_exactly => Get();
    public string recur_repeat_times => Get();
    public string recur_repeat_starts_on => Get();
    public string recur_repeat_stops_on => Get();
    public string recur_error_specify_days => Get();
    public string recur_error_specify_first_day => Get();
    public string recur_error_specify_second_day => Get();
    public string recur_error_specify_times => Get();
    public string recur => Get();
    public string recur_period => Get();
    public string day_mon => Get();
    public string day_tue => Get();
    public string day_wed => Get();
    public string day_thr => Get();
    public string day_fri => Get();
    public string day_sat => Get();
    public string day_sun => Get();
    public string home_screen => Get();
    public string shortcut_new_transaction => Get();
    public string shortcut_new_transfer => Get();
    public string shortcut_summary => Get();
    public string shortcut_not_supported_by_launcher => Get();
    public string use_fixed_layout => Get();
    public string use_fixed_layout_summary => Get();
    public string use_twin_date_picker => Get();
    public string use_twin_date_picker_summary => Get();
    public string show_account_balance_on_selector => Get();
    public string show_account_balance_on_selector_summary => Get();
    public string no_data => Get();
    public string calculating => Get();
    public string budget_mode => Get();
    public string budget_mode_summary => Get();
    public string gdocs_backup => Get();
    public string user_login => Get();
    public string user_login_summary => Get();
    public string user_password => Get();
    public string user_password_summary => Get();
    public string backup_folder => Get();
    public string backup_folder_not_configured => Get();
    public string backup_folder_summary => Get();

    public string report_preferences => Get();
    public string all_reports_preferences => Get();
    public string category_by_period_preferences => Get();
    public string report_include_sub_categories => Get();
    public string report_include_sub_categories_summary => Get();
    public string report_add_sub_categories_result => Get();
    public string report_add_sub_categories_result_summary => Get();
    public string report_consider_null_results => Get();
    public string report_consider_null_results_summary => Get();
    public string report_include_no_filter => Get();
    public string report_include_no_filter_summary => Get();
    public string report_reference_month => Get();
    public string report_reference_month_summary => Get();
    public string report_last_quarter => Get();
    public string report_last_half_year => Get();
    public string report_last_9_months => Get();
    public string report_last_year => Get();

    public string report_reference_period => Get();
    public string report_reference_period_summary => Get();

    public string report_reference_currency => Get();
    public string report_reference_currency_summary => Get();
    public string report_last_n_months => Get();
    public string report_last_n_years => Get();
    public string report_n_months_var => Get();
    public string month_jan => Get();
    public string month_feb => Get();
    public string month_mar => Get();
    public string month_apr => Get();
    public string month_mai => Get();
    public string month_jun => Get();
    public string month_jul => Get();
    public string month_aug => Get();
    public string month_sep => Get();
    public string month_oct => Get();
    public string month_nov => Get();
    public string month_dec => Get();
    public string month_x_jan => Get();
    public string month_x_feb => Get();
    public string month_x_mar => Get();
    public string month_x_apr => Get();
    public string month_x_mai => Get();
    public string month_x_jun => Get();
    public string month_x_jul => Get();
    public string month_x_aug => Get();
    public string month_x_sep => Get();
    public string month_x_oct => Get();
    public string month_x_nov => Get();
    public string month_x_dec => Get();

    public string other => Get();
    public string template_name => Get();
    public string transaction_templates => Get();
    public string transaction_template => Get();
    public string transfer_template => Get();
    public string no_templates => Get();
    public string template => Get();
    public string new_from_template => Get();
    public string delete_template_confirm => Get();
    public string save_as_template => Get();
    public string save_as_template_success => Get();
    public string scheduled_transactions => Get();
    public string no_scheduled_transactions => Get();
    public string recur_interval_geeky => Get();
    public string recurrence_pattern => Get();
    public string recurrence_period => Get();
    public string recurrence_period_starts_on => Get();
    public string recurrence_weekly_days => Get();
    public string recurrence_monthly_pattern => Get();
    public string recurrence_monthly_every_nth_day => Get();
    public string recurrence_monthly_specific_day => Get();
    public string recur_interval_semi_monthly_1 => Get();
    public string recur_interval_semi_monthly_2 => Get();
    public string recurrence_none => Get();
    public string specify_value => Get();
    public string recurrence_evaluate => Get();
    public string first => Get();
    public string second => Get();
    public string third => Get();
    public string fourth => Get();
    public string last => Get();
    public string day => Get();
    public string weekday => Get();
    public string weekend_day => Get();
    public string sunday => Get();
    public string monday => Get();
    public string tuesday => Get();
    public string wednesday => Get();
    public string thursday => Get();
    public string friday => Get();
    public string saturday => Get();
    public string transaction_schedule => Get();
    public string transfer_schedule => Get();
    public string recurrence_period_starts_on_date => Get();
    public string recurrence_period_starts_on_time => Get();
    public string new_scheduled_transfer_title => Get();
    public string new_scheduled_transfer_text => Get();
    public string new_scheduled_transaction_title => Get();
    public string new_scheduled_transaction_text => Get();
    public string new_scheduled_transaction_notification => Get();
    public string new_scheduled_transaction_debit => Get();
    public string new_scheduled_transaction_credit => Get();
    public string new_scheduled_transfer_notification_differ_currency => Get();
    public string new_scheduled_transfer_notification_same_currency => Get();
    public string notification => Get();
    public string notification_options_on => Get();
    public string notification_options_off => Get();
    public string notification_options_short => Get();
    public string notification_options_2_short => Get();
    public string notification_options_3_short => Get();
    public string notification_options_long => Get();
    public string notification_options_2_long => Get();
    public string notification_options_3_long => Get();
    public string notification_options_led_green => Get();
    public string notification_options_led_blue => Get();
    public string notification_options_led_yellow => Get();
    public string notification_options_led_red => Get();
    public string notification_options_led_pink => Get();
    public string notification_options_text => Get();
    public string notification_options_custom => Get();
    public string notification_options_default => Get();
    public string notification_sound => Get();
    public string notification_vibra => Get();
    public string notification_led => Get();
    public string transaction_status => Get();
    public string transaction_status_pending => Get();
    public string transaction_status_unreconciled => Get();
    public string transaction_status_cleared => Get();
    public string transaction_status_reconciled => Get();
    public string scheduled => Get();
    public string expired => Get();
    public string pending => Get();
    public string system_attribute_delete_after_expired => Get();
    public string no_transaction_found => Get();
    public string include_credit => Get();
    public string include_credit_summary => Get();
    public string enable_widget => Get();
    public string enable_widget_summary => Get();
    public string attach_picture => Get();
    public string new_picture => Get();

    public string credits => Get();
    public string sort_by_title => Get();
    public string sort_all_by_title => Get();
    public string include_transfers_into_reports => Get();
    public string include_transfers_into_reports_summary => Get();
    public string treat_transfer_to_ccard_as_payment => Get();
    public string treat_transfer_to_ccard_as_payment_summary => Get();
    public string negative_opening_amount => Get();
    public string negative_opening_amount_summary => Get();
    public string payment_day => Get();
    public string closing_day => Get();
    public string payment_day_hint => Get();
    public string closing_day_hint => Get();

    public string closing_day_title => Get();
    public string reference_period => Get();
    public string regular_closing_day => Get();
    public string custom_closing_day => Get();
    public string alert_invalid_closing_day => Get();
    public string alert_null_closing_day => Get();
    public string alert_regular_closing_day => Get();

    public string ccard_statement => Get();
    public string monthly_view => Get();
    public string credit_card_statement => Get();
    public string ccard_statement_title => Get();
    public string ccard_par => Get();
    public string bill_on => Get();
    public string monthly_result => Get();
    public string closing_day_error => Get();
    public string payment_day_error => Get();
    public string statement_error => Get();
    public string is_ccard_payment => Get();
    public string is_ccard_payment_summary => Get();
    public string header_payments => Get();
    public string header_credits => Get();
    public string header_expenses => Get();
    public string ui_language => Get();
    public string ui_language_summary => Get();
    public string ui_theme => Get();
    public string ui_theme_summary => Get();

    public string enter_pin => Get();
    public string mass_operations => Get();
    public string account_type_paypal => Get();
    public string mass_operations_use_filter => Get();
    public string mass_operations_mark_restored_all => Get();
    public string mass_operations_mark_pending_all => Get();
    public string mass_operations_mark_unreconciled_all => Get();
    public string mass_operations_clear_all => Get();
    public string mass_operations_reconcile => Get();
    public string mass_operations_delete => Get();
    public string proceed => Get();
    public string apply_mass_op => Get();
    public string apply_mass_op_zero_count => Get();
    public string entities => Get();
    public string transaction_status_restored => Get();
    public string scheduled_transactions_restored => Get();
    public string scheduled_transactions_have_been_restored => Get();
    public string restore_missed_scheduled_transactions => Get();
    public string restore_missed_scheduled_transactions_summary => Get();
    public string save_and_new => Get();
    public string show_picture => Get();
    public string show_picture_summary => Get();
    public string payees => Get();
    public string report_by_payee => Get();
    public string report_by_payee_summary => Get();
    public string report_by_payee_by_period => Get();
    public string report_by_payee_by_period_summary => Get();
    public string no_payee => Get();
    public string no_payees => Get();
    public string report_no_payee => Get();
    public string donate => Get();
    public string donate_error => Get();
    public string type => Get();

    public string qif_export => Get();
    public string qif_export_inprogress => Get();
    public string all_accounts => Get();
    public string filter_value_not_found => Get();
    public string empty_report => Get();
    public string day_of_month => Get();

    public string show_running_balance => Get();
    public string show_running_balance_summary => Get();

    public string colorize_blotter_item => Get();
    public string colorize_blotter_item_summary => Get();

    public string show_project_in_blotter => Get();
    public string show_project_in_blotter_summary => Get();

    public string reset_copied_transaction_status => Get();
    public string reset_copied_transaction_status_summary => Get();

    public string reset_copied_foreign_transaction_status => Get();
    public string reset_copied_foreign_transaction_status_summary => Get();

    public string update_copied_transaction_project => Get();
    public string update_copied_transaction_project_summary => Get();

    public string colorize_weekend_date => Get();
    public string colorize_weekend_date_summary => Get();

    public string blotter_show_time_of_day => Get();
    public string blotter_show_time_of_day_summary => Get();

    public string new_currency => Get();
    public string lock_time => Get();
    public string lock_time_summary => Get();


    public string date_format => Get();
    public string pin_protection_lock_transaction => Get();
    public string pin_protection_lock_transaction_summary => Get();

    public string split => Get();
    public string split_transaction => Get();
    public string split_transfer => Get();
    public string unsplit_amount => Get();
    public string unsplit_amount_greater_than_zero => Get();
    public string delete_transaction_parent_confirm => Get();
    public string downloading_rate => Get();
    public string rate_as_of => Get();
    public string rate_info => Get();
    public string downloading_rates => Get();

    public string unsplit_adjust_amount => Get();
    public string unsplit_adjust_amount_summary => Get();
    public string unsplit_adjust_evenly => Get();
    public string unsplit_adjust_evenly_summary => Get();
    public string unsplit_adjust_last => Get();
    public string unsplit_adjust_last_summary => Get();

    public string add_transfer => Get();
    public string file_name => Get();
    public string choose_account => Get();
    public string no_filemanager_installed => Get();
    public string csv_import => Get();
    public string csv_import_inprogress => Get();
    public string select_filename => Get();
    public string import_file_not_found => Get();
    public string import_file_not_found_2 => Get();
    public string import_unknown_category => Get();
    public string import_unknown_project => Get();
    public string import_wrong_currency => Get();
    public string import_wrong_currency_2 => Get();
    public string import_illegal_argument_exception => Get();
    public string import_parse_error => Get();
    public string csv_import_error => Get();
    public string file_import_permission => Get();
    public string provided_by_other_app => Get();
    public string csv_date_required => Get();
    public string csv_date_format_error => Get();
    public string csv_time_required => Get();
    public string csv_time_format_error => Get();
    public string csv_amount_required => Get();
    public string csv_txid_not_found => Get();
    public string csv_currency_not_found => Get();
    public string csv_account_not_found => Get();
    public string csv_import_account_selector => Get();

    public string qif_import => Get();
    public string qif_import_inprogress => Get();
    public string qif_import_error => Get();
    public string qif_import_success => Get();
    public string import_export => Get();
    public string qif_import_disclaimer => Get();

    public string googledrive => Get();
    public string googledrive_authorize => Get();
    public string googledrive_authorize_summary => Get();
    public string googledrive_upload => Get();
    public string googledrive_upload_summary => Get();
    public string googledrive_folder => Get();
    public string googledrive_folder_summary => Get();

    public string remember_last_category_for_payee => Get();
    public string remember_last_category_for_payee_summary => Get();

    public string currency_symbol_format => Get();
    public string currency_number_format => Get();

    public string use_header_from_file => Get();

    public string info => Get();
    public string balance => Get();
    public string show_category_in_transfer => Get();
    public string show_category_in_transfer_summary => Get();
    public string auto_backup => Get();
    public string auto_backup_enabled => Get();
    public string auto_backup_enabled_summary => Get();
    public string auto_backup_time => Get();
    public string auto_backup_time_summary => Get();
    public string select_folder => Get();
    public string create => Get();
    public string create_new_folder => Get();
    public string create_new_folder_title => Get();
    public string create_new_folder_fail => Get();

    public string database_backup => Get();
    public string database_backup_folder => Get();
    public string database_backup_folder_summary => Get();

    public string backup_database_to => Get();
    public string backup_database_to_title => Get();

    public string license => Get();

    public string quick_menu_account_enabled => Get();
    public string quick_menu_account_enabled_summary => Get();
    public string quick_menu_transaction_enabled => Get();
    public string quick_menu_transaction_enabled_summary => Get();
    public string quick_menu_transaction_additional_status => Get();
    public string quick_menu_transaction_additional_status_summary => Get();
    public string quick_menu_transaction_duplicate_keep_time => Get();
    public string quick_menu_transaction_duplicate_keep_time_summary => Get();
    public string quick_menu_transaction_duplicate_keep_date_time => Get();
    public string quick_menu_transaction_duplicate_keep_date_time_summary => Get();

    public string dropbox => Get();
    public string dropbox_authorize => Get();
    public string dropbox_authorize_summary => Get();
    public string dropbox_authorized => Get();
    public string dropbox_unlink => Get();
    public string dropbox_unlink_summary => Get();
    public string dropbox_upload_backup => Get();
    public string dropbox_upload_backup_summary => Get();
    public string dropbox_upload_autobackup => Get();
    public string dropbox_upload_autobackup_summary => Get();
    public string dropbox_upload_pictures => Get();
    public string dropbox_upload_pictures_summary => Get();
    public string dropbox_download_pictures => Get();
    public string dropbox_download_pictures_summary => Get();
    public string dropbox_uploading_file => Get();
    public string dropbox_error => Get();
    public string dropbox_auth_error => Get();
    public string dropbox_loading_files => Get();

    public string exchange_rates => Get();
    public string no_exchange_rates => Get();
    public string exchange_rate => Get();
    public string rate_from_currency => Get();
    public string rate_to_currency => Get();
    public string currency_make_default => Get();
    public string currency_make_default_warning => Get();

    public string back => Get();
    public string select => Get();
    public string category_selector => Get();
    public string use_hierarchical_category_selector => Get();
    public string use_hierarchical_category_selector_summary => Get();
    public string show_recently_used_category => Get();
    public string show_recently_used_category_summary => Get();
    public string hierarchical_category_selector_select_child_immediately => Get();
    public string hierarchical_category_selector_select_child_immediately_summary => Get();
    public string hierarchical_category_selector_income_expense => Get();
    public string hierarchical_category_selector_income_expense_summary => Get();

    public string totals => Get();
    public string account_total_in_currency => Get();
    public string blotter_total_in_currency => Get();
    public string budget_total_in_currency => Get();
    public string home_currency_total => Get();
    public string total_rate => Get();
    public string home_currency => Get();
    public string upload_to_dropbox => Get();
    public string upload_to_gdrive => Get();
    public string not_available => Get();
    public string rate_not_available_on_date_error => Get();
    public string rate_not_available_error => Get();

    public string collapse_blotter_buttons => Get();
    public string collapse_blotter_buttons_summary => Get();

    public string is_active => Get();

    public string report_income_expense_both => Get();
    public string report_income_expense_income => Get();
    public string report_income_expense_expense => Get();
    public string report_income_expense_summary => Get();

    public string csv_import_inprogress_update => Get();

    public string delete_old_transactions => Get();
    public string purge_account_backup_database => Get();
    public string purge_account_date_summary => Get();
    public string purge_account_confirm_title => Get();
    public string purge_account_confirm_message => Get();
    public string purge_account_payee => Get();
    public string purge_account_in_progress => Get();
    public string purge_account_unable_to_do_backup => Get();

    public string show_account_last_transaction_date => Get();
    public string show_account_last_transaction_date_summary => Get();
    public string account_list_date => Get();
    public string account_list_date_summary => Get();


    public string accounts_list_screen => Get();
    public string accounts_list_screen_summary => Get();
    public string blotter_screen => Get();
    public string blotter_screen_summary => Get();
    public string hide_closed_accounts => Get();
    public string hide_closed_accounts_summary => Get();
    public string re_index_categories => Get();
    public string startup_screen => Get();
    public string startup_screen_summary => Get();


    public string integrity_error => Get();
    public string integrity_fix => Get();
    public string integrity_fix_in_progress => Get();

    public string select_to_account_differ_from_to_account => Get();

    public string period_this_and_last_week => Get();
    public string period_this_and_last_month => Get();
    public string period_this_and_last_year => Get();
    public string period_custom => Get();

    public string original_amount => Get();
    public string original_currency_as_account => Get();
    public string split_transfer_not_supported_yet => Get();
    public string show_currency => Get();
    public string show_currency_summary => Get();
    public string enter_currency_decimal_places => Get();
    public string enter_currency_decimal_places_summary => Get();
    public string round_up_amount => Get();
    public string round_up_amount_summary => Get();
    public string reconcile => Get();

    public string file_import_utf8_warning => Get();

    public string show_is_ccard_payment => Get();
    public string show_is_ccard_payment_summary => Get();
    public string open_calculator_for_template_transactions => Get();
    public string open_calculator_for_template_transactions_summary => Get();

    public string installed_on_sd_card_warning => Get();

    public string planner => Get();

    public string period_tomorrow => Get();
    public string period_next_week => Get();
    public string period_this_and_next_week => Get();
    public string period_next_month => Get();
    public string period_this_and_next_month => Get();
    public string period_next_3_months => Get();

    public string exchange_rate_provider_error => Get();
    public string exchange_rate_provider => Get();
    public string exchange_rate_provider_summary => Get();
    public string openexchangerates_app_id_summary => Get();
    public string exchange_rate_default_currency_no_rate => Get();
    public string exchange_rate_set_default_currency => Get();
    public string download_all_rates => Get();
    public string downloading_rates_result => Get();
    public string use_exchange_rate_service_with_historical => Get();

    public string export_splits => Get();
    public string export_split_parents => Get();

    public string account_by_currency => Get();
    public string budget_type_saving => Get();
    public string budget_type_saving_summary => Get();
    public string pin_protection_haptic_feedback => Get();
    public string pin_protection_haptic_feedback_summary => Get();
    public string offline_rate => Get();

    public string show_payee_in_transfer => Get();
    public string show_payee_in_transfer_summary => Get();
    public string google_drive_backup_account => Get();
    public string google_drive_backup_account_summary => Get();
    public string google_drive_sign_out => Get();
    public string google_drive_sign_out_summary => Get();
    public string google_drive_permission_requested => Get();
    public string google_drive_permission_requested_for_account => Get();
    public string google_drive_permission_required => Get();
    public string google_drive_loading_files => Get();
    public string google_drive_account_required => Get();
    public string google_drive_signed_in_as => Get();
    public string google_drive_signed_out => Get();
    public string google_drive_authorized => Get();
    public string google_drive_backup_success => Get();
    public string google_drive_restore_in_progress => Get();
    public string google_drive_connection_resolved => Get();
    public string google_drive_connection_failed => Get();
    public string google_drive_account_select_error => Get();
    public string google_drive_list_files_failed => Get();
    public string google_drive_backup_full_readonly => Get();
    public string google_drive_backup_full_readonly_summary => Get();
    public string google_drive_uploading_file => Get();
    public string google_drive_upload_backup => Get();
    public string google_drive_upload_backup_summary => Get();
    public string google_drive_upload_autobackup => Get();
    public string google_drive_upload_autobackup_summary => Get();
    public string google_drive_upload_pictures => Get();
    public string google_drive_upload_pictures_summary => Get();
    public string google_drive_download_pictures => Get();
    public string google_drive_download_pictures_summary => Get();
    public string google_drive_error => Get();

    public string backup_restore_database_online => Get();
    public string backup_database_online_google_drive => Get();
    public string restore_database_online_google_drive => Get();
    public string backup_database_online_dropbox => Get();
    public string restore_database_online_dropbox => Get();
    public string backup_database_dropbox_inprogress => Get();

    public string request_permissions_title => Get();
    public string request_permissions_description => Get();
    public string request_permissions_storage_title => Get();
    public string request_permissions_storage_description => Get();
    public string request_permissions_camera_title => Get();
    public string request_permissions_camera_description => Get();
    public string request_permissions_get_accounts_title => Get();
    public string request_permissions_get_accounts_description => Get();
    public string request_permissions_granted => Get();
    public string request_permissions_not_granted => Get();
    public string request_permissions_receive_sms_title => Get();
    public string request_permissions_receive_sms_description => Get();
    public string request_permissions_notification_title => Get();
    public string request_permissions_notification_description => Get();
    public string request_permissions_notification_listener_title => Get();
    public string request_permissions_notification_listener_description => Get();
    public string privacy_policy => Get();

    public string show_menu_button_on_accounts_screen => Get();
    public string show_menu_button_on_accounts_screen_summary => Get();
    public string go_to_menu_tab => Get();


    public string sms_sender => Get();
    public string sms_tpl_filter_hint => Get();
    public string sms_number_hint => Get();
    public string sms_tpl => Get();
    public string sms_tpl_title => Get();
    public string sms_tpl_desc => Get();
    public string sms_tpl_hint => Get();
    public string sms_note_title => Get();
    public string sms_note_desc => Get();
    public string sms_note_hint => Get();
    public string sms_tpl_check => Get();
    public string sms_templates => Get();
    public string no_sms_templates => Get();
    public string add_sms_template => Get();
    public string notification_list => Get();
    public string notification_content => Get();
    public string notification_copied => Get();
    public string sort_sms_templates => Get();
    public string duplicate_sms_template => Get();
    public string sms_delete_alert => Get();
    public string new_sms_transaction_text => Get();
    public string new_sms_transaction_title => Get();
    public string new_intent_transaction_text => Get();
    public string new_intent_transaction_title => Get();
    public string choose_sms_template_type_and_account => Get();
    public string choose_sms_template_transfer_to_account => Get();
    public string tpl_not_transfer => Get();
    public string tpl_parse_result => Get();
    public string tpl_parse_not_found => Get();
    public string tpl_failed_to_parse => Get();
    public string pref_tpl => Get();
    public string pref_tpl_transaction_status_title => Get();
    public string pref_tpl_transaction_status => Get();
    public string pref_tpl_adding_to_note_title => Get();
    public string pref_tpl_adding_to_note => Get();
    public string sms_tpl_list_category_payee_project_name => Get();

    public string card_issuer_default => Get();
    public string card_issuer_unionpay => Get();
    public string electronic_payment_type => Get();
    public string account_type_electronic => Get();
    public string delete_account => Get();

    public string preferences_summary => Get();
    public string entities_summary => Get();
    public string scheduled_transactions_summary => Get();
    public string backup_database_summary => Get();
    public string restore_database_summary => Get();
    public string backup_database_online_google_drive_summary => Get();
    public string restore_database_online_google_drive_summary => Get();
    public string backup_database_online_dropbox_summary => Get();
    public string restore_database_online_dropbox_summary => Get();
    public string backup_database_to_summary => Get();
    public string import_export_summary => Get();
    public string mass_operations_summary => Get();
    public string planner_summary => Get();
    public string integrity_fix_summary => Get();
    public string donate_summary => Get();
    public string about_summary => Get();

    public string pin_warning => Get();

    public string auto_backup_reminder_enabled => Get();
    public string auto_backup_reminder_enabled_summary => Get();
    public string auto_backup_warning_enabled => Get();
    public string auto_backup_warning_summary => Get();
    public string auto_backup_is_not_enabled => Get();
    public string integrity_error_message => Get();
    public string autobackup_failed_message => Get();

    public string request_permissions_storage_not_granted => Get();
    public string permissions => Get();
    public string permissions_summary => Get();

    public string pin_protection_use_fingerprint => Get();
    public string pin_protection_use_fingerprint_summary => Get();
    public string pin_protection_use_fingerprint_fallback_to_pin_summary => Get();
    public string pin_protection_use_fingerprint_fallback_to_pin => Get();
    public string fingerprint_description => Get();
    public string fingerprint_hint => Get();
    public string fingerprint_auth_success => Get();
    public string fingerprint_auth_failed => Get();
    public string fingerprint_error => Get();
    public string fingerprint_unavailable_hardware => Get();
    public string fingerprint_unavailable_enrolled_fingerprints => Get();
    public string fingerprint_unavailable_unknown => Get();
    public string fingerprint_unavailable => Get();

    public string image_pick_camera => Get();
    public string image_pick_images => Get();
    public string use_pin => Get();
    public string try_fingerprint_again => Get();
    public string set_focus_on_amount_field => Get();
    public string set_focus_on_amount_field_summary => Get();

    public string notification_channel_name => Get();
    public string confirm_create_entity => Get();

    public string transaction_show_in_account_blotter => Get();
    public string invalid_rrule => Get();

    public string entity_selector_type => Get();
    public string payee_selector_type_title => Get();
    public string payee_selector_type_summary => Get();
    public string project_selector_type_title => Get();
    public string project_selector_type_summary => Get();
    public string location_selector_type_title => Get();
    public string location_selector_type_summary => Get();
    public string selector_type_summary => Get();

    public string card_issuer_mir => Get();

    public string downloading_picture_from_dropbox => Get();
    public string downloading_picture_from_google_drive => Get();
    public string missing_picture_file => Get();

    public string export_tx_ids => Get();
    public string export_attributes => Get();
    public string export_running_balance => Get();

    public string transfer_current_balance => Get();
    public string show_transfer_current_balance => Get();
    public string show_transfer_current_balance_summary => Get();

    public string first_day_of_week => Get();
    public string first_day_of_week_summary => Get();
}
