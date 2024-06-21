using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class WindowBase : System.Windows.Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        protected bool SetProperty<T>(ref T value, T newValue, string name)
        {
            bool changed = !object.Equals(value, newValue);
            value = newValue;
            if (changed)
            {
                NotifyPropertyChanged(name);
            }
            return changed;
        }
    }
}
