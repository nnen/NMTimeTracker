using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker.Model
{
    public class WeekModel : ModelBase
    {
        private readonly DayModel[] m_days;


        public IList<DayModel> Days => m_days;

        public TimeSpan Time { get; }


        public WeekModel(IList<DayModel> days)
        {
            Debug.Assert(days != null && days.Count == 7);
            m_days = days.ToArray();

            var time = TimeSpan.Zero;
            foreach (var day in m_days)
            {
                time += day.Time;
            }
            Time = time;
        }


        public bool Contains(DateTime date)
        {
            return (date >= m_days[0].Date) && (date <= m_days[m_days.Length - 1].Date);
        }
    }
}
