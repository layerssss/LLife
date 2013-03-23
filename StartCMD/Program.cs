using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
namespace StartCMD
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
            foreach (var arg in args)
            {
                Process.Start("cmd.exe"," /k \"cd "+arg.Remove(arg.LastIndexOf('\\'))+"\"");
            }
        }
    }
}
