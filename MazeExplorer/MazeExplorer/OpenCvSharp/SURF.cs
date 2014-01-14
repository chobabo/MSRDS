using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MazeExplorer
{
    /// <summary>
    /// SURF(Speeded Up Robust Features)による対応点検出        
    /// </summary>
    /// <remarks>samples/c/find_obj.cpp から改変</remarks>
    /// 

    public struct CenterPnt
    {
        public IplImage src;
        public string sTime;
        public CvSeq<CvSURFPoint> key;
        public CvSeq<float> des;
        public int matchFeatures;
        public CvRect rt;

        public CenterPnt
            (IplImage _src, string _sTime, CvSeq<CvSURFPoint> _key,
            CvSeq<float> _des, int _matchFeatures, CvRect _rt)
        {
            src = _src;
            sTime = _sTime;
            key = _key;
            des = _des;
            matchFeatures = _matchFeatures;
            rt = _rt;
        }
    }

    public struct MatchInfo
    {
        public CvSeq<CvSURFPoint> key;
        public CvSeq<float> des;
        public string fName;
        public string lshTime;

        public MatchInfo(CvSeq<CvSURFPoint> _key, CvSeq<float> _des, string _fName, string _lshTime)
        {
            key = _key;
            des = _des;
            fName = _fName;
            lshTime = _lshTime;
        }
    }

    class SURF
    {
        //for display of result values
        private float _imageDescriptors;
        public float ImageDescriptors
        {
            get { return _imageDescriptors; }
            set
            {
                _imageDescriptors = value;
            }
        }

        private float _objectDescriptors;
        public float ObjectDescriptors
        {
            get { return _objectDescriptors; }
            set
            {
                _objectDescriptors = value;
            }
        }

        private CvSeq<CvSURFPoint> _keyPoints;
        public CvSeq<CvSURFPoint> KeyPoints
        {
            get { return _keyPoints; }
            set
            {
                _keyPoints = value;
            }
        }

        private CvSeq<float> _descriptorVal;
        public CvSeq<float> DescriptorVal
        {
            get { return _descriptorVal; }
            set 
            {
                _descriptorVal = value;
            }
        }

        private CvSeq<CvSURFPoint> _secondkeyPoints;
        public CvSeq<CvSURFPoint> SecondKeyPoints
        {
            get { return _secondkeyPoints; }
            set
            {
                _secondkeyPoints = value;
            }
        }

        private CvSeq<float> _seconddescriptorVal;
        public CvSeq<float> SecondDescriptorVal
        {
            get { return _seconddescriptorVal; }
            set
            {
                _seconddescriptorVal = value;
            }
        }

        IplImage obj;   //obj-> modeled img
        CvMemStorage storage;

        public void Dispose()
        {
            //memory dispose --> erase
            //if (result != null) Cv.ReleaseImage(result);
            if (obj != null) obj.Dispose();
            if (storage != null) storage.Dispose();
        }

        public SURF()
        {
            //obj = new IplImage(path + dbImg, LoadMode.GrayScale);
            //storage = Cv.CreateMemStorage(0);
        }

        static string path =
                "C:\\Users\\chowonjae\\Documents\\Visual Studio 2010\\Projects\\MRDS\\MazeExplorer\\MazeExplorer\\Database\\";

        /// <summary>
        /// SURF을 이용하여 특징점을 매칭하고 이미지를 반환
        /// </summary>
        /// <param name="src">IplImage</param>>
        /// <returns>IplImage</returns>
        public CenterPnt InterestPointMatching
            (IplImage src, CvSeq<CvSURFPoint> imageKeypoints, CvSeq<float> imageDescriptors, string _filename, CvColor clr)
        {
            CenterPnt ctp;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            // cvExtractSURF
            // SURFで対応点検出
            using (storage = Cv.CreateMemStorage(0))
            using (obj = new IplImage(path + _filename, LoadMode.GrayScale))
            {
                // SURFの処理
                CvSeq<CvSURFPoint> objectKeypoints;
                CvSeq<float> objectDescriptors;

                CvSURFParams param = new CvSURFParams(500, true);  //default threadhole value is 500.
                Cv.ExtractSURF(obj, null, out objectKeypoints, out objectDescriptors, storage, param);
                //Cv.ExtractSURF(image, null, out imageKeypoints, out imageDescriptors, storage, param);
                ctp.key = objectKeypoints;
                ctp.des = objectDescriptors;

                // 特徴点の場所に円を描く
                int[] ptpairs = FindPairs(objectKeypoints, objectDescriptors, imageKeypoints, imageDescriptors);

                if (ptpairs.Length != 0)
                {
                    ctp.matchFeatures = ptpairs.Length / 2;
                }
                else
                {
                    ctp.matchFeatures = 0;
                }

                CvPoint[] points = new CvPoint[ptpairs.Length / 2];
                int j = 0;
                for (int i = 0; i < ptpairs.Length; i += 2)
                {
                    //--original source--//
                    //CvSURFPoint r1 = Cv.GetSeqElem<CvSURFPoint>(objectKeypoints, ptpairs[i]).Value;
                    //CvSURFPoint r2 = Cv.GetSeqElem<CvSURFPoint>(imageKeypoints, ptpairs[i + 1]).Value;
                    //Cv.Line(correspond, r1.Pt, new CvPoint(Cv.Round(r2.Pt.X), Cv.Round(r2.Pt.Y + obj.Height)), CvColor.White);

                    CvSURFPoint r = Cv.GetSeqElem<CvSURFPoint>(imageKeypoints, ptpairs[i + 1]).Value;
                    points[j] = new CvPoint()
                    {
                        X = (int)r.Pt.X,
                        Y = (int)r.Pt.Y + (src.Height / 2)
                    };
                    //CvPoint center = new CvPoint(Cv.Round(r.Pt.X), Cv.Round(r.Pt.Y));
                    int radius = Cv.Round(r.Size * (1.2 / 9.0) * 2);
                    Cv.Circle(src, points[j], radius, clr, 1, LineType.AntiAlias, 0);
                    j++;
                }
                CvRect rect = Cv.BoundingRect(points);
                
                src.Rectangle(new CvPoint(rect.X, rect.Y), new CvPoint(rect.X + rect.Width, rect.Y + rect.Height), clr, 1);
                
                //ctp.x = rect.X + (rect.Width / 2);
                //ctp.y = rect.Y + (rect.Height / 2);
                ctp.rt = rect;
                ctp.src = src.Clone();
                
                watch.Stop();
                ctp.sTime = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";

                return ctp;
            }
        }

        private CvMat CreateData(int n, int d, CvMat data)
        {
            CvMat db = new CvMat(n, d, MatrixType.F32C1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < d; j++)
                {
                    if ((i < data.Rows) && (j < data.Cols))
                    {
                        db[i, j] = data[i, j];
                    }
                    else
                    {
                        db[i, j] = 0;
                    }
                }
            }

            return db;
        }

        /// <summary>
        /// 쿼리 이미지에서 특징점과 서술자를 추출한다.
        /// </summary>
        public void ExtractFeaturesFromImg(IplImage src)
        { 
            using(storage = Cv.CreateMemStorage(0))
            // SURFで対応点検出
            using (IplImage image = new IplImage(src.Size, BitDepth.U8, 1))
            using (IplImage correspond = Cv.CreateImage(new CvSize(src.Width, src.Height / 2), BitDepth.U8, 1))
            {
                Cv.CvtColor(src, image, ColorConversion.BgrToGray);
                //(0,75) 좌표부터 가로 600, 세로150 부분을 관심영역으로 설정한다.
                Cv.SetImageROI(image, new CvRect(0, image.Height / 2, image.Width, image.Height));
                Cv.Copy(image, correspond);
                Cv.ResetImageROI(correspond);

                CvSURFParams param = new CvSURFParams(500, true);  //default threadhole value is 500.
                Cv.ExtractSURF(correspond, null, out _keyPoints, out _descriptorVal, storage, param);
            }
        }

        /// <summary>
        /// 처음 매칭한 포인트를 제거하고 새롭게 keyPoint, Descriptor 생성
        /// </summary>
        public void SecondMat(CvSeq<CvSURFPoint> keyPoints, CvSeq<float> descriptors, CvRect rect)
        {
            int tmpcnt = 0;
            CvSeqReader<CvSURFPoint> kreader = new CvSeqReader<CvSURFPoint>();
            CvSeqReader<float> reader = new CvSeqReader<float>();

            _secondkeyPoints = Cv.CloneSeq(keyPoints, null);
            _seconddescriptorVal = Cv.CloneSeq(descriptors, null);

            Cv.StartReadSeq(KeyPoints, kreader);
            Cv.StartReadSeq(descriptors, reader);

            for (int i = 0; i < descriptors.Total; i++ )
            {
                CvSURFPoint kp = CvSURFPoint.FromPtr(kreader.Ptr);
                IntPtr desc = reader.Ptr;

                int cnt = 0;

                if ((kp.Pt.X >= rect.X) && (kp.Pt.X <= rect.X + rect.Width))
                {

                    if ((kp.Pt.Y + 75 >= rect.Y) && (kp.Pt.Y + 75 <= rect.Y + rect.Height))
                    {
                        cnt++;
                    }
                }

                if (cnt != 0)
                {
                    _secondkeyPoints.Remove(tmpcnt);
                    _seconddescriptorVal.Remove(tmpcnt);
                }
                else
                {
                    tmpcnt++;                 
                }

                Cv.NEXT_SEQ_ELEM(kreader.Seq.ElemSize, kreader);
                Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);
            }
        }


        /// <summary>
        /// LSH을 이용하여 데이터베이스에서 특징점 매칭
        /// </summary>
        /// <param name="src">IplImage</param>>
        /// <returns>IplImage</returns>
        public MatchInfo MatchUsingLsh(CvSeq<CvSURFPoint> _keyPoints, CvSeq<float> _descriptorVal, LoadToFile ltf, NewCvMat ncm)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            MatchInfo mi;
            mi.key = _keyPoints;
            mi.des = _descriptorVal;

            int D = 128;        //데이터의 차원 수(SURF 특징량이므로 128)
            //int N = 100;       //해시 테이블의 크기
            //int L = 5;          //해시 테이블의 숫자
            //int K = 64;         //해시 키 차원 수
            
            // 物体モデルデータベースをインデキシング
            // constructs a LSH table           
            using(CvMat data = CreateData(ncm.objMat.Rows, D, ncm.objMat))
            using (CvLSH lsh = Cv.CreateMemoryLSH(D, ncm.objMat.Rows, 5, 64, MatrixType.F32C1))
            {
                //adds vectors to the LSH
                Cv.LSHAdd(lsh, data);
                
                // クエリからSURF特徴量を抽出
                //CvSeq<CvSURFPoint> queryKeypoints;
                //CvSeq<float> queryDescriptors;

                // 投票箱を用意
                int numObjects = ltf.id.Count();
                int[] votes = new int[numObjects];
                votes.Initialize();

                // クエリのキーポイントの特徴ベクトルをCvMatに展開
                CvMat queryMat = new CvMat(_descriptorVal.Total, D, MatrixType.F32C1);
                CvSeqReader<float> reader = new CvSeqReader<float>();
                Cv.StartReadSeq(_descriptorVal, reader);
   
                float[,] d1 = new float[_descriptorVal.Total, D];

                for (int i = 0; i < _descriptorVal.Total; i++)
                {
                    IntPtr descriptor = reader.Ptr;
                    Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);

                    float[] d2 = new float[D];
                    Marshal.Copy(descriptor, d2, 0, D);

                    for (int j = 0; j < D; j++)
                    {
                        d1[i, j] = d2[j];
                    }

                }
                queryMat = CvMat.FromArray(d1);

                // kd-treeで1-NNのキーポイントインデックスを検索
                int kd = 1;
                CvMat indices = new CvMat(_keyPoints.Total, kd, MatrixType.S32C1);
                CvMat dists = new CvMat(_keyPoints.Total, kd, MatrixType.F64C1);
                Cv.LSHQuery(lsh, queryMat, indices, dists, kd, 100);

                //_matchName = Convert.ToString(indices[1].Val0);

                // 1-NNキーポイントを含む物体に得票
                for (int i = 0; i < indices.Rows; i++)
                {
                    int idx = (int)indices[i].Val0;
                    if (idx >= 0)
                    {
                        votes[ncm.labels[idx]]++;
                    }
                }

                // 投票数が最大の物体IDを求める
                int maxId = -1;
                int maxVal = -1;
                for (int i = 0; i < numObjects; i++)
                {
                    if (votes[i] > maxVal)
                    {
                        maxId = i;
                        maxVal = votes[i];
                    }
                }

                mi.fName = ltf.filepath[maxId];

                watch.Stop();
                mi.lshTime = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";

                return mi;
            }
        }

        /// <summary>
        /// KDTree을 이용하여 데이터베이스에서 특징점 매칭
        /// LSH 처럼 최적화를 해줘야 한다. 아직 안했다^^
        /// </summary>
        /// <param name="src">IplImage</param>>
        /// <returns>IplImage</returns>
        public string Kdtree(IplImage src)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (storage = Cv.CreateMemStorage(0))

            // SURFで対応点検出
            using (IplImage image = new IplImage(src.Size, BitDepth.U8, 1))
            {
                Cv.CvtColor(src, image, ColorConversion.BgrToGray);

                //_matchName = "start";
                int D = 128;
                string result;
                FileIo fi = new FileIo();

                // 物体ID->物体ファイル名のハッシュを作成
                LoadToFile ltf = fi.loadObjectId();
                // キーポイントの特徴ベクトルをobjMat行列にロード
                NewCvMat ncm = fi.loadDescription();

                // 物体モデルデータベースをインデキシング
                // constructs a LSH table
                CvFeatureTree ft = Cv.CreateKDTree(ncm.objMat);

                //_matchName = "load of KD data struct!!";

                // クエリからSURF特徴量を抽出
                CvSeq<CvSURFPoint> queryKeypoints;
                CvSeq<float> queryDescriptors;

                CvSURFParams param = new CvSURFParams(500, true);  //default threadhole value is 500.
                Cv.ExtractSURF(image, null, out queryKeypoints, out queryDescriptors, storage, param);

                // 投票箱を用意
                int numObjects = ltf.id.Count();
                int[] votes = new int[numObjects];
                votes.Initialize();

                CvMat queryMat = new CvMat(queryDescriptors.Total, D, MatrixType.F32C1);
                CvSeqReader<float> reader = new CvSeqReader<float>();
                Cv.StartReadSeq(queryDescriptors, reader);

                float[,] d1 = new float[queryDescriptors.Total, D];

                for (int i = 0; i < queryDescriptors.Total; i++)
                {
                    IntPtr descriptor = reader.Ptr;
                    Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);

                    float[] d2 = new float[D];
                    Marshal.Copy(descriptor, d2, 0, D);

                    for (int j = 0; j < D; j++)
                    {
                        d1[i, j] = d2[j];
                    }

                }
                queryMat = CvMat.FromArray(d1);

                // kd-treeで1-NNのキーポイントインデックスを検索
                int kd = 1;
                CvMat indices = new CvMat(queryKeypoints.Total, kd, MatrixType.S32C1);
                CvMat dists = new CvMat(queryKeypoints.Total, kd, MatrixType.F64C1);
                Cv.FindFeatures(ft, queryMat, indices, dists, kd, 250);

                // 1-NNキーポイントを含む物体に得票
                for (int i = 0; i < indices.Rows; i++)
                {
                    int idx = (int)indices[i].Val0;
                    votes[ncm.labels[idx]]++;
                }

                // 投票数が最大の物体IDを求める
                int maxId = -1;
                int maxVal = -1;
                for (int i = 0; i < numObjects; i++)
                {
                    if (votes[i] > maxVal)
                    {
                        maxId = i;
                        maxVal = votes[i];
                    }
                }

                //_matchName = ltf.filepath[maxId];
                result = ltf.filepath[maxId];

                watch.Stop();
                //_lshTime = Convert.ToString(watch.Elapsed.TotalMilliseconds) + "ms";

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1Ptr">Cではconst float*</param>
        /// <param name="d2Ptr">Cではconst float*</param>
        /// <param name="best"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static unsafe double CompareSurfDescriptors(IntPtr d1Ptr, IntPtr d2Ptr, double best, int length)
        {
            //Debug.Assert(length % 4 == 0);

            double totalCost = 0;

            // ポインタでのアクセスの代わりに配列にコピーしてからやる。            
            /*float[] d1 = new float[length];
            float[] d2 = new float[length];
            Marshal.Copy(d1Ptr, d1, 0, length);
            Marshal.Copy(d2Ptr, d2, 0, length);*/

            // 遅くて問題ならunsafeとか
            float* d1 = (float*)d1Ptr;
            float* d2 = (float*)d2Ptr;

            double t0, t1, t2, t3;
            for (int i = 0; i < length; i += 4)
            {
                t0 = d1[i] - d2[i];
                t1 = d1[i + 1] - d2[i + 1];
                t2 = d1[i + 2] - d2[i + 2];
                t3 = d1[i + 3] - d2[i + 3];
                totalCost += t0 * t0 + t1 * t1 + t2 * t2 + t3 * t3;
                if (totalCost > best)
                    break;
            }

            return totalCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec">Cではconst float*</param>
        /// <param name="laplacian"></param>
        /// <param name="model_keypoints"></param>
        /// <param name="model_descriptors"></param>
        /// <returns></returns>
        private static int NaiveNearestNeighbor(IntPtr vec, int laplacian, CvSeq<CvSURFPoint> model_keypoints, CvSeq<float> model_descriptors)
        {
            int length = (int)(model_descriptors.ElemSize / sizeof(float));
            int neighbor = -1;
            double dist1 = 1e6, dist2 = 1e6;        //이 부분이 유클리디안 디스턴스 임계값이다.
            //double dist1 = 2e6, dist2 = 2e6;
            CvSeqReader<float> reader = new CvSeqReader<float>();
            CvSeqReader<CvSURFPoint> kreader = new CvSeqReader<CvSURFPoint>();
            Cv.StartReadSeq(model_keypoints, kreader, false);
            Cv.StartReadSeq(model_descriptors, reader, false);

            IntPtr mvec;
            CvSURFPoint kp;
            double d;
            for (int i = 0; i < model_descriptors.Total; i++)
            {
                // const CvSURFPoint* kp = (const CvSURFPoint*)kreader.ptr; が結構曲者。
                // OpenCvSharpの構造体はFromPtrでポインタからインスタンス生成できるようにしてるので、こう書ける。
                kp = CvSURFPoint.FromPtr(kreader.Ptr);
                // まともにキャストする場合はこんな感じか
                // CvSURFPoint kp = (CvSURFPoint)Marshal.PtrToStructure(kreader.Ptr, typeof(CvSURFPoint));  

                mvec = reader.Ptr;
                Cv.NEXT_SEQ_ELEM(kreader.Seq.ElemSize, kreader);
                Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);
                if (laplacian != kp.Laplacian)
                {
                    continue;
                }
                d = CompareSurfDescriptors(vec, mvec, dist2, length);
                if (d < dist1)
                {
                    dist2 = dist1;
                    dist1 = d;
                    neighbor = i;
                }
                else if (d < dist2)
                    dist2 = d;
            }
            if (dist1 < 0.6 * dist2)
                return neighbor;
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectKeypoints"></param>
        /// <param name="objectDescriptors"></param>
        /// <param name="imageKeypoints"></param>
        /// <param name="imageDescriptors"></param>
        /// <returns></returns>
        private static int[] FindPairs(CvSeq<CvSURFPoint> objectKeypoints, CvSeq<float> objectDescriptors, CvSeq<CvSURFPoint> imageKeypoints, CvSeq<float> imageDescriptors)
        {
            CvSeqReader<float> reader = new CvSeqReader<float>();
            CvSeqReader<CvSURFPoint> kreader = new CvSeqReader<CvSURFPoint>();
            Cv.StartReadSeq(objectDescriptors, reader);
            Cv.StartReadSeq(objectKeypoints, kreader);

            List<int> ptpairs = new List<int>();

            for (int i = 0; i < objectDescriptors.Total; i++)
            {
                CvSURFPoint kp = CvSURFPoint.FromPtr(kreader.Ptr);
                IntPtr descriptor = reader.Ptr;
                Cv.NEXT_SEQ_ELEM(kreader.Seq.ElemSize, kreader);
                Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);
                int nearestNeighbor = NaiveNearestNeighbor(descriptor, kp.Laplacian, imageKeypoints, imageDescriptors);
                if (nearestNeighbor >= 0)
                {
                    ptpairs.Add(i);
                    ptpairs.Add(nearestNeighbor);
                }
            }
            return ptpairs.ToArray();
        }

        /// <summary>
        /// a rough implementation for object location
        /// </summary>
        /// <param name="objectKeypoints"></param>
        /// <param name="objectDescriptors"></param>
        /// <param name="imageKeypoints"></param>
        /// <param name="imageDescriptors"></param>
        /// <param name="srcCorners"></param>
        /// <returns></returns>
        private static CvPoint[] LocatePlanarObject(CvSeq<CvSURFPoint> objectKeypoints, CvSeq<float> objectDescriptors,
                            CvSeq<CvSURFPoint> imageKeypoints, CvSeq<float> imageDescriptors,
                            CvPoint[] srcCorners)
        {
            CvMat h = new CvMat(3, 3, MatrixType.F64C1);
            int[] ptpairs = FindPairs(objectKeypoints, objectDescriptors, imageKeypoints, imageDescriptors);
            int n = ptpairs.Length / 2;
            if (n < 4)
                return null;

            CvPoint2D32f[] pt1 = new CvPoint2D32f[n];
            CvPoint2D32f[] pt2 = new CvPoint2D32f[n];
            for (int i = 0; i < n; i++)
            {
                pt1[i] = (Cv.GetSeqElem<CvSURFPoint>(objectKeypoints, ptpairs[i * 2])).Value.Pt;
                pt2[i] = (Cv.GetSeqElem<CvSURFPoint>(imageKeypoints, ptpairs[i * 2 + 1])).Value.Pt;
            }

            CvMat pt1Mat = new CvMat(1, n, MatrixType.F32C2, pt1);
            CvMat pt2Mat = new CvMat(1, n, MatrixType.F32C2, pt2);
            if (Cv.FindHomography(pt1Mat, pt2Mat, h, HomographyMethod.Ransac, 5) == 0)
                return null;

            CvPoint[] dstCorners = new CvPoint[4];
            for (int i = 0; i < 4; i++)
            {
                double x = srcCorners[i].X;
                double y = srcCorners[i].Y;
                double Z = 1.0 / (h[6] * x + h[7] * y + h[8]);
                double X = (h[0] * x + h[1] * y + h[2]) * Z;
                double Y = (h[3] * x + h[4] * y + h[5]) * Z;
                dstCorners[i] = new CvPoint(Cv.Round(X), Cv.Round(Y));
            }

            return dstCorners;
        }
    }
}
