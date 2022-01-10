using Financier.DataAccess.Abstractions;

namespace Financier.DataAccess
{
    public class FinancierDatabaseFactory : IFinancierDatabaseFactory
    {
        public IFinancierDatabase CreateDatabase() => new FinancierDatabase();
    }
}
