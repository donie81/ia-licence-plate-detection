using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace IAproject_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Image<Gray, byte> img1, img2, img3, res, img5;
        //Image<Bgr, byte> temp;
        Image<Bgr, byte> temp_load;
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                temp_load = new Image<Bgr, byte>(openFileDialog1.FileName);

                imageBox1.Image = temp_load;
                /*
                img1 = new Image<Gray, byte>(temp_load.Size);
                for (int i = 0; i < temp_load.Height; i++)
                {
                    for (int j = 0; j < temp_load.Width; j++)
                    {
                        double val = ((temp_load[i, j].Red * 299) + (temp_load[i, j].Green * 587) + (temp_load[i, j].Blue * 114)) / 1000;
                        Gray temp = new Gray(val);
                        img1[i, j] = temp;
                    }
                }*/
                img1 = temp_load.Convert<Gray, Byte>().PyrDown().PyrUp();
                //img1 = img1.Resize(512,384,true);

                res = img1.Clone();

            }
        }

        private void Hist_eq()
        {
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
        }

        private void Val_eq()
        {

            //img2 = img1.Clone();
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
        }

        private void Sobel()
        {
            //img3 = img2.Clone();
            //img2 = img1.Clone();
            img3 = img2.Clone();
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            int[,] G = new int[img2.Height, img2.Width];

            for (int i = 1; i < img2.Height - 1; i++)
            {
                for (int j = 1; j < img2.Width - 1; j++)
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
                    img3[i, j] = temp;
                }
            }
            //imageBox3.Image = img3;

        }

        private void Binarisation()
        {
            for (int i = 1; i < img3.Height - 1; i++)
            {
                for (int j = 1; j < img3.Width - 1; j++)
                {
                    if (img3[i, j].Intensity > 130)
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 255;
                        img3[i, j] = temp;
                    }
                    else
                    {
                        Gray temp = new Gray();
                        temp.Intensity = 0;
                        img3[i, j] = temp;
                    }
                }

            }

            //imageBox4.Image = img3;
        }

        private void Dilation()
        {
            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(2, 2, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(img3.Ptr, img3.Ptr, structuring_element, 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Image<Gray, Byte> gray = img1.Clone();
            //Image<Gray, Byte> res = img1.Clone();
            //imageBox2.Image = gray;
            //img1 = gray.Clone();
            Hist_eq();
            Val_eq();
            Sobel();
            Binarisation();
            Dilation();
            //imageBox4.Image = img3;
            gray = img3.Clone();
            Gray cannyThreshold = new Gray(180);
            Gray cannyThresholdLinking = new Gray(120);
            Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);

            IntPtr structuring_element = CvInvoke.cvCreateStructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE, IntPtr.Zero);
            CvInvoke.cvDilate(cannyEdges.Ptr, cannyEdges.Ptr, structuring_element, 1);


            LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
                1, //Distance resolution in pixel-related units
                Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10 //gap between lines
                )[0]; //Get the lines from the first channel
            //imageBox4.Image = cannyEdges;
            List<MCvBox2D> boxList = new List<MCvBox2D>();
            //Console.Write("No of lines in Canny" + lines.Length);
            MemStorage storage = new MemStorage();
            Image<Gray, Byte> temp = new Image<Gray, byte>(img3.Size);
            for (Contour<Point> contours = cannyEdges.FindContours(); contours != null; contours = contours.HNext)
            {
                //Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);
                Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, 2, storage);
                if (currentContour.Area > 500)
                {
                    if (currentContour.Total == 4)
                    {
                        //temp.Draw(contours, new Gray(255), 2);
                        //imageBox3.Image = temp;

                        bool isRectangle = false;
                        Point[] pts = currentContour.ToArray();
                        LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                        //Console.Write("No of lines in current contour " + edges.Length);
                        for (int i = 0; i < edges.Length; i++)
                        {
                            //temp.Draw(edges[i], new Gray(255), 2);
                            //imageBox3.Image = temp;
                            double angle = Math.Abs(
                               edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));

                            //temp.Draw(edges[i], new Gray(255), 2);
                            //temp.Draw(edges[(i + 2) % edges.Length], new Gray(255), 2);
                            //imageBox1.Image = temp;
                            Point e1p1 = edges[i].P1;
                            Point e1p2 = edges[i].P2;

                            Point e2p1 = edges[(i + 2) % edges.Length].P1;
                            Point e2p2 = edges[(i + 2) % edges.Length].P2;

                            double diff1 = Math.Abs(edges[i].Length - edges[(i + 2) % edges.Length].Length);
                            double diff2 = Math.Abs(edges[(i + 1) % edges.Length].Length - edges[(i + 3) % edges.Length].Length);

                            if (diff1 / edges[i].Length <= 0.07 && diff1 / edges[(i + 2) % edges.Length].Length <= 0.07
                               && diff2 / edges[(i + 1) % edges.Length].Length <= 0.07 && diff2 / edges[(i + 3) % edges.Length].Length <= 0.07)
                            {
                                //temp.Draw(edges[0], new Gray(255), 2);
                                //temp.Draw(edges[1], new Gray(255), 2);
                                //temp.Draw(edges[2], new Gray(255), 2);
                                //temp.Draw(edges[3], new Gray(255), 2);
                                //imageBox1.Image = temp;
                                isRectangle = true;
                                boxList.Add(currentContour.GetMinAreaRect());

                            }


                            if (edges[(i + 2) % edges.Length].Direction == edges[i].Direction)
                            {
                                //temp.Draw(edges[i], new Gray(255), 2);
                                //temp.Draw(edges[(i + 2) % edges.Length], new Gray(255), 2);

                                //imageBox1.Image = temp;
                                isRectangle = true;
                                boxList.Add(currentContour.GetMinAreaRect());

                            }



                            if ((angle > 30 && angle < 130))
                            {
                                //temp.Draw(edges[i], new Gray(255), 2);
                                //imageBox1.Image = temp;
                                isRectangle = true;
                                boxList.Add(currentContour.GetMinAreaRect());


                            }

                            //Console.Write("\nAngle in current contour " + angle);



                        }
                    }
                }
            }
            /* List<MCvBox2D> plateList = new List<MCvBox2D>();
             foreach (MCvBox2D box in boxList)
             {
                 double ratio = box.size.Width / box.size.Height;
                 if ((ratio > 3.5 && ratio < 4.5) || (ratio > 0.8 && ratio < 1.4))
                 {
                     Console.Write("\nshape:  " + box.size.Width / box.size.Height);
                     plateList.Add(box);
                 }

             }*/
            foreach (MCvBox2D box in boxList)
            {
                double whRatio = (double)box.size.Width / box.size.Height;

                Image<Bgr, Byte> plate = temp_load.Copy(box);
                //CvInvoke.cvShowImage("plate",plate.Ptr);
                //System.Threading.Thread.Sleep(3000);
                //imageBox2.Image = plate;
                int white_count = 0;
                int black_count = 1;
                double ratio1 = box.size.Width / box.size.Height;
                double ratio2 = box.size.Height / box.size.Width;
                //MessageBox.Show("ratio inside " + ratio1 + " " + ratio2);
                int sz = (temp_load.Height * temp_load.Width) - (plate.Height * plate.Width);
                //MessageBox.Show("Image Size " + temp_load.Height * temp_load.Width + " PLate Size " + plate.Height * plate.Width + "  " + sz);
                if (sz <= 3 * (temp_load.Height + temp_load.Width))
                {
                    //MessageBox.Show("Size");
                    continue;
                }
                for (int i = 0; i < plate.Height; i++)
                {
                    for (int j = 0; j < plate.Width; j++)
                    {
                        //Console.Write(plate[i, j].Intensity);
                        double val = ((plate[i, j].Red * 299) + (plate[i, j].Green * 587) + (plate[i, j].Blue * 114)) / 1000;
                        if (val > 180)
                        {
                            white_count++;
                        }
                        else if (val < 95)
                        {
                            black_count++;
                        }
                    }

                }


                //MessageBox.Show("plate width " + plate.Width);
                // MessageBox.Show("plate height " + plate.Height);
                //MessageBox.Show("plate size " + plate.Width*plate.Height);
                //&& (white_count / (plate.Width * plate.Height)) >= 0.4) && (white_count / (plate.Width * plate.Height)) < 0.8)

                if ((ratio1 > 3.3) || (ratio2 > 3.3))
                {
                    //MessageBox.Show("Black: " + ((double)(1.0 * black_count / (plate.Width * plate.Height))));
                    //MessageBox.Show("White: " + ((double)(1.0 * white_count / (plate.Width * plate.Height))));
                    //MessageBox.Show("ratio " + (double)(1.0 * white_count / (plate.Width * plate.Height)));
                    if (((double)(1.0 * white_count / (plate.Width * plate.Height)) >= 0.3) && ((double)(1.0 * white_count / (plate.Width * plate.Height)) <= 0.8) && ((double)(1.0 * black_count / (plate.Width * plate.Height)) >= 0.1) && ((double)(1.0 * black_count / (plate.Width * plate.Height)) <= 0.4))
                    {
                        //MessageBox.Show("Black: " + black_count);
                        //MessageBox.Show("White: " + white_count);
                        //MessageBox.Show("ratio inside " + ratio1);
                        //Image<Gray, Byte> filteredPlate = FilterPlate(plate);
                        //Image<Gray, Byte> filtered = filteredPlate.Copy(box);
                        double rot = 90;
                        if (plate.Width < plate.Height)
                        {
                            plate = plate.Rotate((double)rot, new Bgr(255, 255, 255), false);
                            //MessageBox.Show("Rot");
                        }
                        imageBox2.Image = plate;
                        break;
                    }
                }

            }
            Image<Gray, Byte> RectangleImage = gray.CopyBlank();
            foreach (MCvBox2D box in boxList)
                RectangleImage.Draw(box, new Gray(255), 2);
            //imageBox2.Image = RectangleImage;
            img5 = RectangleImage.Clone();
            Image<Gray, Byte> lineImage = cannyEdges.CopyBlank();
            foreach (LineSegment2D line in lines)
                lineImage.Draw(line, new Gray(255), 2);
            //imageBox3.Image = lineImage;
            getRegion();
        }

        private static Image<Gray, Byte> FilterPlate(Image<Gray, Byte> plate)
        {
            Image<Gray, Byte> thresh = plate.ThresholdBinaryInv(new Gray(120), new Gray(255));

            Image<Gray, Byte> plateMask = new Image<Gray, byte>(plate.Size);
            Image<Gray, Byte> plateCanny = plate.Canny(new Gray(100), new Gray(50));
            MemStorage stor = new MemStorage();
            {
                plateMask.SetValue(255.0);
                for (
                   Contour<Point> contours = plateCanny.FindContours(
                      Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                      Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,
                      stor);
                   contours != null; contours = contours.HNext)
                {
                    Rectangle rect = contours.BoundingRectangle;
                    if (rect.Height > (plate.Height >> 1))
                    {
                        rect.X -= 1; rect.Y -= 1; rect.Width += 2; rect.Height += 2;
                        rect.Intersect(plate.ROI);

                        plateMask.Draw(rect, new Gray(0.0), -1);
                    }
                }

                thresh.SetValue(0, plateMask);
            }

            thresh._Erode(1);
            thresh._Dilate(1);

            return thresh;
        }

        private void getRegion()
        {
            Bgr col = new Bgr();
            Image<Bgr, Byte> imgReg = new Image<Bgr, byte>(img1.Width, img1.Height);
            for (int i = 0; i < img1.Height; i++)
            {
                for (int j = 0; j < img1.Width; j++)
                {
                    if (img5[i, j].Intensity == 255)
                    {
                        col = new Bgr(Color.LawnGreen);
                        imgReg[i, j] = col;
                    }
                    else
                    {
                        imgReg[i, j] = temp_load[i, j];
                    }
                }
            }

            imageBox5.Image = imgReg;

        }

    }
}
