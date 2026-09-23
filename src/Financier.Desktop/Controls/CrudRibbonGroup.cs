using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using Financier.Common.Localization;

namespace Financier.Desktop.Controls
{
    [ExcludeFromCodeCoverage]
    public class CrudRibbonGroup : RibbonGroup
    {
        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register(
            nameof(AddCommand), typeof(ICommand), typeof(CrudRibbonGroup));

        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(
            nameof(EditCommand), typeof(ICommand), typeof(CrudRibbonGroup));

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand), typeof(ICommand), typeof(CrudRibbonGroup));

        public static readonly DependencyProperty IsDeleteEnabledProperty = DependencyProperty.Register(
            nameof(IsDeleteEnabled), typeof(bool), typeof(CrudRibbonGroup), new PropertyMetadata(true));

        public CrudRibbonGroup()
        {
            Items.Add(CreateButton(nameof(AddCommand), "add", "IconPlus"));
            Items.Add(CreateButton(nameof(EditCommand), "edit", "IconPencil"));

            var deleteButton = CreateButton(nameof(DeleteCommand), "delete", "IconTrash");
            deleteButton.SetBinding(IsEnabledProperty, new Binding(nameof(IsDeleteEnabled)) { Source = this });
            Items.Add(deleteButton);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        // Some entities' OnDelete is still a stub; disable Delete here rather than in CanExecute.
        public bool IsDeleteEnabled
        {
            get => (bool)GetValue(IsDeleteEnabledProperty);
            set => SetValue(IsDeleteEnabledProperty, value);
        }

        private RibbonButton CreateButton(string commandPropertyName, string labelKey, string iconKey)
        {
            var button = new RibbonButton();
            button.SetBinding(RibbonButton.CommandProperty, new Binding(commandPropertyName) { Source = this });
            button.SetBinding(RibbonButton.LabelProperty, new Binding($"[{labelKey}]") { Source = LocalizationService.Instance });
            button.SetResourceReference(RibbonButton.LargeImageSourceProperty, iconKey);
            return button;
        }
    }
}
