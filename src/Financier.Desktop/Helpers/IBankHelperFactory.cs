using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers
{
    public interface IBankHelperFactory
    {
        IBankHelper CreateBankHelper(WizardTypes bank);
    }
}
