using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace MazeExplorer
{
    class SurfAlgorithm
    {

        List<IPoint> ipts = new List<IPoint>();

        public List<IPoint> InterestPointExtraction(Bitmap img)
        {
            //Create Integral Image
            IntegralImage iimg = IntegralImage.FromImage(img);

            //Extract the interest Point
            //0.0001f --> low value, many feature points
            ipts = FastHessian.getIpoints(0.0001f, 5, 2, iimg);

            //Describe the interest points
            //3rd, false-->64bit and true 128bit
            SurfDescriptor.DecribeInterestPoints(ipts, false, false, iimg);

            return ipts;
        }
       
        public void PaintSURF(Bitmap img, List<IPoint>[] matches)
        {
            Graphics g = Graphics.FromImage(img);

            Pen redPen = new Pen(Color.Red);
            Pen bluePen = new Pen(Color.Blue);
            Pen myPen;

            foreach (IPoint ip in matches[0])
            {
                int S = 2 * Convert.ToInt32(2.5f * ip.scale);
                int R = Convert.ToInt32(S / 2f);

                Point pt = new Point(Convert.ToInt32(ip.x), Convert.ToInt32(ip.y));
                Point ptR = new Point(Convert.ToInt32(R * Math.Cos(ip.orientation)),
                             Convert.ToInt32(R * Math.Sin(ip.orientation)));

                myPen = (ip.laplacian > 0 ? bluePen : redPen);

                g.DrawEllipse(myPen, pt.X - R, pt.Y - R, S, S);
                g.DrawLine(new Pen(Color.FromArgb(0, 255, 0)), new Point(pt.X, pt.Y), new Point(pt.X + ptR.X, pt.Y + ptR.Y));
            }

            g.Dispose();
        }

        //----Matching of Original Image and Modeled Image----//
        private const float FLT_MAX = 3.402823466e+38F;        /* max value */

        public List<IPoint>[] InterestPointMatching(List<IPoint> ipts1, List<IPoint> ipts2)
        {
            double dist;
            double d1, d2;
            IPoint match = new IPoint();

            List<IPoint>[] matches = new List<IPoint>[2];
            matches[0] = new List<IPoint>();
            matches[1] = new List<IPoint>();

            for (int i = 0; i < ipts1.Count; i++)
            {
                d1 = d2 = FLT_MAX;

                for (int j = 0; j < ipts2.Count; j++)
                {
                    dist = GetDistance(ipts1[i], ipts2[j]);

                    if (dist < d1) // if this feature matches better than current best
                    {
                        d2 = d1;
                        d1 = dist;
                        match = ipts2[j];
                    }
                    else if (dist < d2) // this feature matches better than second best
                    {
                        d2 = dist;
                    }
                }

                // If match has a d1:d2 ratio < 0.65 ipoints are a match
                if (d1 / d2 < 0.77) //ﾔｽﾐ｡Matchｵ耿ｽﾉﾙ
                {
                    matches[0].Add(ipts1[i]);
                    matches[1].Add(match);
                }
            }
            return matches;
        }

        private static double GetDistance(IPoint ip1, IPoint ip2)
        {
            float sum = 0.0f;
            for (int i = 0; i < 64; ++i)
                sum += (ip1.descriptor[i] - ip2.descriptor[i]) * (ip1.descriptor[i] - ip2.descriptor[i]);
            return Math.Sqrt(sum);
        }
    }
}
