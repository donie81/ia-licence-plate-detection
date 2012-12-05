﻿using System;
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

namespace IAproject
{
    public partial class Form1 : Form
    {
        Image<Gray, Byte> img, img1, img2, img3, img4, img5, img6, img7, img8;

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Load the image from file
                Image<Bgr, Byte> img_bgr = new Image<Bgr, byte>(openFileDialog1.FileName);
                imageBox1.Image = img_bgr.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                //Convert the image to grayscale and filter out the noise
                img = img_bgr.Convert<Gray, Byte>();

                // Blur the image
                img1 = img.SmoothBlur(6,6, true);
               
            }           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            img2 = Hist_eq(img1);
            imageBox2.Image = img2.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            img3 = img2.Clone();
            img4 = Sobel(img3);
            imageBox3.Image = img4.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);            
            img5 = Binarisation(img4);
            img6 = Erosion(img5);
            imageBox4.Image = img6.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            img7 = Canny(img6);          
        }

        public Form1()
        {
            InitializeComponent();
        }

        private Image<Gray,byte> Hist_eq(Image<Gray,byte> img1)
        {            
            Image<Gray,Byte> img2 = img1.Clone();
            //Histogram Equalization
            img2._EqualizeHist();
            //Normalize
            for (int i = 1; i < img2.Height - 1; i++)
            {
                for (int j = 1; j < img2.Width - 1; j++)
                {
                    if (img2[i, j].Intensity < 140)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 0;
                        img2[i, j] = temp;
                    }
                    else if (img2[i, j].Intensity >= 140 && img2[i, j].Intensity < 180)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 160;
                        img2[i, j] = temp;
                    }
                    else
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 255;
                        img2[i, j] = temp;
                    }
                }
            }
            return img2;
        }

        private Image<Gray, byte> Sobel(Image<Gray, byte> img3)
        {
            Image <Gray,byte> img4 =img3.Clone();
           
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            int[,] G = new int[img3.Height, img3.Width];

            for (int i = 1; i < img3.Height - 1; i++)
            {
                for (int j = 1; j < img3.Width - 1; j++)
                {
                    float new_x = 0, new_y = 0;
                    for (int hw = 0; hw < 3; hw++)
                    {
                        for (int wi = 0; wi < 3; wi++)
                        {
                            new_x = new_x + gx[hw, wi] * (float)(img3[i - (1 - hw), j - (1 - wi)].Intensity);
                            new_y = new_y + gy[hw, wi] * (float)(img3[i - (1 - hw), j - (1 - wi)].Intensity);
                        }
                    }
                    Gray temp = new Gray();
                    temp.Intensity = Math.Sqrt(Math.Pow(new_x, 2) + Math.Pow(new_y, 2));
                    img4[i, j] = temp;
                }
            }
            return img4;
        }
        
        private Image<Gray,byte> Binarisation(Image<Gray, byte> img4)
        {
           
            Image<Gray, byte> img5 = img4.Clone();            
            for (int i = 1; i < img4.Height - 1; i++)
            {
                for (int j = 1; j < img4.Width - 1; j++)
                {
                    if (img4[i, j].Intensity > 130)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 255;
                        img5[i, j] = temp;
                    }
                    else
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 0;
                        img5[i, j] = temp;
                    }
                }
                
            }
            return img5;
        }
        private Image<Gray,byte> Erosion(Image<Gray, byte> img5)
        {
            Image<Gray, byte> img6 = img5.Clone();
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(2, 2, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvErode(img6.Ptr, img6.Ptr, structuring_element, 1);
            return img6;
        }
        
        private Image<Gray, byte> Canny(Image<Gray, byte> img6)
        {
            Image<Gray, byte> gray = img6.Clone();
            Gray cannyThreshold = new Gray(1000);//180
            Gray cannyThresholdLinking = new Gray(1000);//120
            Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(cannyEdges.Ptr, cannyEdges.Ptr, structuring_element, 1);


            /*LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
                1, //Distance resolution in pixel-related units
                Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10 //gap between lines
                )[0]; //Get the lines from the first channel8*/
            
            List<MCvBox2D> boxList = new List<MCvBox2D>();
            //Console.Write("No of lines in Canny" + lines.Length);
            MemStorage storage = new MemStorage();
            Image<Gray, Byte> temp = new Image<Gray, byte>(gray.Size);
            for (Contour<Point> contours = cannyEdges.FindContours(); contours != null; contours = contours.HNext)
            {
               
                Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, 2, storage);
                if (currentContour.Area > 500)
                {
                    if (currentContour.Total == 4) //The contour has 4 vertices, it is a rectangle
                    {
                        bool isRectangle = false;
                        Point[] pts = currentContour.ToArray();
                        LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                        //Console.Write("No of lines in current contour " + edges.Length);
                        for (int i = 0; i < edges.Length; i++)
                        {
                            
                            double angle = Math.Abs(
                               edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));

                            if ((angle > 30 && angle < 130))
                            {
                                isRectangle = true;
                                boxList.Add(currentContour.GetMinAreaRect());
                            }
                                                     
                        }
                    }
                }
            }
            
            Image<Bgr, byte> temp_load = new Image<Bgr, byte>(openFileDialog1.FileName);
            foreach (MCvBox2D box in boxList)
            {
                Image<Bgr, Byte> plate = temp_load.Copy(box);
                double rot = 270;
                if (plate.Width > 24 && plate.Height > 37) //(24) & (50 or 36)
                {
                            if (plate.Width <= plate.Height)
                            {
                                plate = plate.Rotate((double)rot, new Bgr(255, 255, 255), false);
                            }
                            imageBox7.Image = plate;
                            break;
                        }
                }
            Image<Gray, Byte> img7;
            Image<Gray, Byte> RectangleImage = gray.CopyBlank();
            foreach (MCvBox2D box in boxList)
                RectangleImage.Draw(box, new Gray(255), 2);
            img7 = RectangleImage.Clone();
            
            Bgr col = new Bgr();
            Image<Bgr, Byte> imgReg = new Image<Bgr, byte>(img1.Width, img1.Height);
            for (int i = 0; i < img7.Height; i++)
            {
                for (int j = 0; j < img7.Width; j++)
                {
                    if (img7[i, j].Intensity == 255)
                    {
                        col = new Bgr(Color.SkyBlue);
                        imgReg[i, j] = col;
                    }
                    else
                    {
                        imgReg[i, j] = temp_load[i, j];
                    }
                }
            }

            imageBox6.Image = imgReg.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            return img7;            
        }

              
    }
}
