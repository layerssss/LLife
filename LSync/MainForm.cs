using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace LSync
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        string dst;
        string src;
        public void Init(string ext, string src, string dst, string title = "LSync")
        {
            this.dst = dst;
            this.src = src;
            this.Text = title;
            var exts = ext.Split('|');
            foreach(var sfile in Directory.GetFiles(src)){
                if (exts.Any(te=>sfile.ToLower().EndsWith(te)))
                {
                    {
                        bool canWrite = true;
                        try
                        {
                            var fs = File.OpenWrite(sfile);
                            fs.Close();
                        }
                        catch {
                            canWrite = false;
                        }
                        if (canWrite)
                        {
                            listBox1.Items.Add(sfile.Substring(sfile.LastIndexOf('\\') + 1));
                            if (File.Exists(dst.TrimEnd('\\') + sfile.Substring(sfile.LastIndexOf('\\'))))
                            {
                                listBox1.SelectedItems.Add(sfile.Substring(sfile.LastIndexOf('\\') + 1));
                            }
                        }
                    }
                }
            }
            foreach (var sfile in Directory.GetFiles(dst))
            {
                if (exts.Any(te => sfile.ToLower().EndsWith(te)))
                {
                    if (!listBox1.Items.Contains(sfile.Substring(sfile.LastIndexOf('\\') + 1)))
                    {
                        File.Delete(sfile);
                        File.Delete(sfile.Remove(sfile.LastIndexOf('.') + 1) + "lrc");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            long sum = 0;
            var fs = new List<FileInfo>();
            foreach (string fname in listBox1.SelectedItems)
            {
                if (!File.Exists(dst.TrimEnd('\\') + '\\' + fname))
                {
                    fs.Add(new FileInfo(src.TrimEnd('\\') + '\\' + fname));
                }
            }
            foreach (string fname in listBox1.Items)
            {
                if(!listBox1.SelectedItems.Contains(fname)){
                    File.Delete(dst.TrimEnd('\\') + '\\' + fname);
                    File.Delete(dst.TrimEnd('\\') + '\\' + fname.Remove(fname.LastIndexOf('.') + 1) + "lrc");
                }
            }
            foreach (var f in fs)
            {
                sum += f.Length;
            }
            this.max= sum;
            var t = new System.Threading.Thread(() =>
            {
                while (fs.Any())
                {
                    var f = fs.First();
                    fs.RemoveAt(0);
                    var buffer=new byte[65536];
                    var rstream = f.OpenRead();
                    var wstream=File.Create(dst.TrimEnd('\\') + '\\' + f.Name);
                    var size = f.Length;
                    while (size != 0)
                    {
                        var len = rstream.Read(buffer, 0, (int)Math.Min(size, buffer.LongLength));
                        size -= len;
                        wstream.Write(buffer, 0, len);
                        wstream.Flush();
                        this.value += len;
                    }
                    wstream.Close();
                    rstream.Close();
                    try
                    {
                        File.Copy(src.TrimEnd('\\') + '\\' + f.Name.Remove(f.Name.LastIndexOf('.') + 1) + "lrc",
                            dst.TrimEnd('\\') + '\\' + f.Name.Remove(f.Name.LastIndexOf('.') + 1) + "lrc", true);
                    }
                    catch { }
                }
            });
            t.Start();
        }
        long value = 0;
        long max = 1;
        private void MainForm_Load(object sender, EventArgs e)
        {
            var lines=File.ReadAllLines("LSync.size");
            this.Size = new Size(Convert.ToInt32(lines[0]), Convert.ToInt32(lines[1]));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllLines("LSync.size", new[] { this.Size.Width.ToString(), this.Size.Height.ToString() });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.max == 0)
            {
                this.Close();
                return;
            }
            
            this.progressBar1.Value = (int)(value * 100 / max);
            if (this.value == this.max)
            {
                this.Close();
            }
        }
    }
}
