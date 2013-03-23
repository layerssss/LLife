using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
namespace FFKiller
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var proc = Process.GetProcessesByName("firefox");
            MessageBox.Show(proc[0].MainWindowHandle.ToInt32().ToString());
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1_MouseDoubleClick(null, null);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.notifyIcon1_MouseDoubleClick(null, null);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label2.Text = trackBar1.Value.ToString() + "秒";

            var proces = Process.GetProcessesByName("firefox");
            var needToBeKilled = proces.Any();
            foreach (var proc in proces)
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    needToBeKilled = false;
                }
            }
            if (needToBeKilled)
            {
                timeOut += timer1.Interval;
            }
            else
            {
                timeOut = 0;
            }
            if (timeOut >= trackBar1.Value * 1000)
            {
                this.kill();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                var config = File.ReadAllLines("FFKiller.cfg");
                trackBar1.Value = Convert.ToInt32(config[0]);
                checkBox1.Checked = Convert.ToBoolean(config[1]);
                this.notifyIcon1_MouseDoubleClick(null, null);
            }
            catch { }
        }
        int timeOut = 0;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllLines("FFKiller.cfg", new[] { trackBar1.Value.ToString(),checkBox1.Checked.ToString() });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.退出ToolStripMenuItem_Click(null, null);
        }

        private void 马上杀ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.kill();
        }
        void kill()
        {
            try
            {
                var proc = Process.GetProcessesByName("firefox");
                var msg = "已杀死进程:";
                foreach(var tp in proc)
                {
                    if (tp.MainWindowHandle == IntPtr.Zero)
                    {
                        msg += tp.Id + ";";
                        tp.Kill();
                    }
                }
                if(msg.Length!=6)
                {
                    if (this.checkBox1.Checked)
                    {
                        this.notifyIcon1.ShowBalloonTip(3000, "火狐进程自动杀", msg, ToolTipIcon.Info);
                    }
                }
            }
            catch { }
            timeOut = 0;
        }


        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                (new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    this.Invoke(new MethodInvoker(() => { this.ShowInTaskbar = false; }));
                })).Start();
            }
        }
    }
}
