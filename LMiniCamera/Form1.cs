using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LMiniCamera
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        HardwareHelperLib.CameraPanel p;
        private void Form1_Load(object sender, EventArgs e)
        {
            var s = Screen.PrimaryScreen.Bounds;
            this.Width = 640;
            this.Height = 480;
            p = new HardwareHelperLib.CameraPanel(panel1.Handle, 0, 0, panel1.Width, panel1.Height);
            p.Start();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            p.Stop();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            p.GrabImage("temp.jpg");
            var img = Image.FromFile("temp.jpg");
            this.BackgroundImage = new Bitmap(img);
            panel1.Visible = !panel1.Visible;
            img.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (panel1.Visible)
            {
                this.button1_Click(null, null);
            }
            var img = Image.FromFile("temp.jpg");
            Clipboard.SetImage(new Bitmap(img));
            img.Dispose();
            this.Close();
        }
    }
}
