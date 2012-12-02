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
        Image<Gray, Byte> img, edgeimage, img3, img4, img5, img6;

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Gray, byte>(openFileDialog1.FileName);                
                imageBox1.Image = img;
            }
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //-------------------CANNY DETECTS EDGES-------------------
            Gray cannyThreshold = new Gray(180);
            Gray cannyThresholdLinking = new Gray(120);
            edgeimage = img.Canny(cannyThreshold, cannyThresholdLinking);
            //imageBox2.Image = img2;

            //-------------------THIS IMAGE OF EDGES IS DILATED-------------------
            
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(edgeimage.Ptr, edgeimage.Ptr, structuring_element, 1);
            imageBox2.Image = edgeimage;
            //imageBox3.Image = edgeimage;
            //imageBox4.Image = edgeimage;
            //imageBox5.Image = edgeimage;
            //imageBox6.Image = edgeimage;


            /*LineSegment2D[] lines = edgeimage.HoughLinesBinary(
    1, //Distance resolution in pixel-related units
    Math.PI / 45.0, //Angle resolution measured in radians.
    20, //threshold
    30, //min Line width
    10 //gap between lines
    )[0]; //Get the lines from the first channel*/

            //-------------------NUMBER OF LINES IN THE EDGE IMAGE ARE CALCULATED AND STORED-------------------
            LineSegment2D[] linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 20, 30, 10)[0];
            textBox1.Text = linesfromhough.Length.ToString();
            MemStorage storage = new MemStorage();
            /*
            for (Contour<Point> contours = edgeimage.FindContours(); contours != null; contours = contours.HNext)
            {
                Contour<Point> CurrentContour = contours.ApproxPoly(contours.Perimeter,2,storage);
                if (CurrentContour.Area > 400)
                {
                    if (CurrentContour.Total == 4)
                    {

                    }
            */
            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 40, 30, 10)[0];
            textBox2.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 60, 30, 10)[0];
            textBox3.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 80, 30, 10)[0];
            textBox4.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 100, 30, 10)[0];
            textBox5.Text = linesfromhough.Length.ToString();


            /*
            cannyThreshold = new Gray(180);
            cannyThresholdLinking = new Gray(120);
            img3 = img.Canny(cannyThreshold, cannyThresholdLinking);
            imageBox3.Image = img3;

            cannyThreshold = new Gray(180);
            cannyThresholdLinking = new Gray(180);
            img4 = img.Canny(cannyThreshold, cannyThresholdLinking);
            imageBox4.Image = img4;

            cannyThreshold = new Gray(180);
            cannyThresholdLinking = new Gray(240);
            img5 = img.Canny(cannyThreshold, cannyThresholdLinking);
            imageBox5.Image = img5;

            cannyThreshold = new Gray(180);
            cannyThresholdLinking = new Gray(360);
            img6 = img.Canny(cannyThreshold, cannyThresholdLinking);
            imageBox6.Image = img6;
             */
        }
    }
}
