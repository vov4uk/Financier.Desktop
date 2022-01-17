using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace fcrd
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            if (!(property.Body is MemberExpression body))
                throw new NotSupportedException("Invalid expression passed. Only property member should be selected.");
            propertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
        }

        public virtual void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}