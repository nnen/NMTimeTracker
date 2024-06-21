using System.ComponentModel;
using System.DirectoryServices;
using System.Media;
using System.Windows;
using System.Windows.Forms;

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private System.Timers.Timer m_timer = new System.Timers.Timer();

        public TimeTracker Tracker
        {
            get
            {
                return App.Current.Tracker;
            }
        }

        public Settings Settings
        {
            get => App.Current.Settings;
        }


        public string TotalTimeString
        {
            get
            {
                TimeSpan span = Tracker.TotalTime;
                return $"{span.Hours}:{span.Minutes:00}:{span.Seconds:00}";
            }
        }


        private NotifyIcon m_notifyIcon;


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            System.Drawing.Icon icon = SystemIcons.Application;
            var iconStream = System.Windows.Application.GetResourceStream(new Uri("AppIcon.ico", UriKind.Relative));
            if (iconStream != null)
            {
                icon = new System.Drawing.Icon(iconStream.Stream);
            }

            m_notifyIcon = new NotifyIcon();
            m_notifyIcon.Icon = icon;
            m_notifyIcon.Text = Title;
            m_notifyIcon.Visible = true;
            m_notifyIcon.DoubleClick += (sender, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.Hide();
                    this.WindowState = WindowState.Minimized;
                }
                else
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                }
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit").Click += (sender, e) => {
                System.Windows.Application.Current.Shutdown();
            };
            m_notifyIcon.ContextMenuStrip = contextMenu;

            m_timer.Interval = 200;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
            m_timer.Elapsed += M_timer_Elapsed;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            e.Cancel = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (m_notifyIcon != null)
            {
                m_notifyIcon.Visible = false;
                m_notifyIcon.Dispose();
            }
            
            base.OnClosed(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }

        private void ForceUpdateTimeText()
        {
            NotifyPropertyChanged("TotalTimeString");
        }

        private void M_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ForceUpdateTimeText();
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            Tracker.StartTime(TimeTrackerEvents.UserStart);
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            Tracker.StopTime(TimeTrackerEvents.UserStop);
            ForceUpdateTimeText();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        private void ShowHistoryWindow()
        {
            var historyWindow = new HistoryWindow();
            historyWindow.Tracker = Tracker;
            historyWindow.Show();
        }

        private void MenuHistory_Click(object sender, RoutedEventArgs e)
        {
            ShowHistoryWindow();
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Settings = App.Current.Settings;
            settingsWindow.ShowDialog();
        }
    }
}