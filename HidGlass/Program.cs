using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace HidGlass
{
    static class Program
    {

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(
            string lpClassName,
            string lpWindowName
        );
        public const int HWND_TOPMOST = -1;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(
            int hwnd,
            int hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int wFlags
        );
        public delegate bool CallBack(int hwnd, int lParam);
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(
            int hwnd,
            System.Text.StringBuilder lpString,
            int cch
        ); 
        [DllImport("user32.Dll ")]
        public static extern int EnumWindows(CallBack x, int y);
        private static bool mycallbak(int hWnd, int lParam)
        {
            if (hWnd == 0)
                return false;
            var str = new System.Text.StringBuilder(255);
            GetWindowText(hWnd, str, 255);
            if (str.ToString().Trim() == "放大镜")
            {
                //SetWindowPos(hWnd, 1024, 0, 0, 0, 0, 0x80 | 0x1 | 0x4);
                var bound = Screen.PrimaryScreen.Bounds;
                ShowWindow(hWnd,SW_HIDE);
                return true;
            }
            return true;
        }
        public const int SW_HIDE = 0;
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(
            int hwnd,
            int nCmdShow
        );
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                EnumWindows(new CallBack(mycallbak), 0);
                System.Threading.Thread.Sleep(100);
            }

        }
    }
}
