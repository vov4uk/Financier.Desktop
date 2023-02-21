using System;

namespace Financier.Desktop.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BankHelperFactory : IBankHelperFactory
    {
        public IBankHelper CreateBankHelper(string bank)
        {
            switch (bank)
            {
                case "Monobank": return new MonobankHelper();
                case "A-Bank": return new ABankHelper();
                case "Raiffeisen": return new RaiffeisenHelper();
                default:
                    throw new NotSupportedException("Bank not found");
            }
        }
    }
}
