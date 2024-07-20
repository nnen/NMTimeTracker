using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public class WeekModel : ModelBase
    {
        private readonly DayModel[] m_days;
        private TimeSpan? m_time = null;


        public IList<DayModel> Days => m_days;

        public TimeSpan Time 
        { 
            get
            {
                if (!m_time.HasValue)
                {
                    var time = TimeSpan.Zero;
                    foreach (var day in m_days)
                    {
                        time += day.Time;
                    }
                    m_time = time;
                }
                return m_time.Value;
            }
        }

        public void InvalidateTime()
        {
            m_time = null;
            NotifyPropertyChanged(nameof(Time));
        }


        public WeekModel(IList<DayModel> days)
        {
            Debug.Assert(days != null && days.Count == 7);
            m_days = days.ToArray();

            foreach (var day in m_days)
            {
                day.PropertyChanged += OnDayPropertyChanged;
            }
        }

        public bool Contains(DateTime date)
        {
            return (date >= m_days[0].Date) && (date <= m_days[m_days.Length - 1].Date);
        }


        private void OnDayPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DayModel.Time))
            {
                InvalidateTime();
            }
        }
    }
}
