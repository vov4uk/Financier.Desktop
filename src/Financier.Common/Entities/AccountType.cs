using System.ComponentModel;
using Financier.Common.Attribute;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AccountType
    {
        [LocalizedDescription("account_type_cash")]
        CASH,

        [LocalizedDescription("account_type_bank")]
        BANK,

        [LocalizedDescription("account_type_credit_card")]
        CREDIT_CARD,

        [LocalizedDescription("account_type_debit_card")]
        DEBIT_CARD,

        [LocalizedDescription("account_type_asset")]
        ASSET,

        [LocalizedDescription("account_type_liability")]
        LIABILITY,

        [LocalizedDescription("account_type_electronic")]
        ELECTRONIC,

        [LocalizedDescription("account_type_other")]
        OTHER,
    }
}
