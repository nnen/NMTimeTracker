using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(string propertyName, ref T value, T newValue)
        {
            bool changed = !Equals(value, newValue);
            value = newValue;
            if (changed)
            {
                NotifyPropertyChanged(propertyName);
            }
            return changed;
        }
    }
}
