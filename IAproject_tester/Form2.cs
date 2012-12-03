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
        Image<Gray, Byte> img, edgeimage, img3, img4, img5, img6, temp;

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
            imageBox2.Image = edgeimage;

            
            //-------------------THIS IMAGE OF EDGES IS DILATED-------------------
            
            /*
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(edgeimage.Ptr, edgeimage.Ptr, structuring_element, 1);
            imageBox2.Image = edgeimage;
            */

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
            
            /*
            LineSegment2D[] linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 20, 30, 10)[0];
            textBox1.Text = linesfromhough.Length.ToString();

            List<MCvBox2D> boxList = new List<MCvBox2D>();
            MemStorage storage = new MemStorage();
            */

            //All contours are found, and only the contours with 4 vertices are considered
            

            /*
            for (Contour<Point> contours = edgeimage.FindContours(); contours != null; contours = contours.HNext)
            {
                Contour<Point> CurrentContour = contours.ApproxPoly(contours.Perimeter * 0.05,2,storage);
                if (CurrentContour.Area > 400)
                {
                    if (CurrentContour.Total == 4) //The contour has 4 vertices, it is a rectangle
                    {
                        
                        bool isRectangle = false;
                        Point[] pts = CurrentContour.ToArray();
                        LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                        for (int i = 0; i < edges.Length; i++)
                        {
                            Image<Gray, byte> temp = new Image<Gray,byte>(img.Size);
                            temp.Draw(edges[i], new Gray(255), 2);
                            imageBox3.Image = temp;
                            double angle = Math.Abs(
                               edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));

                            temp.Draw(edges[i], new Gray(255), 2);
                            temp.Draw(edges[(i + 2) % edges.Length], new Gray(255), 2);
                            imageBox4.Image = temp;
                            Point e1p1 = edges[i].P1;
                            Point e1p2 = edges[i].P2;

                            Point e2p1 = edges[(i + 2) % edges.Length].P1;
                            Point e2p2 = edges[(i + 2) % edges.Length].P2;

                            double diff1 = Math.Abs(edges[i].Length - edges[(i + 2) % edges.Length].Length);
                            double diff2 = Math.Abs(edges[(i + 1) % edges.Length].Length - edges[(i + 3) % edges.Length].Length);

                            if (diff1 / edges[i].Length <= 0.07 && diff1 / edges[(i + 2) % edges.Length].Length <= 0.07
                               && diff2 / edges[(i + 1) % edges.Length].Length <= 0.07 && diff2 / edges[(i + 3) % edges.Length].Length <= 0.07)
                            {
                                temp.Draw(edges[0], new Gray(255), 2);
                                temp.Draw(edges[1], new Gray(255), 2);
                                temp.Draw(edges[2], new Gray(255), 2);
                                temp.Draw(edges[3], new Gray(255), 2);
                                imageBox5.Image = temp;
                                isRectangle = true;
                                boxList.Add(CurrentContour.GetMinAreaRect());

                            }


                            if (edges[(i + 2) % edges.Length].Direction == edges[i].Direction)
                            {
                                temp.Draw(edges[i], new Gray(255), 2);
                                temp.Draw(edges[(i + 2) % edges.Length], new Gray(255), 2);

                                imageBox6.Image = temp;
                                isRectangle = true;
                                boxList.Add(CurrentContour.GetMinAreaRect());

                            }
                            if ((angle > 30 && angle < 130))
                            {
                                //temp.Draw(edges[i], new Gray(255), 2);
                                //imageBox1.Image = temp;
                                isRectangle = true;
                                boxList.Add(CurrentContour.GetMinAreaRect());
                            }

                            //Console.Write("\nAngle in current contour " + angle);
                        }
                    }
                 }
            }
            
            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 40, 30, 10)[0];
            textBox2.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 60, 30, 10)[0];
            textBox3.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 80, 30, 10)[0];
            textBox4.Text = linesfromhough.Length.ToString();

            linesfromhough = edgeimage.HoughLines(new Gray(180), new Gray(120), 1, Math.PI / 45.0, 100, 30, 10)[0];
            textBox5.Text = linesfromhough.Length.ToString();
            */

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

            Image<Gray, byte> img4 = img.Clone();
            //Image<Gray, byte> img5 = Hist_eq();
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            int[,] G = new int[img.Height, img.Width];

            for (int i = 1; i < img.Height - 1; i++)
            {
                for (int j = 1; j < img.Width - 1; j++)
                {
                    float new_x = 0, new_y = 0;
                    //float c;

                    for (int hw = 0; hw < 3; hw++)
                    {
                        for (int wi = 0; wi < 3; wi++)
                        {
                            //c = (float)(img2[i, j].Intensity);
                            new_x = new_x + gx[hw, wi] * (float)(img[i - (1 - hw), j - (1 - wi)].Intensity);
                            new_y = new_y + gy[hw, wi] * (float)(img[i - (1 - hw), j - (1 - wi)].Intensity);
                        }
                    }
                    Gray temp = new Gray();
                    temp.Intensity = Math.Sqrt(Math.Pow(new_x, 2) + Math.Pow(new_y, 2));
                    img4[i, j] = temp;
                }
            }

            imageBox3.Image = img4;

        }
    }
}
