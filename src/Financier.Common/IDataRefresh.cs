using Mvvm.Async;

namespace Financier.Common
{
    public interface IDataRefresh
    {
        IAsyncCommand RefreshDataCommand { get; }
    }
}
