using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace Financier.Desktop.Helpers
{
    public static class DialogHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null)
            where T : System.Windows.Controls.UserControl, new()
        {
            object result = null;
            var dialog = new Window
            {
                Content = new T() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = height,
                Width = width,
                Title = title ?? "Financier",
                ShowInTaskbar = Debugger.IsAttached
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info($"{nameof(T)} dialog close");
            };
            context.RequestSave += (sender, _) =>
            {
                result = sender;
                dialog.Close();
                Logger.Info($"{nameof(T)} dialog save");
            };
            dialog.ShowDialog();
            return result;
        }

        public static bool ShowWizard(WizardBaseVM context)
        {
            bool result = false;
            WizardWindow dialog = new WizardWindow();

            context.RequestClose += (o, args) =>
            {
                dialog.Close();
                result = args;
            };
            dialog.DataContext = context;
            dialog.ShowDialog();

            return result;
        }

        public static string OpenFileDialog(string fileExtention)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = $"{fileExtention} files (*.{fileExtention})|*.{fileExtention}"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return string.Empty;
        }

        public static string SaveFileDialog(string fileExtention, string defaultPath = "")
        {
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = $"{fileExtention} files (*.{fileExtention})|*.{fileExtention}",
                FileName = defaultPath
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
    }
}
