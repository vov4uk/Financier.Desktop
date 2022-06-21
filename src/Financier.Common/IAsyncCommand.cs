using System.Threading.Tasks;

namespace Financier.Common
{
    public interface IAsyncCommand
    {
        Task ExecuteAsync();
        bool CanExecute();
        void RaiseCanExecuteChanged();
    }

    public interface IAsyncCommand<in T>
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
        void RaiseCanExecuteChanged();
    }
}