using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.Wizards
{
    public enum WizardTypes
    {
        [Description("csv")]
        Monobank,
        [Description("pdf")]
        ABank,
        [Description("pdf")]
        Pumb
    }
}
