using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public class MonthModel : ModelBase
    {
        private readonly DayModel[] m_days;
        private TimeSpan? m_time = null;
        private TimeSpan? m_expectedTime = null;


        public TimeSpan Time
        {
            get
            {
                if (!m_time.HasValue)
                {
                    UpdateTime();
                }
                return m_time.Value;
            }
        }

        public TimeSpan ExpectedTime
        {
            get
            {
                if (!m_expectedTime.HasValue)
                {
                    UpdateTime();
                }
                return m_expectedTime.Value;
            }
        }

        private void UpdateTime()
        {
            var time = TimeSpan.Zero;
            var expectedTime = TimeSpan.Zero;
            foreach (var day in m_days)
            {
                time += day.Time;
                expectedTime += day.ExpectedTime;
            }
            m_time = time;
            m_expectedTime = expectedTime;
        }

        public void InvalidateTime()
        {
            m_time = null;
            m_expectedTime = null;
            NotifyPropertyChanged(nameof(Time));
            NotifyPropertyChanged(nameof(ExpectedTime));

        }


        public MonthModel(IList<DayModel> days)
        {
            m_days = days.ToArray();

            foreach (DayModel day in m_days)
            {
                day.PropertyChanged += OnDayPropertyChanged;
            }
        }

        private void OnDayPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DayModel.Time))
            {
                InvalidateTime();
            }
        }
    

        public bool Contains(DateTime date)
        {
            var firstDay = m_days[0].Date;
            return (firstDay.Year == date.Year) && (firstDay.Month == date.Month);
        }
    }
}
