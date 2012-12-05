/*
 * 
 *   License Plate Detector
 *   By:
 *   Raghav Anand 2010067
 *   Karan Gupta  2010037
 *   
 */





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
            img4 = Sobel_filter(img3);
            imageBox3.Image = img4.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);            
            img5 = Binarization(img4);
            img6 = Erosion(img5);
            imageBox4.Image = img6.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            img7 = Contour(img6);          
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

        private Image<Gray, byte> Sobel_filter(Image<Gray, byte> img3)
        {
            Image <Gray,byte> img4 =img3.Clone();
           
            int[,] Gx = new int[,] { { -1, 0, 1 },
                                     { -2, 0, 2 },
                                     { -1, 0, 1 } };
            
            int[,] Gy = new int[,] { { -1, -2, -1 },
                                     { 0, 0, 0 },
                                     { 1, 2, 1 } };
            
            for (int i = 1; i < img3.Height - 1; i++)
            {
                for (int j = 1; j < img3.Width - 1; j++)
                {
                    double convolution_x = 0, convolution_y = 0;

                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            convolution_x = convolution_x + Gx[m, n] * (img3[i - (1 - m), j - (1 - n)].Intensity);
                            convolution_y = convolution_y + Gy[m, n] * (img3[i - (1 - m), j - (1 - n)].Intensity);
                        }
                    }
                    Gray temp = new Gray();
                    temp.Intensity = Math.Sqrt(Math.Pow(convolution_x, 2) + Math.Pow(convolution_y, 2));
                    img4[i, j] = temp;
                }
            }
            return img4;
        }
        
        private Image<Gray,byte> Binarization(Image<Gray, byte> img4)
        {
            Image<Gray, byte> img5 = img4.Clone();            
            for (int i = 1; i < img4.Height - 1; i++)
            {
                for (int j = 1; j < img4.Width - 1; j++)
                {
                    if (img4[i, j].Intensity > 128)
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
        
        private Image<Gray, byte> Contour(Image<Gray, byte> img6)
        {
            Image<Gray, byte> gray = img6.Clone();
            Gray cannyThreshold = new Gray(1000);//180
            Gray cannyThresholdLinking = new Gray(1000);//120
            Image<Gray, Byte> edgeimage = gray.Canny(cannyThreshold, cannyThresholdLinking);

            //Dilating the Edge Image
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(edgeimage.Ptr, edgeimage.Ptr, structuring_element, 1);


            /*LineSegment2D[] lines = edgeimage.HoughLinesBinary(
                1, //Distance resolution in pixel-related units
                Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10 //gap between lines
                )[0]; //Get the lines from the first channel8*/

            
            List<MCvBox2D> rectangle_list = new List<MCvBox2D>();
            //Console.Write("No of lines in Canny" + lines.Length);
            MemStorage storage = new MemStorage();
            Image<Gray, Byte> temp = new Image<Gray, byte>(gray.Size);
            for (Contour<Point> contours = edgeimage.FindContours(); contours != null; contours = contours.HNext)
            {
               
                Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, 2, storage);
                if (currentContour.Area > 500)
                {
                    if (currentContour.Total == 4) //The contour has 4 vertices, it is a rectangle
                    {
                        Point[] pts = currentContour.ToArray();
                        LineSegment2D[] linesarray = PointCollection.PolyLine(pts, true);

                        //Console.Write("No of lines in current contour " + edges.Length);
                        /*for (int i = 0; i < linesarray.Length; i++)
                        {
                            //textBox1.Text = Convert.ToString(linesarray.Length);
;                            double angle = Math.Abs(linesarray[(i + 1) % linesarray.Length].GetExteriorAngleDegree(linesarray[i]));

                            if ((angle > 30 && angle < 130))
                            {
                                rectangle_list.Add(currentContour.GetMinAreaRect());
                            }
                                                     
                        }*/
                        rectangle_list.Add(currentContour.GetMinAreaRect());
                    }
                }
            }
            
            Image<Bgr, byte> original = new Image<Bgr, byte>(openFileDialog1.FileName);
            foreach (MCvBox2D rectangle in rectangle_list)
            {
                Image<Bgr, Byte> plate = original.Copy(rectangle);
                double rotate_angle = 270;
                if (plate.Width > 24 && plate.Height > 37) //(24) & (50 or 36)
                {
                            if (plate.Width <= plate.Height)
                            {
                                plate = plate.Rotate((double)rotate_angle, new Bgr(255, 255, 255), false);
                            }
                            imageBox7.Image = plate;
                            break;
                        }
                }
            Image<Gray, Byte> img7;
            Image<Gray, Byte> RectangleImage = gray.CopyBlank();

            foreach (MCvBox2D rectangle in rectangle_list)
                RectangleImage.Draw(rectangle, new Gray(255), 2);
            img7 = RectangleImage.Clone();
            
            Bgr coloured_region = new Bgr();
            Image<Bgr, Byte> final = new Image<Bgr, byte>(img1.Width, img1.Height);
            for (int i = 0; i < img7.Height; i++)
            {
                for (int j = 0; j < img7.Width; j++)
                {
                    if (img7[i, j].Intensity == 255)
                    {
                        coloured_region = new Bgr(Color.SkyBlue);
                        final[i, j] = coloured_region;
                    }
                    else
                    {
                        final[i, j] = original[i, j];
                    }
                }
            }

            imageBox6.Image = final.Resize(315, 266, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            return img7;            
        }

              
    }
}
