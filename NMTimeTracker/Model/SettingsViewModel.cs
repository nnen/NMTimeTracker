using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NMTimeTracker.Model
{
    public class SettingsViewModel : ModelBase
    {
        public Settings Settings
        {
            get;
            set;
        } = App.Current.Settings;

        public string VersionString
        {
            get
            {
#if (DEBUG)
                var config = "DEBUG";
#else
                var config = "RELEASE";
#endif
                var str = $"{Assembly.GetExecutingAssembly().GetName().Version} ({config})";
                return str;
            }
        }

        public string SettingsFilePath
        {
            get => Settings.GetFilePath();
        }

        public string DatabaseFilePath
        {
            get => DataStore.GetDatabaseFilePath();
        }


        public bool DevOptionsEnabled 
        { 
            get
            {
#if (DEBUG)
                return true;
#else
                return false;
#endif
            }
        }

        public Visibility DevOptionsVisible => DevOptionsEnabled ? Visibility.Visible : Visibility.Collapsed;

        public int SelectedFirstDayOfWeek
        {
            get => (int)Settings.FirstDayOfWeek;
            set => Settings.FirstDayOfWeek = (DayOfWeek)value;
        }
    }
}
