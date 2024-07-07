using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class HistoryViewModel : ModelBase
    {
        private DateTime m_selectedDate = DateTime.Today;
        private WeekModel m_selectedWeek;
        private DayModel m_selectedDay;
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

        public HistoryViewModel()
        {
            UpdateFromSelectedDate();
        }

        private void UpdateFromSelectedDate()
        {
            if (m_updating) return;
            m_updating = true;
            try
            {
                var app = App.Current;
                var store = app.Store;
                var selectedDate = SelectedDate;
                SelectedWeek = store.GetWeek(selectedDate, app.Settings.FirstDayOfWeek);
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

                var day = SelectedDay;
                if (day != null)
                {
                    SelectedWeek = store.GetWeek(day.Date, app.Settings.FirstDayOfWeek);
                    SelectedDate = day.Date;
                }
            }
            finally
            {
                m_updating = false;
            }
        }
    }
}
