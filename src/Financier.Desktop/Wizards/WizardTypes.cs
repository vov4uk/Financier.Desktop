using System.ComponentModel;

namespace Financier.Desktop.Wizards
{
    public enum WizardTypes
    {
        [Description("csv")]
        Monobank,
        [Description("pdf")]
        ABank,
        [Description("xlsx")]
        ABankExcel,
        [Description("xlsx")]
        Privat,
        [Description("pdf")]
        Pumb,
        [Description("pdf")]
        Pireus,
        [Description("pdf")]
        Pko
    }
}
