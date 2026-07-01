using System.ComponentModel;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CardIssuer
    {
        [Description("Visa")]          VISA,
        [Description("Visa Electron")] VISA_ELECTRON,
        [Description("Mastercard")]    MASTERCARD,
        [Description("Maestro")]       MAESTRO,
        [Description("Cirrus")]        CIRRUS,
        [Description("AMEX")]          AMEX,
        [Description("JCB")]           JCB,
        [Description("Diners Club")]   DINERS,
        [Description("Discover")]      DISCOVER,
        [Description("UnionPay")]      UNIONPAY,
        [Description("NETS")]          NETS,
        [Description("RuPay")]         RUPAY,
        [Description("Mir")]           MIR,
        [Description("Default")]       DEFAULT,
    }
}
