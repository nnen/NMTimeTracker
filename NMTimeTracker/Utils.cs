using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public static class Utils
    {
        public static void SetProperty<T>(this PropertyChangedEventHandler self, object sender, ref T value, T newValue, string propertyName)
        {
            value = newValue;
            if (self != null)
            {
                self(sender, new PropertyChangedEventArgs(propertyName));
            }
        }


        public static DateTime GetStartOfWeek(DateTime aWeekDay, DayOfWeek startOfWeek)
        {
            int diff = (7 + (aWeekDay.DayOfWeek - startOfWeek)) % 7;
            return aWeekDay.AddDays(-1 * diff).Date;
        }

        public static DateOnly GetStartOfMonth(DateOnly aMonthDay)
        {
            return new DateOnly(aMonthDay.Year, aMonthDay.Month, 1);
        }

        public static DateTime GetStartOfMonth(DateTime aMonthDay)
        {
            return new DateTime(aMonthDay.Year, aMonthDay.Month, 1);
        }
    }
}
