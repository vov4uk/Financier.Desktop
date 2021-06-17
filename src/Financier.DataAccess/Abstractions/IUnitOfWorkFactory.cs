namespace Financier.DataAccess.Abstractions
{
    public interface IUnitOfWorkFactory
    {
        public IUnitOfWork CreateUnitOfWork();
    }
}