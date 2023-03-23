using System;
using System.Runtime.InteropServices;

namespace Notebook
{
    public static class Program
    {
        #region Main
        [STAThread]
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
        #endregion
    }

    internal sealed class Win32
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void HideConsoleWindow()
        {
            IntPtr hConsole = GetConsoleWindow();

            if (hConsole != IntPtr.Zero)
                ShowWindow(hConsole, 0); // 0 = SW_HIDE
        }
    }
}
