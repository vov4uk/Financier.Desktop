using System;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BankHelperFactory : IBankHelperFactory
    {
        public IBankHelper CreateBankHelper(WizardTypes bank)
        {
            switch (bank)
            {
                case WizardTypes.Monobank : return new MonobankHelper();
                case WizardTypes.ABank: return new ABankHelper();
                case WizardTypes.Pumb: return new PumbHelper();
                default:
                    throw new NotSupportedException("Bank not found");
            }
        }
    }
}
