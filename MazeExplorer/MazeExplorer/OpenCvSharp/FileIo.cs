using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenCvSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace MazeExplorer
{
    public struct SaveToFile
    {
        public CvSeq<CvSURFPoint> _imageKeypoints;
        public CvSeq<float> _imageDescriptors;
        public CvMemStorage _storage;

        public SaveToFile(CvSeq<CvSURFPoint> imageKeypoints, CvSeq<float> imageDescriptors, CvMemStorage storage)
        {
            _imageKeypoints = imageKeypoints;
            _imageDescriptors = imageDescriptors;
            _storage = storage;
        }
    }

    public struct LoadToFile
    {
        public List<int> id;
        public List<string> filepath;

        public LoadToFile(List<int> _id, List<string> _filepath)
        {
            id = _id;
            filepath = _filepath;
        }
    }

    public struct NewCvMat
    {
        public List<int> labels;
        public List<int> laplacians;
        public CvMat objMat;

        public NewCvMat(List<int> _labels, List<int> _laplacians, CvMat _objMat)
        {
            labels = _labels;
            laplacians = _laplacians;
            objMat = _objMat;
        }
    }

    class FileIo
    {
        static string path =
            "C:\\Users\\chowonjae\\Documents\\Visual Studio 2010\\Projects\\MRDS\\MazeExplorer\\MazeExplorer\\Database\\";

        static int SURF_PARAM = 500;                    //SURF의 파라미터
        //static int DIM = 128;                           //SURF 특징량의 차원수
        static string OBJ_NAME = "object.txt";          //物体ID格納ファイル
        static string DESC_NAME = "description.txt";    //特徴量格納ファイル

        //--- SURF 특징점을 검출한다 ---//
        // param[in]    _filename            이미지 파일 이름
        // param[out]   _imageKeyPoints      키 포인트 (출력을 위해 참고로 보자)
        // param[out]   _imageDescriptors    키 포인트의 특징량 (출력을 위해 참고로 보자)
        // param[out]   _storage             Memory Storage (출력을 위해 참고로 보자)
        public SaveToFile extractSURF(string _filename)
        {
            SaveToFile stf;

            using (IplImage img = new IplImage(_filename, LoadMode.GrayScale))
            {
                if (img == null)
                {
                    //label1.Text = "Cannot load image file in Folder";
                }

                // SURFの処理
                stf._storage = Cv.CreateMemStorage(0);
                CvSURFParams param = new CvSURFParams(SURF_PARAM, true);

                Cv.ExtractSURF(img, null, out stf._imageKeypoints, out stf._imageDescriptors, stf._storage, param);

                return stf;
            }
        }

        //---물체 모델을 파일에 보존한다.---//
        //param[in]     _objId          오브젝트ID
        //param[in]     _filename       이미지 파일의 이름
        //param[in]     _stf            이미지로 부터 추출한 특징점 정보가 저장되어 있는 구조체
        //param[in]     _obj            streamwriter을 이용하여 아이디와 파일 경로를 저장한다.
        //param[in]     _desc           streamwriter을 이용하여 특징점 정보를 저장한다.
        public void saveFile(int _objId, string _filename, SaveToFile _stf, StreamWriter _obj, StreamWriter _desc)
        {
            // 物体IDファイルへ登録
            _obj.Write(_objId);
            _obj.Write(",");
            _obj.Write(_filename);
            _obj.Write("\r\n");

            CvSeqReader<CvSURFPoint> kreader = new CvSeqReader<CvSURFPoint>();
            Cv.StartReadSeq(_stf._imageKeypoints, kreader);

            CvSeqReader<float> reader = new CvSeqReader<float>();
            Cv.StartReadSeq(_stf._imageDescriptors, reader);

            int length = (int)(_stf._imageDescriptors.ElemSize / sizeof(float));

            // オブジェクトID, ラプラシアン, 128個の数字をタブ区切りで出力
            for (int i = 0; i < _stf._imageDescriptors.Total; i++)
            {
                // 各キーポイントの特徴量に対し
                // オブジェクトID
                _desc.Write(_objId);
                _desc.Write(",");

                // 特徴点のラプラシアン（SURF特徴量ではベクトルの比較時に使用）
                CvSURFPoint kp = CvSURFPoint.FromPtr(kreader.Ptr);
                IntPtr descriptor = reader.Ptr;

                Cv.NEXT_SEQ_ELEM(kreader.Seq.ElemSize, kreader);
                Cv.NEXT_SEQ_ELEM(reader.Seq.ElemSize, reader);

                _desc.Write(kp.Laplacian);
                _desc.Write(",");

                float[] d1 = new float[length];
                Marshal.Copy(descriptor, d1, 0, length);

                //for debug
                //label1.Text = Convert.ToString(length);

                // 128次元ベクトル
                for (int j = 0; j < length; j++)
                {
                    _desc.Write(d1[j]);

                    if (j != (length - 1))
                    {
                        _desc.Write(",");
                    }
                }

                _desc.Write("\r\n");
            }
        }

        /// <summary>
        /// 파일을 로드해서 아이디와 파일 이름을 불러온다.
        /// </summary>
        /// Read From a OBJ.txt File
        /// <returns>LoadtToFile</returns>
        public LoadToFile loadObjectId()
        {
            LoadToFile ltf;
            ltf.id = new List<int>();
            ltf.filepath = new List<string>();

            try
            {
                // create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(path + OBJ_NAME))
                {
                    string line;
                    //read and display lines from the file until the end of
                    //the file is reached
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] split = line.Split(new char[] { ',' });
                        ltf.id.Add(Convert.ToInt32(split[0]));
                        ltf.filepath.Add(split[1]);
                    }
                }
            }
            catch (Exception e)
            {
                //Let the user Know what went wrong.
                //label1.Text = "The file could not be read:" + e.Message;
                string error = e.Message;
            }

            return ltf;
        }

        /// <summary>
        /// 파일을 로드해서 Cvmat 형식으로 저장한다.
        /// キーポイントのラベル（抽出元の物体ID）とラプラシアンと特徴ベクトルをロードしlabelsとobjMatへ格納
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>LoadtToFile</returns>
        static int DIM = 128;

        public NewCvMat loadDescription()
        {
            // キーポイントの特徴ベクトルをobjMat行列にロード
            NewCvMat cvm;
            cvm.labels = new List<int>();
            cvm.laplacians = new List<int>();

            // create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            List<string> records = new List<string>();
            using (StreamReader sr = new StreamReader(path + DESC_NAME))
            {
                string record;
                while ((record = sr.ReadLine()) != null)
                {
                    records.Add(record);
                }
            }
            int numKeyPoints = records.Count();
            cvm.objMat = Cv.CreateMat(numKeyPoints, DIM, MatrixType.F32C1);
            double[,] arrMat = new double[numKeyPoints, DIM];

            int cur = 0;
            foreach (string r in records)
            {
                string[] split = r.Split(new char[] { ',' });
                cvm.labels.Add(Convert.ToInt32(split[0]));
                cvm.laplacians.Add(Convert.ToInt32(split[1]));

                // DIM次元ベクトルの要素を行列へ格納
                for (int j = 0; j < DIM; j++)
                {
                    arrMat[cur, j] = Convert.ToDouble(split[j + 2]);
                }
                cur++;
            }
            cvm.objMat = CvMat.FromArray(arrMat);
            records.Clear();

            return cvm;
        }
    }
}
