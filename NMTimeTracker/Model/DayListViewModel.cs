using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using NMTimeTracker.Model;

namespace NMTimeTracker
{
    public class DayListViewModel : ObservableCollection<DayViewModel>
    {
        public void SetWeek(WeekModel week)
        {
            ClearItems();
            foreach (var day in week.Days)
            {
                Add(new DayViewModel(day));
            }
        }
    }
}
