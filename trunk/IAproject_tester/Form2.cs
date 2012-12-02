using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace IAproject_tester
{
    public partial class Form2 : Form
    {
        Image<Gray, Byte> img;

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            openFileDialog1.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                
                //Console.Write(dialog.FileName);
                //textBox1.Text = dialog.FileName;
                img = new Image<Gray, byte>(openFileDialog1.FileName);
                
                imageBox1.Image = img;
            }
        }
        public Form2()
        {
            InitializeComponent();
        }
    }
}
