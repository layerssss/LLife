using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
namespace ADSLDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.accs.Clear();
            var chars = new List<char>();
            for (var i = 0; i < this.textBox4.Text.Length; i++)
            {
                chars.Add(this.textBox4.Text[i]);
            }
            this.log("任务已就绪");
            var tmp = new List<int>();
            if (this.textBox7.Text.Length > this.numericUpDown1.Value)
            {
                this.textBox7.Text = this.textBox7.Text.Substring(this.textBox7.Text.Length - (int)this.numericUpDown1.Value);
            }
            this.textBox7.Text = this.textBox7.Text.PadLeft((int)this.numericUpDown1.Value, chars[0]);
            for (var i = (int)this.numericUpDown1.Value; i >0 ; i--)
            {
                tmp.Add(chars.IndexOf(this.textBox7.Text[i-1]));
            }
            var cur = tmp.ToArray();
            cur[0]--;
            lock (this)
            {
                for (int i = 0; i < cur.Length; i++)
                {
                    if (cur[i] == chars.Count - 1)
                    {
                        cur[i] = 0;
                    }
                    else
                    {
                        cur[i]++;
                        var sb = new StringBuilder(this.textBox2.Text);
                        for (int j = cur.Length; j > 0; j--)
                        {
                            sb.Append(chars[cur[j - 1]]);
                        }
                        sb.Append(this.textBox6.Text);
                        accs.Add(sb.ToString());
                        i = -1;
                    }
                }
                this.all = this.accs.Count;
            }
            this.pass = this.textBox1.Text;
            this.link=this.textBox5.Text;
        }
        string pass;
        List<string> accs = new List<string>();
        int all = 0;
        string link;
        void tt()
        {
            while (true)
            {
                if (!accs.Any())
                {
                    this.log("任务已完成");
                    break;
                }
                var i = new Random().Next(this.accs.Count);
                //var i = 0;
                var acc=this.accs[i];
                this.accs.RemoveAt(i);
                //this.log(acc);
                var p = Process.Start(new ProcessStartInfo()
                {
                    FileName = "rasdial.exe",
                    Arguments = this.link + " " + acc + " " + pass,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                p.WaitForExit();
                this.log("账号：" + acc + "；结果：" + p.ExitCode.ToString());
                if (p.ExitCode == 0)
                {
                    p.StartInfo.Arguments = this.link + " /DISCONNECT";
                }
                p.Start();
                p.WaitForExit();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.t.Abort();
            this.log("任务已中止");
        }
        Thread t;
        string logs;
        void log(string line)
        {
            this.logs = DateTime.Now.ToString("HH:mm:ss")+":" +line + "\r\n" + this.logs;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                this.button2.Enabled
                    = !(this.textBox1.Enabled =
                        this.textBox2.Enabled =
                        this.textBox4.Enabled =
                        this.numericUpDown1.Enabled =
                        this.button1.Enabled =
                        this.button4.Enabled =
                        t == null || !t.IsAlive);
                this.textBox3.Text = this.logs;
                if (this.accs.Count == 0)
                {
                    this.progressBar1.Visible = false;
                }
                else
                {
                    this.progressBar1.Value = this.all - this.accs.Count;
                    this.progressBar1.Maximum = this.all;
                    this.progressBar1.Visible = true;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t!=null&&t.IsAlive)
            {
                this.t.Abort();
            }
            var writer = new System.IO.BinaryWriter(System.IO.File.Create("ASDLDetector.cfg"));
            foreach (Control ctl in this.groupBox1.Controls)
            {
                if (ctl is TextBox)
                {
                    writer.Write(ctl.Name);
                    writer.Write((ctl as TextBox).Text);
                } if (ctl is NumericUpDown)
                {
                    writer.Write(ctl.Name);
                    writer.Write((ctl as NumericUpDown).Value);
                }
            }
            writer.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var reader = new System.IO.BinaryReader(System.IO.File.OpenRead("ASDLDetector.cfg"));
                while (reader.PeekChar() != -1)
                {
                    var name=reader.ReadString();
                    if (this.groupBox1.Controls[name] is TextBox)
                    {
                        (this.groupBox1.Controls[name] as TextBox).Text = reader.ReadString();
                    }
                    else
                    {
                        (this.groupBox1.Controls[name] as NumericUpDown).Value = reader.ReadDecimal();
                    }
                }
                reader.Close();
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.logs = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.log("任务已开始");
            this.t = new Thread(this.tt);
            this.t.Start();
        }
    }
}
