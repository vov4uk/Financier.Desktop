using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers.BankHelper
{
    public interface IBankHelperFactory
    {
        IBankHelper CreateBankHelper(WizardTypes bank);
    }
}
