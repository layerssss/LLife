using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace WindowGrabber
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(
            string lpClassName,
            string lpWindowName
        );
        public const int HWND_TOPMOST = -1;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(
            int hwnd,
            int hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int wFlags
        );
        public Form1()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
            if (keyData == Keys.Enter)
            {
                var handler = FindWindow(null, this.comboBox1.Text);
                var bound=Screen.PrimaryScreen.Bounds;
                SetWindowPos(handler, HWND_TOPMOST, bound.X, bound.Y, bound.Width, bound.Height, 0);
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var bound = Screen.PrimaryScreen.Bounds;
            this.Left = (bound.Width - this.Width) / 2;
            this.Top = 0;
            this.hids = System.IO.File.ReadAllLines("hids.txt");
            EnumWindows(new CallBack(mycallbak), 0);

        }
        string[] hids;
        public delegate bool CallBack(int hwnd, int lParam);
        [DllImport("user32.Dll ")]
        public static extern int EnumWindows(CallBack x, int y);  
        private bool mycallbak(int hWnd, int lParam)
        {
            if (hWnd == 0)
                return false;
            if (IsWindowVisible(hWnd) == 0)
            {
                return true;
            }
            var str = new StringBuilder(255);
            GetWindowText(hWnd, str, 255);
            if (str.ToString().Trim() == "")
            {
                return true;
            }
            if(this.hids.Contains(str.ToString().Trim())){
                return true;
            }
            comboBox1.Items.Add(str.ToString());
            return true;
        }
        [DllImport("user32.dll", EntryPoint = "IsWindowEnabled")]
        public static extern int IsWindowEnabled(
            int hwnd
        );
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        public static extern int IsWindowVisible(
            int hwnd
        );
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(
            int hwnd,
            StringBuilder lpString,
            int cch
        ); 
    }
}
