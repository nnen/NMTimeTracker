using NMTimeTracker.View;
using System.ComponentModel;
using System.DirectoryServices;
using System.Media;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

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

        public TimeTracker? Tracker
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
                if (Tracker == null)
                {
                    return "0:00:00";
                }
                Tracker.UpdateTotalTime();
                TimeSpan span = Tracker.TotalTime;
                return $"{span.Hours}:{span.Minutes:00}:{span.Seconds:00}";
            }
        }


        private NotifyIcon m_notifyIcon;
        private System.Drawing.Icon m_iconNormal = SystemIcons.Application;
        private System.Drawing.Icon m_iconRunning = SystemIcons.Application;


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            var normalStream = System.Windows.Application.GetResourceStream(new Uri("AppIcon.ico", UriKind.Relative));
            if (normalStream != null)
                m_iconNormal = new System.Drawing.Icon(normalStream.Stream);

            var runningStream = System.Windows.Application.GetResourceStream(new Uri("AppIconRunning.ico", UriKind.Relative));
            if (runningStream != null)
                m_iconRunning = new System.Drawing.Icon(runningStream.Stream);

            m_notifyIcon = new NotifyIcon();
            m_notifyIcon.Icon = m_iconNormal;

            if (Tracker != null)
                Tracker.PropertyChanged += Tracker_PropertyChanged;
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
                    this.WindowState = WindowState.Normal;
                    this.Show();
                }
            };

            var contextMenu = new ContextMenuStrip();

            var pauseResumeItem = (ToolStripMenuItem)contextMenu.Items.Add("Pause");
            contextMenu.Opening += (sender, e) =>
            {
                pauseResumeItem.Text = (Tracker?.IsTimeRunning == true) ? "Pause" : "Resume";
            };
            pauseResumeItem.Click += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (Tracker?.IsTimeRunning == true)
                        Tracker.StopTime(TimeTrackerEvents.UserStop);
                    else
                        Tracker?.StartTime(TimeTrackerEvents.UserStart);
                });
            };

            contextMenu.Items.Add("Add Modifier...").Click += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var window = new NewModifierWindow();
                    window.ShowDialog();
                });
            };

            contextMenu.Items.Add("Show history...").Click += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ShowHistoryWindow();
                });
            };

            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit").Click += (sender, e) =>
            {
                System.Windows.Application.Current.Shutdown();
            };
            m_notifyIcon.ContextMenuStrip = contextMenu;

            m_timer.Interval = 200;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
            m_timer.Elapsed += M_timer_Elapsed;

            UpdateIcons();
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

        private void Tracker_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimeTracker.IsTimeRunning))
                Dispatcher.InvokeAsync(UpdateIcons);
        }

        private void UpdateIcons()
        {
            var icon = (Tracker?.IsTimeRunning == true) ? m_iconRunning : m_iconNormal;
            m_notifyIcon.Icon = icon;
            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            Tracker?.StartTime(TimeTrackerEvents.UserStart);
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            Tracker?.StopTime(TimeTrackerEvents.UserStop);
            ForceUpdateTimeText();
        }

        private void Button_Modifier(object sender, RoutedEventArgs e)
        {
            var addModifierWindow = new NewModifierWindow();
            addModifierWindow.ShowDialog();
        }

        private void Button_History(object sender, RoutedEventArgs e)
        {
            ShowHistoryWindow();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        private void ShowHistoryWindow()
        {
            var historyWindow = new HistoryWindow();
            //historyWindow.Tracker = Tracker;
            historyWindow.Show();
        }

        private void MenuHistory_Click(object sender, RoutedEventArgs e)
        {
            ShowHistoryWindow();
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            //settingsWindow.Settings = App.Current.Settings;
            settingsWindow.ShowDialog();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (Settings.MainWindowAlwaysOnTop)
            {
                this.Topmost = true;
                this.Activate();
            }
            else
            {
                this.Topmost = false; 
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}