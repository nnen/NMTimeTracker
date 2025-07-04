﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NMTimeTracker.Model;

namespace NMTimeTracker
{
    public class Settings : ModelBase
    {
        private bool m_startOnLaunch = true;
        private bool m_stopOnLock = true;
        private bool m_startOnUnlock = true;
        private bool m_stopOnScreenSaver = true;
        // private DayOfWeek m_firstDayOfWeek = DayOfWeek.Monday;
        private bool m_mainWindowAlwaysOnTop = false;

        public bool StartOnLaunch
        {
            get => m_startOnLaunch;
            set
            {
                SetProperty(nameof(StartOnLaunch), ref m_startOnLaunch, value);
            }
        }

        public bool StopOnLock
        {
            get => m_stopOnLock;
            set
            {
                SetProperty(nameof(StopOnLock), ref m_stopOnLock, value);
            }
        }

        public bool StartOnUnlock
        {
            get => m_startOnUnlock;
            set
            {
                SetProperty(nameof(StartOnUnlock), ref m_startOnUnlock, value);
            }
        }

        public bool StopOnScreenSaver
        {
            get => m_stopOnScreenSaver;
            set
            {
                SetProperty(nameof(StopOnScreenSaver), ref m_stopOnScreenSaver, value);
            }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                var ci = CultureInfo.CurrentCulture;
                return ci.DateTimeFormat.FirstDayOfWeek;
            }
        }

        public bool MainWindowAlwaysOnTop
        {
            get => m_mainWindowAlwaysOnTop;
            set
            {
                SetProperty(nameof(MainWindowAlwaysOnTop), ref m_mainWindowAlwaysOnTop, value);
            }
        }


        public int HoursPerBusinessDay => 8;


        public static string? GetFilePath()
        { 
            var appData = App.GetAppDataDirectoryPath(true);
            if (appData == null)
            {
                return null;
            }
            return Path.Combine(appData, "settings.json");
        }
        
        public void Save()
        {
            var filePath = GetFilePath();
            if (filePath == null)
            {
                throw new InvalidOperationException("Cannot save settings without a file path.");
            }
            var jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, jsonString);
        }

        public static Settings Load()
        {
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                var result = new Settings();
                result.Save();
                return result;
            }
            var jsonString = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<Settings>(jsonString);
            if (config == null)
            {
                return new Settings();
            }
            return config;
        }
    }
}
