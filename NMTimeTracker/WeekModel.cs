using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class DayModel : ModelBase
    {
        private readonly ObservableCollection<Interval> m_intervals = new ObservableCollection<Interval>();
        private readonly ReadOnlyObservableCollection<Interval> m_roIntervals;
        
        public DateTime Date { get; }
        public ReadOnlyObservableCollection<Interval> Intervals => m_roIntervals;

        public TimeSpan Time { get; }
        
        public DayModel(DateTime date, IEnumerable<Interval>? intervals = null)
        {
            m_roIntervals = new ReadOnlyObservableCollection<Interval>(m_intervals);
            
            Date = date;
            
            if (intervals != null)
            {
                foreach (var interval in intervals)
                {
                    m_intervals.Add(interval);
                }
            }

            var time = TimeSpan.Zero;
            foreach (var interval in Intervals)
            {
                time += interval.Span;
            }
            Time = time;
        }


        public void UpdateIntervals(IEnumerable<Interval> intervals)
        {
            m_intervals.Clear();
            foreach (var interval in intervals)
            {
                m_intervals.Add(interval);
            }
        }
    }
    
    
    public class WeekModel : ModelBase
    {
        private readonly DayModel[] m_days;
        
        
        public IList<DayModel> Days => m_days;

        public TimeSpan Time { get; }


        public WeekModel(IList<DayModel> days)
        {
            Debug.Assert((days != null) && (days.Count == 7));
            m_days = days.ToArray();

            var time = TimeSpan.Zero;
            foreach (var day in m_days)
            {
                time += day.Time;
            }
            Time = time;
        }
    }
}
