using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace CloseMonitor
{
    class Program
    {
        private const uint WM_SYSCOMMAND = 0x112; //系统消息
        private const int SC_MONITORPOWER = 0xF170; //关闭显示器的系统命令
        private const int MonitorPowerOff = 2; //2为PowerOff, 1为省电状态，-1为开机
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);//广播消息，所有顶级窗体都会接收

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        static void Main(string[] args)
        {
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MonitorPowerOff);
        }
    }


}
