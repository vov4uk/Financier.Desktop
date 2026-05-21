using Financier.Desktop.Pages.Controls;
using System;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Core;

namespace Financier.Desktop.Helpers
{
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier, string message, MessageOptions messageOptions)
        {
            Application.Current.Dispatcher.Invoke(() => { notifier.Notify<CustomNotification>(() => new CustomNotification(message, messageOptions)); });
        }
    }
}
