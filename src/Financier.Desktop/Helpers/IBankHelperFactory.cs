namespace Financier.Desktop.Helpers
{
    public interface IBankHelperFactory
    {
        IBankHelper CreateBankHelper(string bank);
    }
}
