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

        private DayViewModel? m_selectedDayViewModel;
        private List<DayViewModel> m_selectedDays;

        private DateTime m_rangeStartDate = Utils.GetStartOfMonth(DateTime.Today);
        private DateTime m_rangeEndDate = DateTime.Today;
        private TimeSpan? m_rangeTime;
        private TimeSpan? m_rangeExpectedTime;
        
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
                    m_selectedDayViewModel = null;
                    NotifyPropertyChanged(nameof(SelectedDayViewModel));

                    UpdateFromSelectedDay();
                }
            }
        }

        public DayViewModel SelectedDayViewModel
        {
            get
            {
                if (m_selectedDayViewModel == null)
                {
                    m_selectedDayViewModel = new DayViewModel(m_selectedDay);
                }
                return m_selectedDayViewModel;
            }
            set
            {
                if (value != null)
                {
                    SelectedDay = value.Model;
                }
            }
        }

        public IEnumerable<DayViewModel> SelectedDaysViewModel
        {
            get
            {
                if (m_selectedDays == null)
                {
                    m_selectedDays = new List<DayViewModel>();
                }
                if ((m_selectedDays.Count == 0) && (m_selectedWeek != null))
                {
                    foreach (var day in m_selectedWeek.Days)
                    {
                        m_selectedDays.Add(new DayViewModel(day));
                    }
                }
                return m_selectedDays;
            }
        }

        public Interval? SelectedInterval
        {
            get => m_selectedInterval;
            set => SetProperty(nameof(SelectedInterval), ref m_selectedInterval, value);
        }

        public DateTime RangeStartDate
        {
            get => m_rangeStartDate;
            set
            {
                if (SetProperty(nameof(RangeStartDate), ref m_rangeStartDate, value))
                    InvalidateRangeTime();
            }
        }

        public DateTime RangeEndDate
        {
            get => m_rangeEndDate;
            set
            {
                if (SetProperty(nameof(RangeEndDate), ref m_rangeEndDate, value))
                    InvalidateRangeTime();
            }
        }

        public TimeSpan RangeTime
        {
            get
            {
                if (!m_rangeTime.HasValue) UpdateRangeTime();
                return m_rangeTime!.Value;
            }
        }

        public TimeSpan RangeExpectedTime
        {
            get
            {
                if (!m_rangeExpectedTime.HasValue) UpdateRangeTime();
                return m_rangeExpectedTime!.Value;
            }
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

                //m_selectedDays?.Clear();
                m_selectedDays = null;
                NotifyPropertyChanged(nameof(SelectedDaysViewModel));
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

                //m_selectedDays?.Clear();
                m_selectedDays = null;
                NotifyPropertyChanged(nameof(SelectedDaysViewModel));
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

        private void UpdateRangeTime()
        {
            var store = App.Current.Store;
            if (store == null)
            {
                m_rangeTime = TimeSpan.Zero;
                m_rangeExpectedTime = TimeSpan.Zero;
                return;
            }

            var time = TimeSpan.Zero;
            var expectedTime = TimeSpan.Zero;
            var start = m_rangeStartDate.Date;
            var end = m_rangeEndDate.Date;

            if (start <= end)
            {
                var current = start;
                while (current <= end)
                {
                    var day = store.GetDay(current);
                    time += day.Time;
                    expectedTime += day.ExpectedTime;
                    current = current.AddDays(1);
                }
            }

            m_rangeTime = time;
            m_rangeExpectedTime = expectedTime;
        }

        private void InvalidateRangeTime()
        {
            m_rangeTime = null;
            m_rangeExpectedTime = null;
            NotifyPropertyChanged(nameof(RangeTime));
            NotifyPropertyChanged(nameof(RangeExpectedTime));
        }
    }
}
