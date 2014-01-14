using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace MazeExplorer
{
    class ImageProcess : IDisposable
    {
        IplImage subgray;

        //Convert to Gray Image
        public IplImage grayProcess(IplImage src)
        {
            subgray = new IplImage(src.Size, BitDepth.U8, 1);
            Cv.CvtColor(src, subgray, ColorConversion.BgrToGray);
            return subgray;
        }

        //Binary Process
        public IplImage ThresholdProcess(IplImage src)
        {
            subgray = new IplImage(src.Size, BitDepth.U8, 1);       //create the memory
            Cv.CvtColor(src, subgray, ColorConversion.BgrToGray);   //convert to gray
            Cv.Smooth(subgray, subgray, SmoothType.Gaussian, 5);    //interval of gaussian smooth
            Cv.Threshold(subgray, subgray, 120, 255, ThresholdType.Binary);  //threshold value = 120
            return subgray;
        }

        //Canny edge
        public IplImage BuildCanny(IplImage src)
        {
            subgray = new IplImage(src.Size, BitDepth.U8, 1);
            Cv.CvtColor(src, subgray, ColorConversion.BgrToGray);

            Cv.Canny(subgray, subgray, 80, 255);

            return subgray;
        }

        //Sobel edge
        public IplImage BuildSobel(IplImage src)
        {
            subgray = new IplImage(src.Size, BitDepth.U8, 1);
            Cv.CvtColor(src, subgray, ColorConversion.BgrToGray);

            Cv.Sobel(subgray, subgray, 1, 0, ApertureSize.Size3);

            return subgray;
        }

        //Laplace edge
        public IplImage BuildLaplace(IplImage src)
        {
            subgray = new IplImage(src.Size, BitDepth.U8, 1);

            using (IplImage temp = new IplImage(src.Size, BitDepth.S16, 1))
            
            using (IplImage graytemp = new IplImage(src.Size, BitDepth.U8, 1))
            {
                Cv.CvtColor(src, graytemp, ColorConversion.BgrToGray);

                Cv.Laplace(graytemp, temp);
                Cv.ConvertScaleAbs(temp, subgray);
                return subgray;
            }
        }

        public void Dispose()
        {
            if (subgray != null) Cv.ReleaseImage(subgray);
        }
    }
}
