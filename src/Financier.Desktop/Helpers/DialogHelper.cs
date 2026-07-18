using System.Diagnostics;
using System.Windows;
using Financier.Common.Localization;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards;
using Application = System.Windows.Application;

namespace Financier.Desktop.Helpers
{

    public class DialogHelper : IDialogWrapper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null!)
            where T : System.Windows.Controls.UserControl, new()
        {
            object result = null!;
            var dialog = new Window
            {
                Content = new T() { DataContext = context },
                ResizeMode = ResizeMode.NoResize,
                Height = height,
                Width = width,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Title = title ?? "Financier",
                ShowInTaskbar = Debugger.IsAttached,
                Language = System.Windows.Markup.XmlLanguage.GetLanguage(LocalizationService.Instance.CurrentCulture.IetfLanguageTag)
            };
            context.RequestCancel += (_, _) =>
            {
                dialog.Close();
                Logger.Info($"{typeof(T).Name} dialog cancel clicked");
            };
            context.RequestSave += (sender, _) =>
            {
                result = sender!;
                dialog.Close();
                Logger.Info($"{typeof(T).Name} dialog save clicked");
            };
            dialog.ShowDialog();
            return result!;
        }

        public object ShowWizard(WizardBaseVM context)
        {
            bool save = false;
            object result = null!;
            WizardWindow dialog = new WizardWindow()
            {
                Language = System.Windows.Markup.XmlLanguage.GetLanguage(LocalizationService.Instance.CurrentCulture.IetfLanguageTag)
            };

            context.RequestClose += (sender, args) =>
            {
                dialog.Close();
                save = args;
                result = sender!;
            };
            dialog.DataContext = context;
            dialog.ShowDialog();

            return save ? result! : null!;
        }

        public string OpenFileDialog(string fileExtention)
        {
            var openFileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog()
            {
                Multiselect = false,
                Filter = $"{fileExtention} files (*.{fileExtention})|*.{fileExtention}"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return string.Empty;
        }

        public string SaveFileDialog(string fileExtention, string defaultPath = "")
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog
            {
                Filter = $"{fileExtention} files (*.{fileExtention})|*.{fileExtention}",
                FileName = defaultPath
            };
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        public bool ShowMessageBox(string text, string caption, bool yesNoButtons = false)
        {
            if (yesNoButtons)
            {
                var result = System.Windows.MessageBox.Show(text, caption, MessageBoxButton.YesNo);
                return result == MessageBoxResult.Yes;
            }

            MessageBox.Show(text, caption);
            return true;

        }
    }
}
