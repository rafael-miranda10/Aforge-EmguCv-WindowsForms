using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filter;
        VideoCaptureDevice device;
        VideoCapture capture = new VideoCapture(); //create a camera capture
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDetect_Click(object sender, System.EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_newFrame;
            device.Start();
        }

        private void Device_newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            //Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            //foreach (Rectangle rectangle in rectangles)
            //{
            //    using (Graphics graphics = Graphics.FromImage(bitmap))
            //    {
            //        using (Pen pen = new Pen(Color.Red, 5))
            //        {
            //            graphics.DrawRectangle(pen, rectangle);
            //        }
            //    }
            //}
            //pic.Image = bitmap;


            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> originalImage = new Image<Bgr, byte>(bitmap);
            Image<Bgr, byte> GaussianImage = originalImage.SmoothGaussian(11);
            Image<Gray, byte> originalImgGray = originalImage.Convert<Gray, byte>();
            Image<Gray, byte> gaussianImgGray = GaussianImage.Convert<Gray, byte>();


            //Canny
            // Image<Gray, byte> originalFilteredImage = originalImgGray.Canny(20, 50);
            // Image<Gray, byte> gaussianFilteredImage = gaussianImgGray.Canny(20, 50);

            //Sobel
            //Image<Gray, float> originalFilteredImage = originalImgGray.Sobel(1, 0, 5);
            //Image<Gray, float> gaussianFilteredImage = originalImgGray.Sobel(1, 0, 3).Sobel(0,1,3).AbsDiff(new Gray(0.0));

            //Laplace
            //Image<Gray, float> originalFilteredImage = originalImgGray.Laplace(9);
            //Image<Gray, float> gaussianFilteredImage = originalImgGray.Laplace(7);


            //ThresholdBinary
            //Image<Gray, byte> originalFilteredImage = originalImgGray.ThresholdBinary(new Gray(45), new Gray(255));
            //Image<Gray, byte> gaussianFilteredImage = gaussianImgGray.ThresholdBinary(new Gray(30), new Gray(255));


            Image<Gray, byte> originalFilteredImage = new Image<Gray, byte>(originalImgGray.Width, originalImgGray.Height, new Gray(0));
            Image<Gray, byte> gaussianFilteredImage = new Image<Gray, byte>(gaussianImgGray.Width, gaussianImgGray.Height, new Gray(0));
            CvInvoke.Threshold(originalImgGray, originalFilteredImage, 125, 255,Emgu.CV.CvEnum.ThresholdType.Otsu);
            CvInvoke.Threshold(gaussianImgGray, gaussianFilteredImage, 125, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);

            // https://stackoverflow.com/questions/25989754/what-is-the-function-to-find-otsu-threshold-in-emgu-cv





            pic.Image = originalImage.Bitmap;
            pic2.Image = originalFilteredImage.Bitmap;
            pic3.Image = gaussianFilteredImage.Bitmap;


        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
            {
                cboDevice.Items.Add(device.Name);
            }
            cboDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
            {
                device.Stop();
            }
        }
    }
}
