using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenCvSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace MazeExplorer
{
    public partial class Database : Form
    {
        public Database()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FileIo fi = new FileIo();

            //Streamwriter을 이용해서 텍스트 파일을 생성하고 거기에 데이터를 보존한다.
            StreamWriter OBJ_FILE, DESC_FILE;
            OBJ_FILE = File.CreateText(path + OBJ_NAME);
            DESC_FILE = File.CreateText(path + DESC_NAME);

            //Process the list of files found in they contain.
            string[] fileEntries = Directory.GetFiles(path, "*.bmp");

            for (int i = 0; i < fileEntries.Length; i++)
            {
                //SURF 특징점 추출을 위해 SavetoFile의 구조체를 선언한다.
                SaveToFile _stf = new SaveToFile();

                //특징점을 추출하여 구조체에 저장한다.
                FileInfo fileInfo = new FileInfo(fileEntries[i]);   //파일정보를 지역변수로 초기화 시켜준다.
                string _name = fileInfo.Name;
                _stf = fi.extractSURF(path + _name);

                //두개의 텍스트 파일에 파일의 인덱스 정보와 특징점 정보를 각각 저장한다.
                fi.saveFile(i, _name, _stf, OBJ_FILE, DESC_FILE);

                _stf._imageDescriptors.Dispose();
                _stf._imageKeypoints.Dispose();
                _stf._storage.Dispose();
            }

            OBJ_FILE.Close();
            DESC_FILE.Close();
        }

        static string path = 
            "C:\\Users\\chowonjae\\Documents\\Visual Studio 2010\\Projects\\MRDS\\MazeExplorer\\MazeExplorer\\Database\\";

        //static int SURF_PARAM = 500;                    //SURF의 파라미터
        //static int DIM = 128;                           //SURF 특징량의 차원수
        static string OBJ_NAME = "object.txt";          //物体ID格納ファイル
        static string DESC_NAME = "description.txt";    //特徴量格納ファイル
      
    }
}
