using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using HardwareHelperLib;
namespace MouseSwitcher
{
    static class Program
    {
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
                var hwh=new HH_Lib();
                string[] HardwareList = hwh.GetAll();
                var found = false;
                foreach (var hard in HardwareList)
                {
                    if (hard.EndsWith("mouse"))
                    {
                        found = true;
                    }
                }
                foreach (var hard in HardwareList)
                {
                    if (hard.EndsWith("TouchPad"))
                    {
                        hwh.SetDeviceState(new[]{hard}, !found);
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}
