using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public static class SystemUtils
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, ref bool lpvParam, int fWinIni);

        public static bool IsScreenSaverRunning()
        {
            const int SPI_GETSCREENSAVERRUNNING = 114;
            bool isRunning = false;

            if (!SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0))
            {
                return false;
            }

            return isRunning;
        }
    }
}
