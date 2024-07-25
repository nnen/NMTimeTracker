using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NMTimeTracker.View
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            
            try
            {
                var enumInfo = GetEnumInfo(value.GetType());
                return enumInfo.GetName(value);
            }
            catch
            {
            }
            
            return value.ToString() ?? String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private static readonly Dictionary<Type, EnumInfo> sm_enumInfos = new();

        private static EnumInfo GetEnumInfo(Type enumType)
        {
            if (!sm_enumInfos.TryGetValue(enumType, out var enumInfo))
            {
                enumInfo = new EnumInfo(enumType);
                sm_enumInfos[enumType] = enumInfo;
            }
            return enumInfo;
        }


        private class EnumInfo
        {
            private readonly Dictionary<object, string> m_valueToName = new();

            public EnumInfo(Type enumType)
            {
                var members = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                if (members != null)
                {
                    foreach (var member in members)
                    {
                        var value = member.GetValue(null);
                        if (value == null)
                        {
                            continue;
                        }

                        var descAttr = member.GetCustomAttribute<DescriptionAttribute>();
                        if (descAttr != null)
                        {
                            m_valueToName[value] = descAttr.Description;
                        }
                        else
                        {
                            m_valueToName[value] = member.Name;
                        }
                    }
                }
            }

            public string GetName(object value)
            {
                if (m_valueToName.TryGetValue(value, out var name))
                {
                    return name;
                }
                return value.ToString() ?? String.Empty;
            }
        }
    }
}
