using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LSync
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var f = new MainForm();
            LCmd.CmdLineHandler.SafeHandleArgs(typeof(MainForm).GetMethod("Init"), f, args);
            Application.Run(f);
        }
    }
}
