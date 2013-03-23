using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
namespace LVOASyncer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.syncThread = new Thread(this.threadStart);
            this.syncThread.Start();
            foreach (Control c in tabPage1.Controls)
            {
                c.Enabled = false;
            }
            tabControl1.SelectedTab = tabPage2;
        }
        Thread syncThread;
        private void button4_Click(object sender, EventArgs e)
        {
            syncThread.Abort();
            try
            {
                wc.CancelAsync();
            }
            catch { }
            foreach (Control c in tabPage1.Controls)
            {
                c.Enabled = true;
            }
            tabControl1.SelectedTab = tabPage1;
        }
        WebClient wc = new WebClient();
        void threadStart()
        {
            while (true)
            {
                var ready = true;
                try
                {
                    var sdPath = this.setTarget;
                    var drvs=DriveInfo.GetDrives();
                    var found=false;
                    foreach (var drv in drvs)
                    {
                        if (sdPath.ToUpper().StartsWith(drv.RootDirectory.FullName.ToUpper()))
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        throw (new Exception("未插入储存卡"));
                    }
                    File.WriteAllText("TEST", sdPath + "LVOASyncerTest.txt");
                    File.Delete(sdPath + "LVOASyncerTest.txt");
                    statTarget = "储存卡状态：就绪";
                }
                catch (Exception ex)
                {
                    statTarget = "储存卡状态：[ERR:" + ex.Message + "]";
                    ready = false;
                }
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                var titles = new List<string>();
                var links = new List<string>();
                try
                {
                    this.statNet = "网络状态：未知";
                    if (this.setIsSelected[0])
                    {
                        this.fetchLinks("http://www.51voa.com/sp.xml", titles, links, wc);
                        this.statNet = "网络状态：就绪";
                    }
                    if (this.setIsSelected[1])
                    {
                        this.fetchLinks("http://www.51voa.com/st.xml", titles, links, wc);
                        this.statNet = "网络状态：就绪";
                    }
                    if (this.setIsSelected[2])
                    {
                        this.fetchLinks("http://www.51voa.com/en.xml", titles, links, wc);
                        this.statNet = "网络状态：就绪";
                    }
                }
                catch (Exception ex)
                {
                    statNet = "网络状态：[ERR" + ex.Message + "]";
                    ready = false;
                }
                if (ready)
                {
                    var need = this.setMax - Directory.GetFiles(this.setTarget, "[VOA*.mp3").Length;
                    this.newLog("同步开始");
                    while (need > 0)
                    {
                        if (!links.Any())
                        {
                            break;
                        }
                        var link = links.First();
                        var title = titles.First();
                        var time = DateTime.Parse(title.Substring(1, title.IndexOf(']') - 1));
                        title = "[VOA" + time.ToString("MMdd") + title.Substring(title.IndexOf(']')) + ".mp3";
                        title = title.Replace("<", "");
                        title = title.Replace(">", "");
                        title = title.Replace("?", "");
                        title = title.Replace("\\", "");
                        title = title.Replace("*", "");
                        title = title.Replace(":", "");
                        title = title.Replace("\"", "");
                        title = title.Replace("|", "");
                        title = title.Replace("/", "");
                        links.RemoveAt(0);
                        titles.RemoveAt(0);
                        if (File.Exists("Cache\\"+title))
                        {
                            continue;
                        }
                        var str = wc.DownloadString(link);
                        str = str.Substring(str.IndexOf("Player(\"") + "Player(\"".Length);
                        var i = str.IndexOf("\");");
                        var url = "http://down.51voa.com" + str.Remove(i);
                        this.newLog("正在下载" + title);
                        progBusy = true;
                        wc.DownloadFileAsync(new Uri(url), "Cache\\" + title);

                        while (this.progBusy) { Thread.Sleep(100); }
                        try
                        {
                            TagLib.File f = TagLib.File.Create("Cache\\" + title);
                            f.Tag.Album = "VOA" + DateTime.Now.ToString("-MM-dd");
                            f.Tag.Performers = new[] { "VOA" };
                            f.Tag.Title = title;
                            f.Save();

                        }
                        catch (Exception ex)
                        {
                            this.newLog("下载" + title + "时发生错误：" + ex.Message);
                        }
                        File.Copy("Cache\\" + title, this.setTarget + title);
                        this.newLog("已下载" + title);
                        need--;
                    }
                    this.newLog("同步完成");
                }
                Thread.Sleep(60000);
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progAll = e.TotalBytesToReceive;
            this.progNow = e.BytesReceived;
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.progBusy = false;
        }
        bool progBusy = false;
        long progAll = 0;
        long progNow = 0;
        void fetchLinks(string url, List<string> titles, List<string> links, WebClient webClient)
        {
            var str = webClient.DownloadString(url);
            var i = 0;
            while ((i = str.IndexOf("<item>")) != -1)
            {
                str = str.Substring(i + 6);

                str = str.Substring(str.IndexOf("<![CDATA[ ") + "<![CDATA[ ".Length);

                i = str.IndexOf(" ]]>");
                titles.Add(str.Remove(i));

                str = str.Substring(str.IndexOf("<link>") + "<link>".Length);
                i = str.IndexOf("</link>");
                links.Add(str.Remove(i));
            }
        }
        public string statTarget = "储存卡状态：";
        public string statNet = "网络状态：";
        public string statLog = "";
        public string setTarget = "";
        public int setMax;
        public bool[] setIsSelected = new bool[3];
        public void newLog(string msg)
        {
            lock (this.statLog)
            {
                statLog = "[" + DateTime.Now.ToLongTimeString() + "]" + msg + "\r\n" + statLog;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var reader = new BinaryReader(File.OpenRead("LVOASyncer.cfg"));
                textBox1.Text = reader.ReadString();
                numericUpDown1.Value = reader.ReadDecimal();
                checkBox1.Checked = reader.ReadBoolean();
                for (int i = 0; i < 3; i++)
                {
                    if (reader.ReadBoolean()) { listBox1.SelectedItems.Add(listBox1.Items[i]); }
                }
                reader.Close();
                timer1_Tick(null, null);
                this.syncThread = new Thread(this.threadStart);
                this.syncThread.Start();
                foreach (Control c in tabPage1.Controls)
                {
                    c.Enabled = false;
                }
            }
            catch
            {
                tabControl1.SelectedTab = tabPage1;
            }
            try
            {
                Directory.CreateDirectory("Cache");
            }
            catch { }

        }
        bool closing = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.closing && e.CloseReason == CloseReason.UserClosing)
            {
                this.Visible = false;
                e.Cancel = true;
            }
            var writer = new BinaryWriter(File.Create("LVOASyncer.cfg"));
            writer.Write(textBox1.Text);
            writer.Write(numericUpDown1.Value);
            writer.Write(checkBox1.Checked);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(listBox1.SelectedIndices.Contains(i));
            }
            writer.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.closing = true;
            try
            {
                this.syncThread.Abort();
                this.wc.CancelAsync();
            }
            catch { }
            this.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible = !this.Visible)
            {
                this.Activate();
            }
        }

        private void 显示隐藏51VOA同步助手ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1_MouseDoubleClick(null, null);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.button1_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.labTarget.Text = this.statTarget.Replace("\r\n", "");
            this.labNet.Text = this.statNet.Replace("\r\n","");
            this.textLog.Text = this.statLog;
            this.setTarget = this.textBox1.Text.TrimEnd('\\') + '\\';
            this.setMax = (int)this.numericUpDown1.Value;
            for (var i = 0; i < 3; i++)
            {
                this.setIsSelected[i] = this.listBox1.SelectedIndices.Contains(i);
            }
            if (this.progressBar1.Visible = this.progBusy)
            {
                this.progressBar1.Maximum = (int)this.progAll;
                this.progressBar1.Value = (int)this.progNow;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(textBox1.Text.TrimEnd('\\') + '\\');
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.button4_Click(null, null);
            this.button3_Click(null, null);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                this.notifyIcon1_MouseDoubleClick(null, null);
            }
        }
    }
}
