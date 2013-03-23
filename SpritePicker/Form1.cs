using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpritePicker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
            if (files.Length != 1)
            {

                e.Effect = DragDropEffects.None;
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Bitmap bmp;
            try
            {
                String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
                var img = Image.FromFile(files[0]);
                bmp = new Bitmap(img);
                img.Dispose();
            }
            catch
            {
                MessageBox.Show("不能识别该图像文件的格式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.pictureBox1.Size = bmp.Size;
            this.pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(string.Format("x:{0}   y:{1}", e.Location.X, e.Location.Y));
        }
    }
}
