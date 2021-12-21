namespace Financier.DataAccess.Abstractions
{
    public interface IFinancierDatabaseFactory
    {
        IFinancierDatabase CreateDatabase();
    }
}
