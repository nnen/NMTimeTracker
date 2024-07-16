using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public class Modifier : ModelBase
    {
        private readonly long m_id;
        private DateTime m_date;
        private TimeSpan m_time;
        private string? m_comment;


        public long Id => m_id;

        public DateTime Date { 
            get => m_date; 
        }

        public TimeSpan Time 
        { 
            get => m_time;
            set => SetProperty(nameof(Time), ref m_time, value);
        }

        public string? Comment
        {
            get => m_comment;
            set => SetProperty(nameof(Comment), ref m_comment, value);
        }


        public Modifier(long id, DateTime date, TimeSpan time, string? comment)
        {
            m_id = id;
            m_date = date;
            m_time = time;
            m_comment = comment;
        }
    }
}
