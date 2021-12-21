﻿using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Wizards;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace Financier.Desktop.Helpers
{
    public interface IDialogWrapper
    {
        object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null)
            where T : System.Windows.Controls.UserControl, new();

        string OpenFileDialog(string fileExtention);

        string SaveFileDialog(string fileExtention, string defaultPath = "");

        bool ShowWizard(WizardBaseVM context);
    }

    public class DialogHelper : IDialogWrapper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null)
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
                Logger.Info($"{typeof(T).Name} dialog cancel clicked");
            };
            context.RequestSave += (sender, _) =>
            {
                result = sender;
                dialog.Close();
                Logger.Info($"{typeof(T).Name} dialog save clicked");
            };
            dialog.ShowDialog();
            return result;
        }

        public bool ShowWizard(WizardBaseVM context)
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

        public string OpenFileDialog(string fileExtention)
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

        public string SaveFileDialog(string fileExtention, string defaultPath = "")
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
