using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using NMTimeTracker.Model;

namespace NMTimeTracker
{
    public class HistoryViewModel : ModelBase
    {
        private DateTime m_selectedDate = DateTime.Today;
        private WeekModel m_selectedWeek;
        private DayModel m_selectedDay;
        private MonthModel m_selectedMonth;
        private Interval? m_selectedInterval;
        private bool m_updating = false;
        
        public DateTime SelectedDate
        {
            get => m_selectedDate;
            set
            {
                if (SetProperty(nameof(SelectedDate), ref m_selectedDate, value))
                {
                    UpdateFromSelectedDate();
                }
            }
        }

        public WeekModel SelectedWeek
        {
            get => m_selectedWeek;
            set => SetProperty(nameof(SelectedWeek), ref m_selectedWeek, value);
        }

        public MonthModel SelectedMonth
        {
            get => m_selectedMonth;
            set => SetProperty(nameof(SelectedMonth), ref m_selectedMonth, value);
        }

        public DayModel SelectedDay
        {
            get => m_selectedDay;
            set
            {
                if (SetProperty(nameof(SelectedDay), ref m_selectedDay, value))
                {
                    UpdateFromSelectedDay();
                }
            }
        }

        public Interval? SelectedInterval
        {
            get => m_selectedInterval;
            set => SetProperty(nameof(SelectedInterval), ref m_selectedInterval, value);
        }

        public ICommand RemoveSelectedIntervalCommand { get; } 


        public HistoryViewModel()
        {
            RemoveSelectedIntervalCommand = new LambdaCommand(this.RemoveSelectedInterval);

            UpdateFromSelectedDate();

            Debug.Assert(m_selectedWeek != null);
            Debug.Assert(m_selectedMonth != null);
            Debug.Assert(m_selectedDay != null);
        }


        public void RemoveSelectedInterval()
        {
            if (SelectedInterval != null)
            {
                var app = App.Current;
                var store = app.Store;
                store?.DeleteInterval(SelectedInterval);
            }
        }

        public void RemoveIntervals(IEnumerable<Interval> intervals)
        {
            var app = App.Current;
            var store = app.Store;
            if (store != null)
            {
                foreach (var interval in intervals)
                {
                    store.DeleteInterval(interval);
                }
            }
        }

        public void RemoveModifiers(IEnumerable<Modifier> modifiers)
        {
            var app = App.Current;
            var store = app.Store;
            if (store != null)
            {
                foreach (var modifier in modifiers)
                {
                    store.DeleteModifier(modifier);
                }
            }
        }

        private void UpdateFromSelectedDate()
        {
            if (m_updating) return;
            m_updating = true;
            try
            {
                var app = App.Current;
                var store = app.Store;
                if (store == null)
                {
                    throw new Exception("Data store is not initialized.");
                }
                var selectedDate = SelectedDate;
                UpdateWeekAndMonth(store, selectedDate);
                SelectedDay = store.GetDay(selectedDate);
            }
            finally 
            { 
                m_updating = false; 
            }
        }

        private void UpdateFromSelectedDay()
        {
            if (m_updating) return;
            m_updating = true;
            try
            {
                var app = App.Current;
                var store = app.Store;
                if (store == null)
                {
                    throw new Exception("Data store is not initialized.");
                }
                var day = SelectedDay;
                if (day != null)
                {
                    UpdateWeekAndMonth(store, day.Date);
                    SelectedDate = day.Date;
                }
            }
            finally
            {
                m_updating = false;
            }
        }
    
        private void UpdateWeekAndMonth(DataStore store, DateTime date)
        {
            if ((SelectedWeek == null) || !SelectedWeek.Contains(date))
            {
                SelectedWeek = store.GetWeek(date, App.Current.Settings.FirstDayOfWeek);
            }
            if ((SelectedMonth == null) || !SelectedMonth.Contains(date))
            {
                SelectedMonth = store.GetMonth(date);
            }
        }
    }
}
