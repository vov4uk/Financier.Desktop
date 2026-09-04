using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers
{
    public interface IDialogWrapper
    {
        object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null)
            where T : System.Windows.Controls.UserControl, new();

        string OpenFileDialog(string fileExtention);

        string SaveFileDialog(string fileExtention, string defaultPath = "");

        object ShowWizard(WizardBaseVM context);

        bool ShowMessageBox(string text, string caption, bool yesNoButtons = false);
    }
}
