namespace Financier.Common
{
    public interface IDataRefresh
    {
        IAsyncCommand RefreshDataCommand { get; }
    }
}
