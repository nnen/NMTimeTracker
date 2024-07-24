using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using NMTimeTracker.Model;

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
        
        UnexpectedStop,
    }
    
    public class TimeTracker : ModelBase
    {
        public TimeTracker(DataStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            m_store = store;

            m_today = m_store.GetDay(DateTime.Today);
            //m_intervals = [.. m_store.GetIntervalsInDay(DateTime.Today)];
            //m_readonlyIntervals = new(m_intervals);
            
            m_timer.Interval = 60 * 1000;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
            m_timer.Elapsed += (sender, e) =>
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (m_currentIntervalId.HasValue)
                    {
                        m_today.UpdateLastInterval(m_currentIntervalId.Value, DateTime.Now);
                        m_store.UpdateInterval(m_today.LastInterval);
                    }

                    //if (m_currentInterval != null)
                    //{
                    //    m_currentInterval.End = DateTime.Now;
                    //    m_store.UpdateInterval(m_currentInterval);
                    //}
                });

                UpdateToday();
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


        public DayModel Today
        {
            get => m_today;
            set
            {
                SetProperty(nameof(Today), ref m_today, value);
                NotifyPropertyChanged(nameof(TotalTime));
            }
        }

        private void UpdateToday()
        {
            if (m_today.Date != DateTime.Today)
            {
                Today = m_store.GetDay(DateTime.Today);
            }
        }

        public long? CurrentIntervalId
        {
            get => m_currentIntervalId;
            set
            {
                bool changed = (m_currentIntervalId != value);
                m_currentIntervalId = value;
                if (changed)
                {
                    NotifyPropertyChanged(nameof(CurrentIntervalId));
                    NotifyPropertyChanged(nameof(CurrentInterval));
                    NotifyPropertyChanged(nameof(IsTimeRunning));
                }
            }
        }

        public Interval? CurrentInterval
        {
            get
            {
                if (m_today == null)
                {
                    return null;
                }

                if (!m_currentIntervalId.HasValue)
                {
                    return null;
                }

                var interval = m_today.LastInterval;
                if ((interval == null) || (interval.Id != m_currentIntervalId.Value))
                {
                    return null;
                }
                
                return interval;
            }
        }

        public bool IsTimeRunning => m_currentIntervalId.HasValue;

        public void StartTime(TimeTrackerEvents reason)
        {
            UpdateToday();

            if (CurrentInterval != null)
            {
                return;
            }

            var now = DateTime.Now;
            var interval = m_store.CreateInterval(now, reason);
            CurrentIntervalId = interval.Id;
            return;
        }

        public void StopTime(TimeTrackerEvents reason)
        {
            var interval = CurrentInterval;
            if (interval == null)
            {
                return;
            }
            
            interval.End = DateTime.Now;
            interval.EndReason = reason;
            m_store.UpdateInterval(interval);
            
            CurrentIntervalId = null;
        }

        public Modifier AddModifier(DateTime date, TimeSpan time, string? comment = null)
        {
            return m_store.CreateModifier(date, time, comment);
        }

        public Modifier AddModifier(TimeSpan time, string? comment = null)
        {
            return m_store.CreateModifier(DateTime.Today, time, comment);
        }

        public TimeSpan TotalTime
        {
            get
            {
                return m_today.Time;
            }
        }

        public void UpdateTotalTime()
        {
            if (m_currentIntervalId.HasValue)
            {
                m_today.UpdateLastInterval(m_currentIntervalId.Value, DateTime.Now);
                NotifyPropertyChanged(nameof(TotalTime));
            }
        }

        private DataStore m_store;
        private System.Timers.Timer m_timer = new System.Timers.Timer();

        private DayModel m_today;
        private long? m_currentIntervalId;


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
