using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public struct ModifierData
    {
        public long Id;
        public DateTime Date;
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

        public Modifier(long id, DateTime date, TimeSpan time, string? comment)
        {
            m_data.Id = id;
            m_data.Date = date;
            m_data.Time = time;
            m_data.Comment = comment;
        }
    }
}
