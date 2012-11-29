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

namespace IAproject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Image();
            Hist_eq();
            Val_eq();
            Sobel();
            Binarisation();
            Dilation();
            //Canny();
            //getRegion();
            //Raghav Anand 2010067
            //Karan Gupte 2010037
            //<--tester code
            //new tester
        }

        public Image<Gray, byte> Image()
        {
            
            Image<Gray, byte> xyz = new Image<Gray, byte>("C:/Emgu/emgucv-windows-x86 2.4.0.1717/Emgu.CV.Example/LicensePlateRecognition/license-plate.jpg");
            imageBox1.Image = xyz;
            return xyz;
            
            //Raghav Anand 2010067
            //Karan Gupte 2010037
            //<--tester code
            //new tester


        }
        Image<Gray, byte> img2;
        private Image<Gray,byte> Hist_eq()
       {
           Image<Gray, byte> img1 = Image();
            img2 = img1.Clone();
            int[] hist = new int[256];
            int[] cdf = new int[256];
            int[] eq = new int[256];

            for (int i = 0; i < 256; i++)
            {
                hist[i] = 0;
                cdf[i] = 0;
                eq[i] = 0;
            }

            for (int i = 0; i < img1.Height; i++)
            {
                for (int j = 0; j < img1.Width; j++)
                {
                    int val = (int)(img1[i, j].Intensity);
                    hist[val] = hist[val] + 1;
                }
            }


            cdf[0] = hist[0];
            for (int i = 1; i < 256; i++)
            {
                cdf[i] = cdf[i - 1] + hist[i];
            }


            for (int i = 0; i < img1.Height; i++)
            {
                for (int j = 0; j < img1.Width; j++)
                {
                    int val = (int)(img1[i, j].Intensity);
                    eq[val] = (int)(((cdf[val] - cdf[0]) * 255.0) / ((img1.Width * img1.Height) - cdf[0]));
                    Gray temp = new Gray();
                    temp.Intensity = eq[val];
                    img2[i, j] = temp;
                }
            }
            //imageBox2.Image = img2;
            return img2;
        }

        private Image<Gray,byte> Val_eq()
        {

            //img2 = img1.Clone();
            Image <Gray,byte> img3 = Hist_eq();
            for (int i = 1; i < img3.Height - 1; i++)
            {
                for (int j = 1; j < img3.Width - 1; j++)
                {
                    if (img3[i, j].Intensity < 140)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 0;
                        img3[i, j] = temp;
                    }
                    else if (img3[i, j].Intensity >= 140 && img3[i, j].Intensity < 180)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 160;
                        img3[i, j] = temp;
                    }
                    else
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 255;
                        img3[i, j] = temp;
                    }
                }
            }
            //imageBox3.Image = img3;
            return img3;
        }

        private Image<Gray, byte> Sobel()
        {
            //img3 = img2.Clone();
            //img2 = img1.Clone();
           
            Image <Gray,byte> img4 = Val_eq();
            Image<Gray, byte> img5 = Hist_eq();
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            int[,] G = new int[img5.Height, img5.Width];

            for (int i = 1; i < img5.Height - 1; i++)
            {
                for (int j = 1; j < img5.Width - 1; j++)
                {
                    float new_x = 0, new_y = 0;
                    float c;

                    for (int hw = 0; hw < 3; hw++)
                    {
                        for (int wi = 0; wi < 3; wi++)
                        {
                            //c = (float)(img2[i, j].Intensity);
                            new_x = new_x + gx[hw, wi] * (float)(img2[i - (1 - hw), j - (1 - wi)].Intensity);
                            new_y = new_y + gy[hw, wi] * (float)(img2[i - (1 - hw), j - (1 - wi)].Intensity);
                        }
                    }
                    Gray temp = new Gray();
                    temp.Intensity = Math.Sqrt(Math.Pow(new_x, 2) + Math.Pow(new_y, 2));
                    img4[i, j] = temp;
                }
            }
            //imageBox4.Image = img4;
            return img4;

        }
        private Image<Gray,byte> Binarisation()
        {
            Image<Gray, byte> img6 = Sobel();
            for (int i = 1; i < img6.Height - 1; i++)
            {
                for (int j = 1; j < img6.Width - 1; j++)
                {
                    if (img6[i, j].Intensity > 130)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 255;
                        img6[i, j] = temp;
                    }
                    else
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 0;
                        img6[i, j] = temp;
                    }
                }
                
            }

            //imageBox5.Image = img6;
            return img6;
        }
        private Image<Gray,byte> Dilation()
        {
            Image<Gray, byte> img7 = Binarisation();
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(2, 2, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(img7.Ptr, img7.Ptr, structuring_element, 1);
            imageBox6.Image = img7;
            return img7;
        }

        

        

    }
}
