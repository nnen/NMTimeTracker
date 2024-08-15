using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class MainClass
    {
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOW = 5;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);


        [STAThread]
        public static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + App.Guid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    var windowHandle = FindWindow(null, "NM Time Tracker");
                    if (windowHandle != 0)
                    {
                        ShowWindow(windowHandle, SW_SHOWNORMAL);
                        Thread.Sleep(200);
                        SetForegroundWindow(windowHandle);
                        return;
                    }
                    
                    MessageBox.Show("NM Time Tracker is already running.", "NM Time Tracker");
                    return;
                }
                
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
