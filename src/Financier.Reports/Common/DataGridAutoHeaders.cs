using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace fcrd
{
    internal class DataGridAutoHeaders : DataGrid
    {
        public DataGridAutoHeaders()
        {
            this.AutoGenerateColumns = true;
            this.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(this.DataGridAutoHeadersAutoGeneratingColumn);
        }

        private void DataGridAutoHeadersAutoGeneratingColumn(
          object sender,
          DataGridAutoGeneratingColumnEventArgs e)
        {
            string proprtyDisplayName = this.GetProprtyDisplayName(e.PropertyDescriptor);
            string name = e.PropertyType.Name;
            if (!string.IsNullOrEmpty(proprtyDisplayName))
                e.Column.Header = proprtyDisplayName;
            else
                e.Column.Visibility = Visibility.Hidden;
            if (!(name == typeof(long).Name) && !(name == typeof(long?).Name) && !(name == typeof(double).Name) && !(name == typeof(double?).Name))
                return;
            e.Column.CellStyle = new Style(typeof(DataGridCell));
            e.Column.CellStyle.Setters.Add((SetterBase)new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right));
        }

        private string GetProprtyDisplayName(object p)
        {
            PropertyDescriptor propertyDescriptor = p as PropertyDescriptor;
            string proprtyDisplayName = (string)null;
            if (propertyDescriptor != null && propertyDescriptor.Attributes[typeof(DisplayNameAttribute)] is DisplayNameAttribute attribute && attribute != DisplayNameAttribute.Default)
                proprtyDisplayName = attribute.DisplayName;
            return proprtyDisplayName;
        }
    }
}