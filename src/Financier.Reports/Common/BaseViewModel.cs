using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Financier.Reports.Common
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            if (!(property.Body is MemberExpression body))
                throw new NotSupportedException("Invalid expression passed. Only property member should be selected.");
            propertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
        }

        public virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}