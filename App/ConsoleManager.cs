using NLog;
using System.Runtime.InteropServices;

namespace App
{
    public class ConsoleManager
    {
        private const string _consoleTitle = "J-Rdp log";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);



        public static void SetVisibility(bool show)
        {
            if (show)
                OpenConsole();
            else
                CloseConsole();
        }

        private static void OpenConsole()
        {
            bool isSuccess = AllocConsole();
            if (!isSuccess)
            {
                _logger.Warn("Failed to open console.");
                return;
            }

            SetConsoleTitle(_consoleTitle);

            // Redirect standard output and error streams to the console
            var consoleOut = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(consoleOut);
            Console.SetError(consoleOut);

            _logger.Info("Opened console.");
        }

        private static void CloseConsole()
        {
            bool isSuccess = FreeConsole();
            if (isSuccess)
                _logger.Info("Closed console.");
            else
                _logger.Warn("Failed to close console.");
        }
    }
}
