using System.IO;
using System.Windows;

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static readonly Guid Guid = new Guid(
            "AB09E3AB-8C10-43EC-AB7A-D1F5A4261558"
        );


        new public static App Current
        {
            get => (App)System.Windows.Application.Current;
        }

        public Settings Settings { get; } = Settings.Load();

        public DataStore? Store { get; private set; }


        public TimeTracker? Tracker { get; private set; }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Tracker != null)
            {
                Tracker.StopTime(TimeTrackerEvents.AppExit);
                Tracker.StopListening();
            }

            if (Store != null)
            {
                Store.Dispose();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Store = DataStore.Create();
            Tracker = new TimeTracker(Store);

            Tracker.StartListening();
            if (Tracker.StartOnLaunch)
            {
                Tracker.StartTime(TimeTrackerEvents.AppStartup);
            }
        }


        public static string GetAppDataDirectoryPath(bool createDir = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = typeof(DataStore).Assembly.GetName().Name;
            var dirPath = Path.Combine(appData, appName);
            if (createDir)
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
    }
}
