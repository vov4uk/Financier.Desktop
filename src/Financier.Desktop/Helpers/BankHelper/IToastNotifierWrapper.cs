using System;
using System.Windows;
using Financier.Desktop.Pages.Controls;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Financier.Desktop.Helpers.BankHelper
{
    public interface IToastNotifierWrapper
    {
        void ShowMessage(string message);
        void ShowWarning(string message);
    }

    public class ToastNotifierWrapper : IToastNotifierWrapper
    {
        private readonly Notifier notifier;
        public ToastNotifierWrapper()
        {
            this.notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(10),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.Width = 250;
            });
        }
        public void ShowMessage(string message)
        {
            Application.Current?.Dispatcher.Invoke(() => { notifier?.Notify(() => new CustomNotification(message, new MessageOptions { ShowCloseButton = true })); });
        }
        public void ShowWarning(string message)
        {
            Application.Current?.Dispatcher.Invoke(() => { notifier?.ShowWarning(message, new MessageOptions { ShowCloseButton = true }); });
        }
    }
}
