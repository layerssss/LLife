using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace svchost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Random random = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            Cursor.Position = new Point(Cursor.Position.X + (int)Math.Round(this.speedX), Cursor.Position.Y + (int)Math.Round(this.speedY));
            var deltaSpeedX = (this.random.NextDouble() - 0.5) * 1;
            var deltaSpeedY = (this.random.NextDouble() - 0.5) * 1;
            this.speedX += deltaSpeedX;
            this.speedY += deltaSpeedY;
            if (Math.Abs(this.speedX) * 300000 > count * count)
            {
                this.speedX*=0.9;
            }
            if (Math.Abs(this.speedY) * 300000 > count * count)
            {
                this.speedY*=0.9;
            }
            count++;
            if (count > 2000)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.ShowBalloonTip(0, "layerssss说道", "你不觉得你的鼠标刚才在乱动吗？", ToolTipIcon.Warning);

            }
        }
        double speedX = 0;
        double speedY = 0;
        int count = 0;

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                if (DateTime.Now > DateTime.Parse("2011/4/1 14:00"))
                {
                    timer1.Enabled = true;
                }
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Activate();
            var f = new Form2();
            f.ShowDialog();
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Image.FromFile("pic.jpg");

        }
    }
}
