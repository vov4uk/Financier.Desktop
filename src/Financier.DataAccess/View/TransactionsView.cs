using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Financier.DataAccess.Utils;

namespace Financier.DataAccess.View
{
    public abstract class TransactionsView : Entity
    {
        [NotMapped]
        public const string TRANSFER_DELIMITER = " \u00BB ";

        public int _id { get; set; }
        public int parent_id { get; set; }
        public int from_account_id { get; set; }
        public string from_account_title { get; set; }
        public bool from_account_is_include_into_totals { get; set; }

        [ForeignKey("from_account_currency")]
        public int from_account_currency_id { get; set; }

        public int? to_account_id { get; set; }
        public string to_account_title { get; set; }

        [ForeignKey("to_account_currency")]
        public int? to_account_currency_id { get; set; }

        [ForeignKey("category")]
        public int? category_id { get; set; }

        public string category_title { get; set; }
        public int category_left { get; set; }
        public int category_right { get; set; }
        public int category_type { get; set; }
        public int? project_id { get; set; }
        public string project { get; set; }
        public int? location_id { get; set; }
        public string location { get; set; }
        public int? payee_id { get; set; }
        public string payee { get; set; }
        public string note { get; set; }
        public int from_amount { get; set; }
        public int to_amount { get; set; }
        public long datetime { get; set; }

        [ForeignKey("original_currency")]
        public int? original_currency_id { get; set; }

        public long original_from_amount { get; set; }
        public int is_template { get; set; }
        public string template_name { get; set; }
        public string recurrence { get; set; }
        public string notification_options { get; set; }
        public string status { get; set; }
        public int is_ccard_payment { get; set; }
        public long last_recurrence { get; set; }
        public string attached_picture { get; set; }
        public int? from_account_balance { get; set; }
        public int? to_account_balance { get; set; }
        public long is_transfer { get; set; }

        public virtual Currency from_account_currency { get; set; }
        public virtual Currency to_account_currency { get; set; }
        public virtual Currency original_currency { get; set; }
        public virtual Category category { get; set; }

        [NotMapped]
        public string Type
        {
            get
            {
                if (this.to_account_id > 0)
                {
                    return "Transfer";
                }
                else if (category_id == -1)
                {
                    return "Share";
                }
                else if (from_amount > 0)
                {
                    return "Income";
                }
                return "Expense";
            }
        }

        [NotMapped]
        public string AccountTitle
        {
            get
            {
                if (this.to_account_id > 0)
                {
                    return $"{from_account_title}{TRANSFER_DELIMITER}{to_account_title}";
                }
                return from_account_title;
            }
        }

        [NotMapped]
        public string TransactionTitle => TransactionTitleUtils.GenerateTransactionTitle(payee, note, location_id > 0 ? location : string.Empty, category_id, category_title, to_account_id);

        [NotMapped]
        public string AmountTitle
        {
            get
            {
                if (to_account_id > 0)
                {
                    return Utils.Utils.GetTransferAmountText(from_account_currency, from_amount, to_account_currency,
                        to_amount);
                }

                if (original_currency_id > 0)
                {
                    return Utils.Utils.SetAmountText(original_currency, original_from_amount, from_account_currency,
                        from_amount, true);
                }
                return Utils.Utils.SetAmountText(from_account_currency, from_amount, true);
            }
        }

        [NotMapped]
        public string BalanceTitle
        {
            get
            {
                if (this.to_account_id > 0)
                {
                    return Utils.Utils.SetTransferBalanceText(from_account_currency, from_account_balance, to_account_currency, to_account_balance);
                }
                return Utils.Utils.SetAmountText(from_account_currency, from_account_balance ?? 0, false);
            }
        }

        [NotMapped]
        public bool HasNoCategory => Type != "Transfer" && category_id == 0;
    }
}