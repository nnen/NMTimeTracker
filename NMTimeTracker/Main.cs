using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class MainClass
    {
        [STAThread]
        public static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + App.Guid))
            {
                if (!mutex.WaitOne(0, false))
                {
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
