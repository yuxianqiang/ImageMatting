using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Matting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            img = Cv2.ImRead(@"Image\greenImage3.jpg");
            imgBack = Cv2.ImRead(@"Image\background.jpg");
            Cv2.CvtColor(img, hsv, ColorConversionCodes.BGR2HSV);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadImage();
        }

        Mat img = new Mat();
        Mat imgBack = new Mat();
        Mat dilate = new Mat();
        Mat hsv = new Mat();
        Mat mask = new Mat();
        Mat erode = new Mat();
        Mat copyBack = new Mat();
        Mat medianBlur = new Mat();
        public void ReadImage()
        {
            imgBack.CopyTo(copyBack);
            label1.Text = "hmin:"+ trackBar1.Value;
            label2.Text = "smin:" + trackBar2.Value;
            label3.Text = "vmin:" + trackBar3.Value;
            label4.Text = "hmax:" + trackBar4.Value;
            label5.Text = "smax:" + trackBar5.Value;
            label6.Text = "vmax:" + trackBar6.Value;

            Scalar lowerGreen = new Scalar(trackBar1.Value, trackBar2.Value, trackBar3.Value);
            Scalar upperGreen = new Scalar(trackBar4.Value, trackBar5.Value, trackBar6.Value);

            Cv2.InRange(hsv, lowerGreen, upperGreen, mask);

            Cv2.MedianBlur(mask, medianBlur, 7);
            //Cv2.ImShow("mask", medianBlur);

            //Cv2.Erode(mask, erode, new Mat());
            //Cv2.Dilate(erode, dilate, new Mat());
            //开运算
            Cv2.MorphologyEx(medianBlur, erode, MorphTypes.Open, new Mat(), new OpenCvSharp.Point(), 0);
            //闭运算
            Cv2.MorphologyEx(erode, dilate, MorphTypes.Close, new Mat(), new OpenCvSharp.Point(), 0);
           
            //Cv2.ImShow("dilate", dilate);

            ConvolutionImage(dilate, img, copyBack);
        }

        unsafe void ConvolutionImage(Mat maskTemp, Mat imgGreen, Mat imgBackTemp)
        {
            int cols = imgBackTemp.Cols* imgBackTemp.Channels();
            int rows = imgBackTemp.Rows;

            for (int i = 0; i < rows; i++)
            {
                byte* byteGreen = (byte*) imgGreen.Ptr(i).ToPointer();
                byte* byteBack = (byte*) imgBackTemp.Ptr(i).ToPointer();
                byte* byteMask = (byte*) maskTemp.Ptr(i).ToPointer();

                for (int j = 0; j < cols; j++)
                {
                    if (byteMask[j/3] != 255)
                    {
                        byteBack[j] = byteGreen[j];
                    }
                }
            }
            //Cv2.ImShow("imgBack", imgBackTemp); 
            pictureBox1.Image = BitmapConverter.ToBitmap(imgBackTemp);
        }
    }
}

