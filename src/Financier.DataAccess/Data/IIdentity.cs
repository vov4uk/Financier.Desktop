namespace Financier.DataAccess.Data
{
    public interface IIdentity
    {
        int Id { get; set; }

        long UpdatedOn { get; set; }
    }
}
