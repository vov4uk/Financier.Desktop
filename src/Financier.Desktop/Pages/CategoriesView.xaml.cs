using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Financier.Common.Model;
using Financier.Desktop.ViewModel;

namespace Financier.Desktop.Views
{
    [ExcludeFromCodeCoverage]
    public partial class CategoriesView : UserControl
    {
        public CategoriesView() => InitializeComponent();

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is CategoriesVM vm)
                vm.SelectedValue = e.NewValue as CategoryTreeModel;
        }

        private void CategoriesTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (item != null)
                item.IsSelected = true;
        }

        private static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                if (obj is T t) return t;
                if (obj is FrameworkContentElement)
                {
                    obj = ((FrameworkContentElement)obj).Parent;
                    continue;
                }
                else if (obj is Visual || obj is Visual3D)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                    continue;
                }
                else {
                    return null;
                }
            }
            return null;
        }
    }
}
