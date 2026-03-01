using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public enum ModifierKinds
    {
        [Description("Worked time")]
        WorkedTime,
        [Description("Expected time")]
        ExpectedTime,
    }

    public struct ModifierData
    {
        public long Id;
        public DateTime Date;
        public ModifierKinds Kind;
        public TimeSpan Time;
        public string? Comment;
    }

    public class Modifier : ModelBase
    {
        private ModifierData m_data;


        public long Id => m_data.Id;

        public DateTime Date { 
            get => m_data.Date; 
            set => SetProperty(nameof(Date), ref m_data.Date, value);
        }

        public ModifierKinds Kind
        {
            get => m_data.Kind;
            set => SetProperty(nameof(Kind), ref m_data.Kind, value);
        }

        public TimeSpan Time 
        { 
            get => m_data.Time;
            set => SetProperty(nameof(Time), ref m_data.Time, value);
        }

        public string? Comment
        {
            get => m_data.Comment;
            set => SetProperty(nameof(Comment), ref m_data.Comment, value);
        }


        public Modifier(in ModifierData data)
        {
            m_data = data;
        }

        public Modifier(long id, DateTime date, ModifierKinds kind, TimeSpan time, string? comment)
        {
            m_data.Id = id;
            m_data.Date = date;
            m_data.Kind = kind;
            m_data.Time = time;
            m_data.Comment = comment;
        }
    }
}
