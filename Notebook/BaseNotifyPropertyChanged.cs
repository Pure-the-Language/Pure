using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notebook
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
