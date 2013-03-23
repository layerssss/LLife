using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LVMWareService
{
    public partial class LVMWareService : ServiceBase
    {
        string[] vmx;
        string vpath;
        public LVMWareService()
        {
            InitializeComponent();
            this.vmx = System.Configuration.ConfigurationManager.AppSettings["VmxPaths"].Split('|');
            this.vpath=System.Configuration.ConfigurationManager.AppSettings["VMWarePath"].TrimEnd('\\')+'\\';
        }

        protected override void OnStart(string[] args)
        {
            foreach (var v in vmx)
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = vpath + "vmrun.exe",
                    Arguments = "-T  ws  start \"" + v + "\"",
                    WorkingDirectory=vpath
                };
                p.Start();
                p.WaitForExit();
            }
            System.Threading.Thread.Sleep(1000);
            foreach(var vmp in Process.GetProcessesByName("vmware-vmx")){
                vmp.PriorityClass = ProcessPriorityClass.RealTime;
            }
        }

        protected override void OnStop()
        {
            foreach (var v in vmx)
            {
                Process.Start(vpath + "vmrun.exe", "suspend \"" + v + "\"").WaitForExit();
            }
        }
    }
}
