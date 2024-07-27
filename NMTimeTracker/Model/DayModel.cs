using System.Collections.ObjectModel;

namespace NMTimeTracker.Model
{
    public class DayModel : ModelBase
    {
        private readonly ObservableCollectionEx<Interval> m_intervals = new ObservableCollectionEx<Interval>();
        private readonly ReadOnlyObservableCollection<Interval> m_roIntervals;
        
        private readonly ObservableCollectionEx<Modifier> m_modifiers = new ObservableCollectionEx<Modifier>();
        private ReadOnlyObservableCollection<Modifier>? m_roModifiers;
        
        private TimeSpan? m_time;

        public DateTime Date { get; }
        public ReadOnlyObservableCollection<Interval> Intervals => m_roIntervals;

        public Interval? LastInterval
        {
            get
            {
                if (m_intervals.Count == 0)
                {
                    return null;
                }
                return m_intervals[m_intervals.Count - 1];
            }
        }

        public ReadOnlyObservableCollection<Modifier> Modifiers
        {
            get
            {
                if (m_roModifiers == null)
                {
                    m_roModifiers = new ReadOnlyObservableCollection<Modifier>(m_modifiers);
                }
                return m_roModifiers;
            }
        }


        public TimeSpan Time
        {
            get
            {
                if (!m_time.HasValue)
                {
                    var time = TimeSpan.Zero;
                    var dayStart = Date;
                    var dayEnd = dayStart.AddDays(1);
                    foreach (var interval in Intervals)
                    {
                        time += interval.GetOverlap(dayStart, dayEnd);
                    }
                    foreach (var modifier in m_modifiers)
                    {
                        time += modifier.Time;
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


        public DayModel(DateTime date, IEnumerable<Interval>? intervals = null, IEnumerable<Modifier>? modifiers = null)
        {
            m_roIntervals = new ReadOnlyObservableCollection<Interval>(m_intervals);
            
            Date = date;

            if (intervals != null)
            {
                m_intervals.AddRange(intervals);
            }

            if (modifiers != null)
            {
                m_modifiers.AddRange(modifiers);
            }
        }


        public void UpdateIntervals(IEnumerable<Interval> intervals)
        {
            m_intervals.SuppressNotifications();
            try
            {
                m_intervals.Clear();
                m_intervals.AddRange(intervals);
            }
            finally
            {
                m_intervals.ResumeNotifications();
            }
            InvalidateTime();
        }

        public void UpdateModifiers(IEnumerable<Modifier> modifiers)
        {
            m_intervals.SuppressNotifications();
            try
            {
                m_modifiers.Clear();
                m_modifiers.AddRange(modifiers);
            }
            finally
            {
                m_intervals.ResumeNotifications();
            }
            InvalidateTime();
        }

        public bool UpdateLastInterval(long id, DateTime end, TimeTrackerEvents endReason = TimeTrackerEvents.StillRunning)
        {
            if (m_intervals.Count == 0)
            {
                return false;
            }

            var last = m_intervals[m_intervals.Count - 1];
            if (last.Id != id)
            {
                return false;
            }

            last.End = end;
            last.EndReason = endReason;
            InvalidateTime();
            return true;
        }


        public void OnIntervalDeleted(Interval interval)
        {
            for (int i = 0; i < m_intervals.Count; ++i)
            {
                if (m_intervals[i].Id == interval.Id)
                {
                    m_intervals.RemoveAt(i);
                    InvalidateTime();
                    break;
                }
            }
        }

        public void OnModifierAdded(Modifier modifier)
        {
            m_modifiers.Add(modifier);
            InvalidateTime();
        }

        internal void OnIntervalAdded(Interval interval)
        {
            m_intervals.Add(interval);
            InvalidateTime();
            NotifyPropertyChanged(nameof(LastInterval));
        }

        public void OnModifierDeleted(Modifier modifier)
        {
            for (int i = 0; i < m_modifiers.Count; ++i)
            {
                if (m_modifiers[i].Id == modifier.Id)
                {
                    m_modifiers.RemoveAt(i);
                    InvalidateTime();
                    break;
                }
            }
        }
    }
}
