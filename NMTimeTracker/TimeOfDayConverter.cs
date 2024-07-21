using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NMTimeTracker
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimeOfDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            if (parameter is string paramStr)
            {
                if (paramStr.Length > 0)
                {
                    return date.ToString(paramStr);
                }
            }
            return date.ToString("HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Now;
        }
    }


    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToString("D");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(DateTime), typeof(string))]
    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("d MMM");
            }
            else if (value is string str)
            {
                return str;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("ddd");
            }
            else if (value is string str)
            {
                return str;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanConverter : IValueConverter
    {
        private static Regex s_timeSpanRegex = new Regex(@"(?<hours>\d+)h\s*(?<minutes>\d+)m", RegexOptions.Compiled);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan span = (TimeSpan)value;
            int hours = (int)Math.Floor(span.TotalHours);
            int minutes = span.Minutes;
            return $"{hours}h {minutes:00}m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                var m = s_timeSpanRegex.Match(strValue);
                if (m.Success)
                {
                    int hours = int.Parse(m.Groups["hours"].Value);
                    int minutes = int.Parse(m.Groups["minutes"].Value);
                    return new TimeSpan(hours, minutes, 0);
                }
                
                if (TimeSpan.TryParse(strValue, out TimeSpan result))
                {
                    return result;
                }
            }

            return TimeSpan.Zero;
        }
    }
}
