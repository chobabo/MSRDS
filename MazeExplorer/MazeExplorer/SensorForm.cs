using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

using System.Diagnostics;

using OpenCvSharp;

//using cv = Emgu.CV;
//using Emgu.Util;
//using Emgu.CV.Structure;
//using Emgu.CV.UI;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

namespace MazeExplorer
{
    public partial class SensorForm : Form
    {
        private MazeExplorerOperations _mainPort;
        
        private IplImage src;

        public SensorForm(MazeExplorerOperations mainPort, string RobotName)
        {
            InitializeComponent();

            _mainPort = mainPort;

            RobotNameLabel.Text = RobotName;

            FileIo fi = new FileIo();

            // 物体ID->物体ファイル名のハッシュを作成
            ltf = fi.loadObjectId();
            // キーポイントの特徴ベクトルをobjMat行列にロード
            ncm = fi.loadDescription();

            //파티클 필터를 위한 상한값과 노이즈의 초기 선언
            initialPF();
            // パーティクルフィルタ
            pf = new pfilter(pNum, upper, lower, noise);
            pf2 = new pfilter(pNum, upper2, lower2, noise2);
            // 位置推定の出力用
            p = new Particle();
            p2 = new Particle();
        }

        // 데이터베이스에서 파일을 불러와서 이 형식들로 저장을 해주자!! 
        // LSH 비교를 위해 그런거야!! 
        static LoadToFile ltf;
        static NewCvMat ncm;

        // 파티클 필터를 위해서 노이즈와 상관계수의 상한과 하한을 선언하자
        LIMIT upper, lower, upper2, lower2;
        NOISE noise, noise2;
        const int pNum = 1000;

        public void initialPF()
        {
            int w = 600;
            int h = 150;

            upper.x = w;
            upper.y = h;
            upper.vx = w / 2;
            upper.vy = h / 2;

            lower.x = 0;
            lower.y = 0;
            lower.vx = -(w / 2);
            lower.vy = -(h / 2);

            noise.x = 50;
            noise.y = 50;
            noise.vx = 100;
            noise.vy = 100;

            upper2.x = w;
            upper2.y = h;
            upper2.vx = w / 2;
            upper2.vy = h / 2;

            lower2.x = 0;
            lower2.y = 0;
            lower2.vx = -(w / 2);
            lower2.vy = -(h / 2);

            noise2.x = 50;
            noise2.y = 50;
            noise2.vx = 100;
            noise2.vy = 100;
        }

        pfilter pf, pf2;
        Particle p, p2;

        //private IplImage _image1; 
        //public IplImage Image1
        //{
        //    get { return _image1; }
        //    set
        //    {
        //        _image1 = value;
        //        pBoxIpl1.ImageIpl = _image1;
        //    }
        //}

        //private int[] _tmpLRF = new int[180];
        //public int[] LRF
        //{
        //    get { return _tmpLRF; }
        //    set 
        //    {
        //        _tmpLRF = value;
        //        label1.Text = Convert.ToString(_tmpLRF[180]);
        //    }
        //}

        #region create to omnidirectional image and show at winform
        int OmniWidth = 600;
        int OmniHeight = 150;

        //integrated Image for omnidirectional view
        private Bitmap omniDirectionImage(Bitmap fImage, Bitmap eImage, Bitmap rImage, Bitmap wImage)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Bitmap img = new Bitmap(OmniWidth, OmniHeight);
            RectangleF sourceRect1 = new RectangleF(0, 0, rImage.Width, rImage.Height);
            RectangleF sourceRect2 = new RectangleF(75, 0, rImage.Width, rImage.Height);
            
            RectangleF destinationRect1 = new RectangleF(525, 0, rImage.Width, rImage.Height);
            RectangleF destinationRect2 = new RectangleF(0, 0, rImage.Width, rImage.Height);

            using (Graphics g = Graphics.FromImage(img))
            {
                g.DrawImage(rImage, destinationRect1, sourceRect1, GraphicsUnit.Pixel);
                g.DrawImage(wImage, 75, 0);
                g.DrawImage(fImage, 225, 0);
                g.DrawImage(eImage, 375, 0);
                g.DrawImage(rImage, destinationRect2, sourceRect2, GraphicsUnit.Pixel);
                watch.Stop();
                label10.Text = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";
                return img;
            }
        }
        #endregion

        //SurfAlgorithm SA = new SurfAlgorithm();
        //List<IPoint> ipts2 = new List<IPoint>();
        //static string sampleImg = "C:\\Users\\chowonjae\\Documents\\Visual Studio 2010\\Projects\\MRDS\\MazeExplorer\\MazeExplorer\\img1.bmp";
        //Bitmap testImg = new Bitmap(sampleImg);

        //Received bmp image data from pioneer robot and convert to OpenCv image format
        public void imageProcessing(Bitmap fImage, Bitmap eImage, Bitmap rImage, Bitmap wImage, string _filterName)
        {
            Stopwatch watchTotal = new Stopwatch();
            watchTotal.Start();

            //create omnidirectional image from cemera capture image
            Bitmap img = new Bitmap(OmniWidth, OmniHeight);
            img = omniDirectionImage(fImage, eImage, rImage, wImage);

            //Create List of interest points
            //List<IPoint> ipts1 = new List<IPoint>();
            //int MatchCnt = 0;

            //create IplImage for OpenCv processing using open Csharp
            IplImage ipl = new IplImage(img.Width, img.Height, BitDepth.U8, 3);
            ipl.CopyFrom(img);

            //create image for OpenCv processing using Emgu
            //cv.Image<Bgr, byte> eimg = new cv.Image<Bgr, byte>(img);

            //Filter of ImageProcess 
            ImageProcess ipImage = new ImageProcess();
            SURF sf = new SURF();
            CenterPnt ctp, ctp2;
            MatchInfo mi;

            switch (_filterName)
            {
                case "Gray":
                    src = ipImage.grayProcess(ipl);
                    break;

                case "Threshold":
                    src = ipImage.ThresholdProcess(ipl);
                    break;

                case "Canny":
                    src = ipImage.BuildCanny(ipl);
                    break;

                case "Sobel":
                    src = ipImage.BuildSobel(ipl);
                    break;

                case "Laplace":
                    src = ipImage.BuildLaplace(ipl);
                    break;

                case "Surf":

                    //---- using LSH ----//

                    //---- first loop
                    sf.ExtractFeaturesFromImg(ipl);
                    mi = sf.MatchUsingLsh(sf.KeyPoints, sf.DescriptorVal, ltf, ncm);
                    label14.Text = mi.fName;
                    label2.Text = mi.lshTime;
                    label4.Text = mi.des.Total + " points";
                    ctp = sf.InterestPointMatching(ipl, sf.KeyPoints, sf.DescriptorVal, mi.fName, CvColor.Yellow);
                    label16.Text = ctp.sTime;
                    label6.Text = ctp.des.Total + " points";
                    if (ctp.matchFeatures != 0)
                    {
                        label8.Text = ctp.matchFeatures + " points";
                    }

                    //src = ctp.src;

                    //---- Second Loop
                    sf.SecondMat(sf.KeyPoints, sf.DescriptorVal, ctp.rt);
                    mi = sf.MatchUsingLsh(sf.SecondKeyPoints, sf.SecondDescriptorVal, ltf, ncm);
                    label18.Text = mi.fName;
                    label19.Text = mi.lshTime;
                    label17.Text = mi.des.Total + " points";
                    ctp2 = sf.InterestPointMatching(ctp.src, sf.SecondKeyPoints, sf.SecondDescriptorVal, mi.fName, CvColor.Green);
                    label20.Text = ctp2.sTime;
                    label21.Text = ctp2.des.Total + " points";
                    if (ctp2.matchFeatures != 0)
                    {
                        label22.Text = ctp2.matchFeatures + " points";
                    }

                    src = ctp2.src;


                    
                    //----for particle filter----//
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    pf.predict();
                    pf.weight(ctp.rt.X + (ctp.rt.Width / 2), ctp.rt.Y + (ctp.rt.Height / 2));
                    pf.measure(p);
                    pf.resample();

                    for (int i = 0; i < pNum; i++)
                    {
                        Cv.Circle(src, pf.particles[i].get_x, pf.particles[i].get_y, 1, CvColor.Orange, Cv.FILLED);
                    }
                    // 物体位置（パーティクルの重心）推定結果の表示
                    Cv.Circle(src, p.get_x, p.get_y, 10, CvColor.Orange, Cv.FILLED);
                    watch.Stop();
                    label24.Text = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";

                    //---- second loop
                    Stopwatch watch2 = new Stopwatch();
                    watch2.Start();
                    pf2.predict();
                    pf2.weight(ctp2.rt.X + (ctp2.rt.Width / 2), ctp2.rt.Y + (ctp2.rt.Height / 2));
                    pf2.measure(p2);
                    pf2.resample();

                    for (int i = 0; i < pNum; i++)
                    {
                        Cv.Circle(src, pf2.particles[i].get_x, pf2.particles[i].get_y, 1, CvColor.Violet, Cv.FILLED);
                    }
                    // 物体位置（パーティクルの重心）推定結果の表示
                    Cv.Circle(src, p2.get_x, p2.get_y, 10, CvColor.Violet, Cv.FILLED);
                    watch2.Stop();
                    label25.Text = Convert.ToString(watch2.Elapsed.TotalMilliseconds) + "ms";
                    
                    

                    break;

                default:
                    src = ipl;
                    break;
            }            

            // Load an Image
            pBoxIpl1.ImageIpl = src;

            watchTotal.Stop();
            label26.Text = Convert.ToString(watchTotal.Elapsed.TotalMilliseconds) + "ms";
        }

        /// <summary>
        /// Width of robot.  change this value to change the red indicators in the laser views
        /// </summary>
        private static readonly int robotWidth = 400; //mm

        /// <summary>
        /// maximum distance the laser can see
        /// </summary>
        private static readonly int maxDist = 8000; //mm

        public void createTop(sicklrf.State stateType)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int height = 200;
            int width = 400;

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.LightGray);

            double angularOffset = -90 + stateType.AngularRange / 2.0;
            double piBy180 = Math.PI / 180.0;
            double halfAngle = stateType.AngularRange / 360.0 / 2.0;
            double scale = (double)height / (double)maxDist;

            GraphicsPath path = new GraphicsPath();

            for (int pass = 0; pass != 2; pass++)
            {
                for (int i = 0; i < stateType.DistanceMeasurements.Length; i++)
                {
                    int range = stateType.DistanceMeasurements[i];

                    if (range <= 0)
                        range = maxDist;

                    double angle = i * (stateType.AngularRange / 360.0) - angularOffset;
                    double lowAngle = (angle - halfAngle) * piBy180;
                    double highAngle = (angle + halfAngle) * piBy180;

                    double drange = range * scale;

                    float lx = (float)(height + drange * Math.Cos(lowAngle));
                    float ly = (float)(height - drange * Math.Sin(lowAngle));
                    float hx = (float)(height + drange * Math.Cos(highAngle));
                    float hy = (float)(height - drange * Math.Sin(highAngle));

                    if (pass == 0)
                    {
                        if (i == 0)
                        {
                            path.AddLine(height, height, lx, ly);
                        }
                        path.AddLine(lx, ly, hx, hy);
                    }
                    else
                    {
                        if (range < maxDist)
                        {
                            double myangle = i * Math.PI * stateType.AngularRange / stateType.DistanceMeasurements.Length / 180; //radians
                            double obsThresh = Math.Abs(robotWidth / (2 * Math.Cos(myangle)));
                            if (range < obsThresh)
                                g.DrawLine(Pens.Red, lx, ly, hx, hy);
                            else
                                g.DrawLine(Pens.DarkBlue, lx, ly, hx, hy);
                        }
                    }
                }

                if (pass == 0)
                {
                    g.FillPath(Brushes.White, path);
                }
            }

            float botWidth = (float)((robotWidth / 2.0) * scale);
            g.DrawLine(Pens.Red, height, height - botWidth, height, height);
            g.DrawLine(Pens.Red, height - 3, height - botWidth, height + 3, height - botWidth);
            g.DrawLine(Pens.Red, height - botWidth, height - 3, height - botWidth, height);
            g.DrawLine(Pens.Red, height + botWidth, height - 3, height + botWidth, height);
            g.DrawLine(Pens.Red, height - botWidth, height - 1, height + botWidth, height - 1);

            g.DrawString(
                stateType.TimeStamp.ToString(),
                new Font(FontFamily.GenericSansSerif, 10, GraphicsUnit.Pixel),
                Brushes.Gray, 0, 0
            );

            picLRFtop.Image = bmp;

            watch.Stop();
            label12.Text = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";
        }

        private void SensorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (src != null) Cv.ReleaseImage(src);
        }

        //private void groupBox4_Enter(object sender, EventArgs e)
        //{

        //}

        //private void label1_Click(object sender, EventArgs e)
        //{

        //}


    }
}
