using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NMTimeTracker.Model
{
    public class GenerateHistoryViewModel : ModelBase
    {
        public int NumberOfDays { get; set; }
        public int IntervalsPerDay { get; set; }
        public int ModifiersPerDay { get; set; }


        private bool m_isGenerating = false;
        private DateTime m_day;
        private int m_dayCounter = 0;
        private float m_progress = 0.0f;

        public bool IsGenerating
        {
            get => m_isGenerating;
            private set
            {
                if (SetProperty(nameof(IsGenerating), ref m_isGenerating, value))
                {
                    NotifyPropertyChanged(nameof(ProgressVisibility));
                }
            }
        }

        public Visibility ProgressVisibility
        {
            get => IsGenerating ? Visibility.Visible : Visibility.Collapsed;
        }

        public float Progress 
        { 
            get => m_progress; 
            private set => SetProperty(nameof(Progress), ref m_progress, value); 
        }


        public void GenerateStep()
        {
            var app = App.Current;
            var store = app.Store;

            if (m_dayCounter == 0)
            {
                m_day = DateTime.Today;
            }
            else if (m_dayCounter >= NumberOfDays)
            {
                Progress = 1.0f;
                return;
            }

            var start = m_day.Date + TimeSpan.FromHours(8);

            for (int j = 0; j < IntervalsPerDay; ++j)
            {
                var end = start + TimeSpan.FromHours(1);
                store.CreateInterval(start, TimeTrackerEvents.UserStart, end, TimeTrackerEvents.UserStop);
                start = end + TimeSpan.FromMinutes(15);
            }

            m_day -= TimeSpan.FromDays(1);
            m_dayCounter++;

            Progress = (float)m_dayCounter / (float)NumberOfDays;
        }

        public void Generate()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                IsGenerating = true;
                m_dayCounter = 0;
                while (m_dayCounter < NumberOfDays)
                {
                    GenerateStep();
                }
                IsGenerating = false;
            };
            worker.RunWorkerAsync();
        }
    }
}
