using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Financier.Common.Attribute;
using Financier.Common.Localization;

namespace Financier.Common.Controls
{
    [ExcludeFromCodeCoverage]
    public class DataGridAutoHeaders : DataGrid
    {
        public DataGridAutoHeaders()
        {
            AutoGenerateColumns = true;
            AutoGeneratingColumn += DataGridAutoHeadersAutoGeneratingColumn!;
        }

        private void DataGridAutoHeadersAutoGeneratingColumn(
          object sender,
          DataGridAutoGeneratingColumnEventArgs e)
        {
            string propertyDisplayName = GetPropertyDisplayName(e.PropertyDescriptor);
            string propertyStyle = GetPropertyCellTemplateName(e.PropertyDescriptor);
            string name = e.PropertyType.Name;

            if (!string.IsNullOrEmpty(propertyStyle))
            {
                var column = new DataGridTemplateColumn();
                column.CellTemplate = FindResource(propertyStyle) as DataTemplate;
                if (!string.IsNullOrEmpty(propertyDisplayName))
                {
                    column.Header = propertyDisplayName;
                }
                else
                {
                    column.Visibility = Visibility.Collapsed;
                }

                e.Column = column;
            }
            else
            {
                if (!string.IsNullOrEmpty(propertyDisplayName))
                {
                    e.Column.Header = LocalizationService.Instance[propertyDisplayName];
                }
                else
                {
                    e.Column.Visibility = Visibility.Collapsed;
                }

                if (name != typeof(long).Name && name != typeof(long?).Name && name != typeof(double).Name && name != typeof(double?).Name)
                    return;
                e.Column.CellStyle = new Style(typeof(DataGridCell));
                e.Column.CellStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            }
        }

        private static string GetPropertyDisplayName(object p)
        {
            PropertyDescriptor propertyDescriptor = (p as PropertyDescriptor)!;
            string propertyDisplayName = null;
            if (propertyDescriptor != null && propertyDescriptor.Attributes[typeof(DisplayNameAttribute)] is DisplayNameAttribute attribute && attribute != DisplayNameAttribute.Default)
            {
                propertyDisplayName = attribute.DisplayName;
            }
            return propertyDisplayName;
        }

        private static string GetPropertyCellTemplateName(object p)
        {
            PropertyDescriptor propertyDescriptor = (p as PropertyDescriptor)!;
            string proprtyDisplayName = null;
            if (propertyDescriptor != null && propertyDescriptor.Attributes[typeof(CellTemplateAttribute)] is CellTemplateAttribute attribute)
            {
                proprtyDisplayName = attribute.Key;
            }
            return proprtyDisplayName;
        }
    }
}
