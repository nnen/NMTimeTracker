namespace NMTimeTracker.Model
{
    public class Interval : ModelBase
    {
        private long m_id;

        private DateTime m_end;

        public long Id => m_id;

        public DateTime Start { get; set; }
        public TimeOnly StartTimeOfDay
        {
            get => TimeOnly.FromTimeSpan(Start.TimeOfDay);
            set
            {
                Start = new DateTime(DateOnly.FromDateTime(Start), value);
            }
        }
        public TimeTrackerEvents StartReason { get; set; }

        public DateTime End
        {
            get => m_end;
            set
            {
                m_end = value;
                NotifyPropertyChanged(nameof(End));
                NotifyPropertyChanged(nameof(Span));
                NotifyPropertyChanged(nameof(SpanText));
            }
        }
        public TimeOnly EndTimeOfDay
        {
            get => TimeOnly.FromTimeSpan(End.TimeOfDay);
            set
            {
                End = new DateTime(DateOnly.FromDateTime(End), value);
            }
        }
        public TimeTrackerEvents EndReason { get; set; }

        public TimeSpan Span
        {
            get
            {
                return End - Start;
            }
        }

        public string SpanText
        {
            get
            {
                return (End - Start).ToString();
            }
        }

        public Interval(long id, DateTime start, TimeTrackerEvents startReason)
        {
            m_id = id;
            Start = start;
            StartReason = startReason;
            End = start;
            EndReason = TimeTrackerEvents.None;
        }
        public Interval(long id, DateTime start, TimeTrackerEvents startReason, DateTime end, TimeTrackerEvents endReason)
        {
            m_id = id;
            Start = start;
            StartReason = startReason;
            End = end;
            EndReason = endReason;
        }
    }
}
