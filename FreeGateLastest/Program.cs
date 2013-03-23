using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace FreeGateLastest
{
    class Program
    {
        static void Main(string[] args)
        {
            var fis = (new DirectoryInfo(System.Environment.CurrentDirectory)).GetFiles("fg*.exe");
            var fi = fis[0];
            var time = fi.LastWriteTime;
            foreach (var tfi in fis)
            {
                if (tfi.LastWriteTime > time)
                {
                    fi = tfi;
                    time = fi.LastWriteTime;
                }
            }
            Process.Start(new ProcessStartInfo() { WorkingDirectory = System.Environment.CurrentDirectory, FileName = fi.Name });
        }
    }
}
