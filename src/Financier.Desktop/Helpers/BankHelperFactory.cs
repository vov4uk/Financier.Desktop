using System;

namespace Financier.Desktop.Helpers
{
    public class BankHelperFactory : IBankHelperFactory
    {
        public IBankHelper CreateBankHelper(string bank)
        {
            switch (bank)
            {
                case "Monobank": return new MonobankHelper();
                case "A-Bank": return new ABankHelper();
                default:
                    throw new NotSupportedException("Bank not found");
            }
        }
    }
}
