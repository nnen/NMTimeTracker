using NMTimeTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NMTimeTracker
{
    public class DayViewModel : ModelBase
    {
        public DayModel Model { get; }

        public DateTime Date => Model.Date;

        public TimeSpan Time => Model.Time;

        public Visibility ModifierIndicatorVisibility
        {
            get
            {
                if (Model.HasModifiers)
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        public DayViewModel(DayModel day)
        {
            Model = day;
            Model.PropertyChanged += (sender, e) =>
            {
                NotifyPropertyChanged(e.PropertyName);
                NotifyPropertyChanged(nameof(ModifierIndicatorVisibility));
            };
        }
    }
}
