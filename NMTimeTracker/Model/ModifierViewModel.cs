using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NMTimeTracker.Model
{
    public class ModifierViewModel : ModelBase
    {
        private Modifier? m_modifier = null;
        private DateTime m_date = DateTime.Today;
        private DateTime? m_dateTo = null;
        private ModifierKinds m_kind = ModifierKinds.WorkedTime;
        private TimeSpan m_time = TimeSpan.Zero;
        private string m_comment = string.Empty;

        public bool IsAdding => m_modifier == null;
        public bool IsModifying => !IsAdding;

        public Visibility AddButtonVisiblity => IsAdding ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ApplyButtonVisibility => IsModifying ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DateToVisibility => IsAdding ? Visibility.Visible : Visibility.Collapsed;

        public string WindowTitle => IsAdding ? "Add Modifier" : "Edit Modifier";

        public bool IsRange => m_dateTo.HasValue && m_dateTo.Value.Date > m_date.Date;
        

        public Modifier? Modifier 
        { 
            get => m_modifier;
            set
            {
                if (SetProperty(nameof(Modifier), ref m_modifier, value))
                {
                    if (value != null)
                    {
                        Date = value.Date;
                        DateTo = null;
                        Kind = value.Kind;
                        Time = value.Time;
                        Comment = value.Comment ?? string.Empty;
                    }

                    NotifyPropertyChanged(nameof(AddButtonVisiblity));
                    NotifyPropertyChanged(nameof(ApplyButtonVisibility));
                    NotifyPropertyChanged(nameof(DateToVisibility));
                }
            }
        }

        public DateTime Date
        {
            get => m_date;
            set
            {
                if (SetProperty(nameof(Date), ref m_date, value))
                    NotifyPropertyChanged(nameof(Description));
            }
        }

        public DateTime? DateTo
        {
            get => m_dateTo;
            set
            {
                if (SetProperty(nameof(DateTo), ref m_dateTo, value))
                {
                    NotifyPropertyChanged(nameof(IsRange));
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public ModifierKinds Kind
        {
            get => m_kind;
            set
            {
                SetProperty(nameof(Kind), ref m_kind, value);
                NotifyPropertyChanged(nameof(IsWorkedTime));
                NotifyPropertyChanged(nameof(IsExpectedTime));
            }
        }

        public bool IsWorkedTime
        {
            get => Kind == ModifierKinds.WorkedTime;
            set => Kind = value ? ModifierKinds.WorkedTime : ModifierKinds.ExpectedTime;
        }

        public bool IsExpectedTime
        {
            get => Kind == ModifierKinds.ExpectedTime;
            set => Kind = value ? ModifierKinds.ExpectedTime : ModifierKinds.WorkedTime;
        }

        public TimeSpan Time 
        { 
            get => m_time;
            set
            {
                if (SetProperty(nameof(Time), ref m_time, value))
                {
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public string Comment 
        {
            get => m_comment;
            set => SetProperty(nameof(Comment), ref m_comment, value); 
        }


        public string Description
        {
            get
            {
                var time = Time;
                bool subtract = time.TotalSeconds < 0;
                if (subtract) time = time.Negate();
                string verb = subtract ? "Subtract" : "Add";
                string amount = $"{(int)Math.Floor(time.TotalHours)}h {time.Minutes:00}m";

                if (IsRange)
                {
                    int days = (int)(m_dateTo!.Value.Date - m_date.Date).TotalDays + 1;
                    return $"{verb} {amount} for each of {days} day(s).";
                }
                return $"{verb} {amount}.";
            }
        }


        public void AddModifier()
        {
            var app = App.Current;
            var endDate = IsRange ? m_dateTo!.Value.Date : m_date.Date;
            var current = m_date.Date;
            while (current <= endDate)
            {
                app.Tracker?.AddModifier(current, Kind, Time, Comment);
                current = current.AddDays(1);
            }
        }

        public void ApplyChanges()
        {
            if (m_modifier != null)
            {
                var app = App.Current;
                var store = app.Store;

                m_modifier.Date = Date;
                m_modifier.Kind = Kind;
                m_modifier.Time = Time;
                m_modifier.Comment = Comment;
                
                store?.UpdateModifier(m_modifier);
            }
        }
    }
}
