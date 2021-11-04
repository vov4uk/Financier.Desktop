namespace Financier.DataAccess.Data
{
    public interface IActive : IIdentity
    {
        bool IsActive { get; set; }

        string Title { get; set; }
    }
}
