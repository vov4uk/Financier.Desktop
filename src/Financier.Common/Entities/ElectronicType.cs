using System.ComponentModel;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ElectronicType
    {
        [Description("PayPal")]        PAYPAL,
        [Description("Bitcoin")]       BITCOIN,
        [Description("Amazon")]        AMAZON,
        [Description("Ebay")]          EBAY,
        [Description("Google Wallet")] GOOGLE_WALLET,
        [Description("Web Money")]     WEB_MONEY,
        [Description("Yandex Money")]  YANDEX_MONEY,
        [Description("AliPay")]        ALIPAY,
    }
}
