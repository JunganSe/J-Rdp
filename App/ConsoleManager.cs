using NLog;
using System.Runtime.InteropServices;

namespace App
{
    public class ConsoleManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();



        public static void SetVisibility(bool show)
        {
            if (show)
                OpenConsole();
            else
                CloseConsole();
        }

        private static void OpenConsole()
        {
            AllocConsole();
            _logger.Info("Opened console.");
        }

        private static void CloseConsole()
        {
            FreeConsole();
            _logger.Info("Closed console.");
        }
    }
}
