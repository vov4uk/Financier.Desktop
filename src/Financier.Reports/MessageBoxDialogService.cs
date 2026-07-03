namespace Financier.Reports
{
    internal sealed class MessageBoxDialogService : IDialogService
    {
        public static readonly IDialogService Instance = new MessageBoxDialogService();

        private MessageBoxDialogService() { }

        public void ShowMessage(string message) =>
            System.Windows.MessageBox.Show(message);
    }
}
