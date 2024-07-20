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
        private TimeSpan m_time = TimeSpan.Zero;
        private string m_comment = string.Empty;

        public bool IsAdding => m_modifier == null;
        public bool IsModifying => !IsAdding;


        public Visibility AddButtonVisiblity => IsAdding ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ApplyButtonVisibility => IsModifying ? Visibility.Visible : Visibility.Collapsed;

        
        public string WindowTitle => IsAdding ? "Add Modifier" : "Edit Modifier";
        

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
                        Time = value.Time;
                        Comment = value.Comment ?? string.Empty;
                    }

                    NotifyPropertyChanged(nameof(AddButtonVisiblity));
                    NotifyPropertyChanged(nameof(ApplyButtonVisibility));
                }
            }
        }

        public DateTime Date 
        {
            get => m_date;
            set => SetProperty(nameof(Date), ref m_date, value);
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
                if (time.TotalSeconds < 0)
                {
                    time = time.Negate();
                    return $"Subtract {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} seconds.";
                }
                return $"Add {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} seconds.";
            }
        }


        public void AddModifier()
        {
            var app = App.Current;
            app.Tracker?.AddModifier(Date, Time, Comment);
        }

        public void ApplyChanges()
        {
            if (m_modifier != null)
            {
                var app = App.Current;
                var store = app.Store;

                m_modifier.Date = Date;
                m_modifier.Time = Time;
                m_modifier.Comment = Comment;
                
                store?.UpdateModifier(m_modifier);
            }
        }
    }
}
