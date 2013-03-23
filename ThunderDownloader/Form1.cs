using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
namespace ThunderDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DateTime d1 = DateTime.Now;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ((DateTime.Now - this.d1).TotalMilliseconds < 2000)
            {
                return;
            }
            this.d1 = DateTime.Now;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.StreamReader sr = new System.IO.StreamReader(assembly.GetManifestResourceStream("ThunderDownloader.json2.js"), Encoding.UTF8);
            var scripts = sr.ReadToEnd();
            sr = new System.IO.StreamReader(assembly.GetManifestResourceStream("ThunderDownloader.scripts.js"), Encoding.UTF8);
            scripts += sr.ReadToEnd();
            sr.Close();
            {
                mshtml.IHTMLDocument2 currentDoc = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;
                mshtml.IHTMLWindow2 win = (mshtml.IHTMLWindow2)currentDoc.parentWindow;
                win.execScript(scripts, "javascript");//调用函数F1 
            }
            var result = (string)webBrowser1.Document.InvokeScript("getList");
            var json=JObject.Parse(result);
            var list=json.SelectToken("list");
            if (list != null)
            {
                if (list.Count() > 0)
                {
                    MessageBox.Show(list.First().ToString());
                }
            }

        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            textBox1.Text += e.Url + "\r\n";
        }

        private void webBrowser1_FileDownload(object sender, EventArgs e)
        {
            textBox1.Text += "dopwn\r\n";
        }

    }
}
