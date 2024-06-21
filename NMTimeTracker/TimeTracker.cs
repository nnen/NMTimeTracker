using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace NMTimeTracker
{
    public enum TimeTrackerEvents
    {
        None,
        
        AppStartup,
        AppExit,
        SessionLock,
        SessionUnlock,
        UserStart,
        UserStop,
    }

    public class Interval : INotifyPropertyChanged
    {
        private long m_id;

        private DateTime m_end;

        public long Id => m_id;

        public DateTime Start { get; set; }
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


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    public class TimeTracker : INotifyPropertyChanged
    {
        public TimeTracker(DataStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            m_store = store;
            m_intervals = [.. m_store.GetIntervalsInDay(DateTime.Today)];
            m_readonlyIntervals = new(m_intervals);
            
            m_timer.Interval = 60 * 1000;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
            m_timer.Elapsed += (sender, e) =>
            {
                if (m_currentInterval != null)
                {
                    m_store.UpdateInterval(m_currentInterval);
                }
            };
        }


        public bool StartOnLaunch
        {
            get => App.Current.Settings.StartOnLaunch;
        }
        public bool StartOnUnlock 
        {
            get => App.Current.Settings.StartOnUnlock;
        }
        public bool StopOnLock 
        {
            get => App.Current.Settings.StopOnLock;
        }

        public void StartListening()
        {
            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        public void StopListening()
        {
            Microsoft.Win32.SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
            case Microsoft.Win32.SessionSwitchReason.SessionLock:
                if (StopOnLock)
                {
                    StopTime(TimeTrackerEvents.SessionLock);
                }
                break;

            case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
                if (StartOnUnlock)
                {
                    StartTime(TimeTrackerEvents.SessionUnlock);
                }
                break;
            }

        }


        public ReadOnlyObservableCollection<Interval> Intervals => m_readonlyIntervals;

        public Interval? CurrentInterval
        {
            get => m_currentInterval;
            set
            {
                bool changed = !object.ReferenceEquals(m_currentInterval, value);
                m_currentInterval = value;
                if (changed)
                {
                    NotifyPropertyChanged(nameof(CurrentInterval));
                    NotifyPropertyChanged(nameof(IsTimeRunning));
                }
            }
        }

        public bool IsTimeRunning => (m_currentInterval != null);

        public void StartTime(TimeTrackerEvents reason)
        {
            m_closedIntervalsSpan = null;

            if (m_currentInterval != null)
            {
                return;
            }

            var now = DateTime.Now;
            var interval = m_store.CreateInterval(now, reason);
            m_intervals.Add(interval);
            CurrentInterval = interval;
            return;
        }

        public void StopTime(TimeTrackerEvents reason)
        {
            m_closedIntervalsSpan = null;

            var interval = CurrentInterval;
            CurrentInterval = null;
            
            if (interval == null)
            {
                return;
            }
            
            interval.End = DateTime.Now;
            interval.EndReason = reason;
            m_store.UpdateInterval(interval);
        }

        public TimeSpan TotalTime
        {
            get
            {
                TimeSpan sum = TimeSpan.Zero;

                if (m_currentInterval != null)
                {
                    m_currentInterval.End = DateTime.Now;
                }

                foreach (var interval in m_intervals)
                {
                    sum += interval.Span;
                }

                return sum;

                //if (!m_closedIntervalsSpan.HasValue)
                //{
                //    TimeSpan sum = TimeSpan.Zero;

                //    foreach (var interval in m_intervals) 
                //    {
                //        if ( interval.IsClosed)
                //        {
                //            sum += interval.Span;
                //        }
                //    }

                //    m_closedIntervalsSpan = sum;
                //}

                //if ((m_currentInterval == null) || m_currentInterval.IsClosed)
                //{
                //    return m_closedIntervalsSpan.Value;
                //}
                //
                //return m_closedIntervalsSpan.Value + (DateTime.Now - m_currentInterval.Start);
            }
        }

        private DataStore m_store;
        private System.Timers.Timer m_timer = new System.Timers.Timer();

        private readonly ObservableCollection<Interval> m_intervals;
        private readonly ReadOnlyObservableCollection<Interval> m_readonlyIntervals;
        private TimeSpan? m_closedIntervalsSpan;
        private Interval? m_currentInterval;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
