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
//using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace MazeExplorer
{
    public partial class MazeForm : Form
    {
        private MazeExplorerOperations _mainPort;

        public bool camStart = false;
        public bool lrfStart = false;
        public bool trackerStart = false;
        public string filterName;

        //drive.DriveOperations _drivePort;
        //drive.DriveOperations _drivePort2;
        //drive.DriveOperations _drivePort3;

        MazeGrid mazeGrid = new MazeGrid();

        long startTicks = 0;
        long endTicks = 0;

        public PointF RobotPosition
        {
            get { return mazeGrid.RobotPosition; }
            set
            {
                r1PoseX.Text = Convert.ToString(mazeGrid.RobotPosition.X);
                r1PoseZ.Text = Convert.ToString(mazeGrid.RobotPosition.Y);

                mazeGrid.RobotPosition = value;
                mazeGrid.Invalidate();
            }
        }

        public PointF RobotPosition2
        {
            get { return mazeGrid.RobotPosition2; }
            set
            {
                r2PoseX.Text = Convert.ToString(mazeGrid.RobotPosition2.X);
                r2PoseZ.Text = Convert.ToString(mazeGrid.RobotPosition2.Y);

                mazeGrid.RobotPosition2 = value;
                mazeGrid.Invalidate();
            }
        }

        public PointF RobotPosition3
        {
            get { return mazeGrid.RobotPosition3; }
            set
            {
                r3PoseX.Text = Convert.ToString(mazeGrid.RobotPosition3.X);
                r3PoseZ.Text = Convert.ToString(mazeGrid.RobotPosition3.Y);

                mazeGrid.RobotPosition3 = value;
                mazeGrid.Invalidate();
            }
        }

        public MazeForm(MazeExplorerOperations mainPort)
        {
            InitializeComponent();

            _mainPort = mainPort;
            _tState.PositionHistory = new List<SimplePositionState>();
        }

        //public void SetDrivePort(drive.DriveOperations drivePort, drive.DriveOperations drivePort2, drive.DriveOperations drivePort3)
        //{
        //    _drivePort = drivePort;
        //    _drivePort2 = drivePort2;
        //    _drivePort3 = drivePort3;
        //}

        private void MakeDefaultState()
        {
            mazeGrid.GridDrawMode = 0;
            mazeGrid.MergeDisplayMode = false;

            btnDrawMode.BackColor = Color.White;
            btnEditMode.BackColor = Color.White;
            btnTarget.BackColor = Color.White;
            btnRobot.BackColor = Color.White;
            btnRobot2.BackColor = Color.White;
            btnRobot3.BackColor = Color.White;
            Pos1.BackColor = Color.White;
            Pos2.BackColor = Color.White;
            Pos3.BackColor = Color.White;

            btnDrawMode.ForeColor = Color.Gray;
            btnEditMode.ForeColor = Color.Gray;
            btnTarget.ForeColor = Color.Gray;
            btnRobot.ForeColor = Color.Gray;
            btnRobot2.ForeColor = Color.Gray;
            btnRobot3.ForeColor = Color.Gray;
        }

        private void MazeForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            mazeGrid.Left = 15;
            mazeGrid.Top = 60;
            mazeGrid.Width = 501;
            mazeGrid.Height = 501;

            this.Controls.Add(mazeGrid);

            mazeGrid.CleareGrid();

            MakeDefaultState();

            btnDrawMode_Click(null, null);
            btnDrawMode.Focus();
        }

        private void btnDrawMode_Click(object sender, EventArgs e)
        {
            if (btnDrawMode.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnDrawMode.BackColor = Color.Yellow;
                btnDrawMode.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 1;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void btnEditMode_Click(object sender, EventArgs e)
        {
            if (btnEditMode.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnEditMode.BackColor = Color.Yellow;
                btnEditMode.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 2;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void btnClearMap_Click(object sender, EventArgs e)
        {
            mazeGrid.CleareGrid(); 
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            saveDlg.FileName = "MazeData";
            saveDlg.DefaultExt = "txt";
            saveDlg.Filter = "Text files (*.txt)|*.txt";


            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveDlg.FileName))
                {
                    sw.WriteLine(mazeGrid.WallWidth.ToString() + ":" + mazeGrid.WallHeight.ToString());

                    //Start position
                    sw.WriteLine(mazeGrid.StartPosition.X.ToString() + ":" + mazeGrid.StartPosition.Y.ToString());

                    //Targte position
                    sw.WriteLine(mazeGrid.TargetPosition.X.ToString() + ":" + mazeGrid.TargetPosition.Y.ToString());

                    string lineStr = "";
                    for (int j = 0; j < mazeGrid.WallHeight; j++)
                    {
                        lineStr = "";

                        for (int i = 0; i < mazeGrid.WallWidth; i++)
                        {
                            lineStr = lineStr + mazeGrid.statusMap[i, j].ToString() + ":";
                            lineStr = lineStr + "0";
                        }
                        sw.WriteLine(lineStr);
                    }
                    sw.Close();
                }
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            openDlg.FileName = "MazeData";
            openDlg.DefaultExt = "txt";
            openDlg.Filter = "Text files (*.txt)|*.txt";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                mazeGrid.CleareGrid();

                using (StreamReader sr = new StreamReader(openDlg.FileName))
                {
                    string line = sr.ReadLine();

                    string[] arr;
                    arr = line.Split(new Char[] { ':' }, 2);

                    int width = System.Int16.Parse(arr[0]);
                    int height = System.Int16.Parse(arr[1]);

                    line = sr.ReadLine();
                    arr = line.Split(new Char[] { ':' }, 2);
                    Point sp = new Point(System.Int16.Parse(arr[0]), System.Int16.Parse(arr[1]));
                    mazeGrid.StartPosition = sp;

                    line = sr.ReadLine();
                    arr = line.Split(new Char[] { ':' }, 2);
                    Point tp = new Point(System.Int16.Parse(arr[0]), System.Int16.Parse(arr[1]));
                    mazeGrid.TargetPosition = tp;

                    string[] arrLine;
                    for (int j = 0; j < height; j++)
                    {
                        line = sr.ReadLine();
                        arrLine = line.Split(new Char[] { ':' }, width + 1);

                        for (int i = 0; i < width; i++)
                        {
                            mazeGrid.statusMap[i, j] = System.Int16.Parse(arrLine[i]);
                        }
                    }
                }

                mazeGrid.CalcTargetRegion();

                mazeGrid.Invalidate();
            }
        }

        private void btnSubmitMap_Click(object sender, EventArgs e)
        {
            MakeDefaultState();

            mazeGrid.MergeBox();

            float _x, _y, _w, _h;
            float gridScale = 0.2f;

            List<BoxState> NewBoxEntityList = new List<BoxState>();

            //Submit wall
            for (int i = 0; i < mazeGrid.MergedBoxList.Count; i++)
            {
                if (mazeGrid.MergedBoxList[i].Direction == 'H')
                {
                    _x = mazeGrid.MergedBoxList[i].StartPoint.X * gridScale;
                    _y = mazeGrid.MergedBoxList[i].StartPoint.Y * gridScale;

                    _w = mazeGrid.MergedBoxList[i].Length * gridScale;
                    _h = gridScale;
                }
                else
                {
                    _x = mazeGrid.MergedBoxList[i].StartPoint.X * gridScale;
                    _y = mazeGrid.MergedBoxList[i].StartPoint.Y * gridScale;

                    _w = gridScale;
                    _h = mazeGrid.MergedBoxList[i].Length * gridScale;
                }

                BoxState _state = new BoxState();

                _state.BoxType = "Wall";

                _state.StartPosX = _x - 5f;
                _state.StartPosZ = _y - 5f;

                _state.EndPosX = _x + _w - 5f;
                _state.EndPosZ = _y + _h - 5f;

                NewBoxEntityList.Add(_state);
            }

            //Submit target position
            if (mazeGrid.TargetPosition.X > 0 && mazeGrid.TargetPosition.Y > 0)
            {
                BoxState _tstate = new BoxState();

                _tstate.BoxType = "Target";

                _tstate.StartPosX = (mazeGrid.TargetPosition.X * 0.02f) - 5.0f;
                _tstate.StartPosZ = (mazeGrid.TargetPosition.Y * 0.02f) - 5.0f;

                _tstate.EndPosX = 0;
                _tstate.EndPosZ = 0;

                NewBoxEntityList.Add(_tstate);
            }

            //Submit start position
            if (mazeGrid.StartPosition.X > 0 && mazeGrid.StartPosition.Y > 0)
            {
                BoxState _sstate = new BoxState();

                _sstate.BoxType = "Start";

                _sstate.StartPosX = (mazeGrid.StartPosition.X * 0.02f) - 5.0f;
                _sstate.StartPosZ = (mazeGrid.StartPosition.Y * 0.02f) - 5.0f;

                _sstate.EndPosX = 0;
                _sstate.EndPosZ = 0;

                NewBoxEntityList.Add(_sstate);
            }

            //Submit start position
            if (mazeGrid.StartPosition2.X > 0 && mazeGrid.StartPosition2.Y > 0)
            {
                BoxState _s2state = new BoxState();

                _s2state.BoxType = "Start2";

                _s2state.StartPosX = (mazeGrid.StartPosition2.X * 0.02f) - 5.0f;
                _s2state.StartPosZ = (mazeGrid.StartPosition2.Y * 0.02f) - 5.0f;

                _s2state.EndPosX = 0;
                _s2state.EndPosZ = 0;

                NewBoxEntityList.Add(_s2state);
            }

            //Submit start position
            if (mazeGrid.StartPosition3.X > 0 && mazeGrid.StartPosition3.Y > 0)
            {
                BoxState _s3state = new BoxState();

                _s3state.BoxType = "Start3";

                _s3state.StartPosX = (mazeGrid.StartPosition3.X * 0.02f) - 5.0f;
                _s3state.StartPosZ = (mazeGrid.StartPosition3.Y * 0.02f) - 5.0f;

                _s3state.EndPosX = 0;
                _s3state.EndPosZ = 0;

                NewBoxEntityList.Add(_s3state);
            }

            _mainPort.Post(new InsertBox(NewBoxEntityList));            
        }

        private void btnSetDrive_Click(object sender, EventArgs e)
        {
            camStart = true;
            lrfStart = true;
            trackerStart = true;

            //float _left = (float)System.Double.Parse(txtLeft.Text);
            //float _right = (float)System.Double.Parse(txtRight.Text);

            //_drivePort.EnableDrive(new drive.EnableDriveRequest(true));
            //_drivePort.SetDrivePower(new drive.SetDrivePowerRequest(_left, _right));

            //Set drive enable
            MazeExplorerState state = new MazeExplorerState();
            state.IsEnableOfDiffentiaDrive = true;
            _mainPort.Post(new UpdateMazeState(state));

            startTicks = System.DateTime.Now.Ticks;
            timer1.Enabled = true;
        }

        private void btnSetRobot_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            btnStop_Click(null, null);

            RobotPosition rPos = new RobotPosition();
            rPos.IsInitialPositon = true;
            rPos.IsInitialPositon2 = true;
            rPos.IsInitialPositon3 = true;

            _mainPort.Post(new MoveRobotPosition(rPos));  
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //_drivePort.EnableDrive(new drive.EnableDriveRequest(false));
            //_drivePort.SetDrivePower(new drive.SetDrivePowerRequest(0, 0));

            //SetDriveSpeed("FirstRobot(R1)", 0, 0);
            //SetDriveSpeed("SecondRobot(R2)", 0, 0);
            //SetDriveSpeed("ThirdRobot(R3)", 0, 0);

            //EnableDisableSetOfDrive("FirstRobot(R1)", false);
            //EnableDisableSetOfDrive("SecondRobot(R2)", false);
            //EnableDisableSetOfDrive("ThirdRobot(R3)", false);

            //Set drive disable
            MazeExplorerState state = new MazeExplorerState();
            state.IsEnableOfDiffentiaDrive = false;
            _mainPort.Post(new UpdateMazeState(state));


            endTicks = System.DateTime.Now.Ticks;

            long diffTime = (endTicks - startTicks) / 10000;

            int minutes = (int)(diffTime / 60000);
            int seconds = (int)((diffTime % 60000) / 1000);
            long milisec = (diffTime % 60000) % 1000;

            labelTimer.Text = minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0') + "." + milisec.ToString().PadRight(3, '0');
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mazeGrid.TargetPosition.X >= 0 && mazeGrid.TargetPosition.Y >= 0)
            {
                if (mazeGrid.IsTargetTouched)
                {
                    timer1.Enabled = false;
                    btnStop_Click(null, null);

                    MessageBox.Show("Robot has been reached in the target");
                }
            }

            endTicks = System.DateTime.Now.Ticks;

            long diffTime = (endTicks - startTicks) / 10000;

            int minutes = (int)(diffTime / 60000);
            int seconds = (int)((diffTime % 60000) / 1000);
            long milisec = (diffTime % 60000) % 1000;

            labelTimer.Text = minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0') + "." + milisec.ToString().PadRight(3, '0');

        }

        private void btnRobot_Click(object sender, EventArgs e)
        {
            if (btnRobot.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnRobot.BackColor = Color.Yellow;
                btnRobot.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 4;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            if (btnTarget.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnTarget.BackColor = Color.Yellow;
                btnTarget.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 3;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        //private void MazeForm_MouseDown(object sender, MouseEventArgs e)
        //{

        //}

        //private void label8_Click(object sender, EventArgs e)
        //{

        //}

        private void btnRobot2_Click(object sender, EventArgs e)
        {
            if (btnRobot2.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnRobot2.BackColor = Color.Yellow;
                btnRobot2.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 5;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void btnRobot3_Click(object sender, EventArgs e)
        {
            if (btnRobot3.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                btnRobot3.BackColor = Color.Yellow;
                btnRobot3.ForeColor = Color.Black;
                mazeGrid.GridDrawMode = 6;
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        public TrackerState _tState = new TrackerState();
        private Tracker _traker = new Tracker();

        private void btnTracker_Click(object sender, EventArgs e)
        {
            //_traker.SendJpeg(_tState, "FirstRobotLogger.jpg");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _traker.SendJpeg(_tState, "FirstRobotLogger.jpg");
        }

        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Gray";
        }

        private void thresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Threshold";
        }

        private void cannyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Canny";
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Sobel";
        }

        private void laplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Laplace";
        }

        private void sURFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterName = "Surf";
        }

        private Point startPos(int x, int y)
        {
            Point _tmpPos = new Point(-1000, -1000);

            _tmpPos.X = x;
            _tmpPos.Y = y;

            return _tmpPos;
        }

        private void Pos1_Click(object sender, EventArgs e)
        {
            if (Pos1.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                Pos1.BackColor = Color.Yellow;
                Pos1.ForeColor = Color.Black;

                mazeGrid.StartPosition = startPos(Convert.ToInt32(XPos1.Text), Convert.ToInt32(YPos1.Text));
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void Pos2_Click(object sender, EventArgs e)
        {
            if (Pos2.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                Pos2.BackColor = Color.Yellow;
                Pos2.ForeColor = Color.Black;

                mazeGrid.StartPosition2 = startPos(Convert.ToInt32(XPos2.Text), Convert.ToInt32(YPos2.Text));
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void Pos3_Click(object sender, EventArgs e)
        {
            if (Pos3.BackColor != Color.FromArgb(255, 128, 255))
            {
                MakeDefaultState();

                Pos3.BackColor = Color.Yellow;
                Pos3.ForeColor = Color.Black;

                mazeGrid.StartPosition3 = startPos(Convert.ToInt32(XPos3.Text), Convert.ToInt32(YPos3.Text));
            }
            else
            {
                MakeDefaultState();
            }

            mazeGrid.Invalidate();
        }

        private void dATABASEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Database db = new Database();
            db.Show();
        }

        //private void readyToDrive_Click(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();

        //    EnableDisableSetOfDrive(_select, true);
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();
        //    _leftWheelSpeed = 0;
        //    _rightWheelSpeed = 0;

        //    SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        //    EnableDisableSetOfDrive(_select, false);
        //}

        //void EnableDisableSetOfDrive(string _select, bool _boolSelect)
        //{
        //    if (_select == "FirstRobot(R1)")
        //    {
        //        _drivePort.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
        //    }
        //    else if (_select == "SecondRobot(R2)")
        //    {
        //        _drivePort2.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
        //    }
        //    else if (_select == "ThirdRobot(R3)")
        //    {
        //        _drivePort3.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
        //    } 
        //}

        //void SetDriveSpeed(string _select, double _leftSpeed, double _rightSpeed)
        //{
        //    if (_select == "FirstRobot(R1)")
        //    {
        //        _drivePort.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
        //    }
        //    else if (_select == "SecondRobot(R2)")
        //    {
        //        _drivePort2.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
        //    }
        //    else if (_select == "ThirdRobot(R3)")
        //    {
        //        _drivePort3.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
        //    } 
        //}

        //double _leftWheelSpeed = 0;
        //double _rightWheelSpeed = 0;

        //private void numericGoAhead_ValueChanged(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();
        //    int num = Convert.ToInt32(numericGoAhead.Value);

        //    _leftWheelSpeed = num * 0.1;
        //    _rightWheelSpeed = num * 0.1;

        //    SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        //}

        //private void numericBack_ValueChanged(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();
        //    int num = Convert.ToInt32(numericBack.Value);

        //    _leftWheelSpeed = num * -0.1;
        //    _rightWheelSpeed = num * -0.1;

        //    SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        //}

        //private void numericLeft_ValueChanged(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();
        //    int num = Convert.ToInt32(numericLeft.Value);

        //    _leftWheelSpeed = num * 0.1;
        //    //_rightWheelSpeed = num * 0.1;

        //    SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        //}

        //private void numericRight_ValueChanged(object sender, EventArgs e)
        //{
        //    string _select = comboBox1.SelectedItem.ToString();
        //    int num = Convert.ToInt32(numericRight.Value);

        //    //_leftWheelSpeed = num * 0.1;
        //    _rightWheelSpeed = num * 0.1;

        //    SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        //}
    }

    public class MazeGrid : PictureBox
    {
        public class MergedBoxClass
        {
            public char Direction = 'H'; //H: Horizontal, V: Vertical
            public Point StartPoint = new Point();
            public int Length = 0;
        }

        const int wallWidth = 50;
        const int wallHeight = 50;
        const int gridScale = 10;

        public bool MergeDisplayMode = false;

        private bool mousePressedFlag = false;
        public int GridDrawMode = 0; //0: default, 1: Build wall, 2: Erase

        public int MousePX = 0;
        public int MousePY = 0;

        public short[,] statusMap = new short[wallWidth, wallHeight];
        public short[,] statusMerge = new short[wallWidth, wallHeight];
        GraphicsPath[,] gridPath = new GraphicsPath[wallWidth, wallHeight];

        private List<MergedBoxClass> _MergedBoxList = new List<MergedBoxClass>();

        public List<MergedBoxClass> MergedBoxList
        {
            get { return _MergedBoxList; }
            set { _MergedBoxList = value; }
        }

        private PointF _robotPosition = new PointF(-999999.0f, -999999.0f);
        public PointF RobotPosition
        {
            get { return _robotPosition; }
            set { _robotPosition = value; }
        }

        private PointF _robotPosition2 = new PointF(-999999.0f, -999999.0f);
        public PointF RobotPosition2
        {
            get { return _robotPosition2; }
            set { _robotPosition2 = value; }
        }

        private PointF _robotPosition3 = new PointF(-999999.0f, -999999.0f);
        public PointF RobotPosition3
        {
            get { return _robotPosition3; }
            set { _robotPosition3 = value; }
        }

        private Point _targetPosition = new Point(-1000, -1000);
        public Point TargetPosition
        {
            get { return _targetPosition; }
            set { _targetPosition = value; }
        }

        private Point _startPosition = new Point(250, 470);
        public Point StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        private Point _startPosition2 = new Point(300, 470);
        public Point StartPosition2
        {
            get { return _startPosition2; }
            set { _startPosition2 = value; }
        }

        private Point _startPosition3 = new Point(200, 470);
        public Point StartPosition3
        {
            get { return _startPosition3; }
            set { _startPosition3 = value; }
        }

        //path
        Rectangle _robotPoints = new Rectangle();
        Rectangle _robotPoints2 = new Rectangle();
        Rectangle _robotPoints3 = new Rectangle();
        PointF[] _targetPoints = new PointF[5];

        //GraphicsPath and Region
        GraphicsPath _targetPath = new GraphicsPath();
        Region _targetRegion = new Region();

        public bool IsTargetTouched = false;

        public MazeGrid()
            : base()
        {
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);

            PointF[] _points = new PointF[5];

            for (int i = 0; i < wallWidth; i++)
                for (int j = 0; j < wallHeight; j++)
                {
                    //Define path for filling area
                    _points[0] = new PointF(i * gridScale, j * gridScale);
                    _points[1] = new PointF(i * gridScale, j * gridScale + gridScale);
                    _points[2] = new PointF(i * gridScale + gridScale, j * gridScale + gridScale);
                    _points[3] = new PointF(i * gridScale + gridScale, j * gridScale);
                    _points[4] = new PointF(i * gridScale, j * gridScale);

                    gridPath[i, j] = new GraphicsPath();
                    gridPath[i, j].AddLines(_points);
                    gridPath[i, j].CloseFigure();
                }
        }

        public int WallWidth
        {
            get { return wallWidth; }
        }

        public int WallHeight
        {
            get { return wallHeight; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Point _s = new Point(0, 0);
            Point _e = new Point(0, 0);

            for (int i = 0; i < wallWidth * gridScale; i += gridScale)
            {
                //Vertical Line
                _s.X = i;
                _s.Y = 0;

                _e.X = i;
                _e.Y = wallHeight * gridScale;

                e.Graphics.DrawLine(Pens.White, _s, _e);
            }

            for (int i = 0; i < wallHeight * gridScale; i += gridScale)
            {
                //Horizontal line
                _s.X = 0;
                _s.Y = i;

                _e.X = wallWidth * gridScale;
                _e.Y = i;

                e.Graphics.DrawLine(Pens.White, _s, _e);
            }

            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, wallWidth * gridScale, wallHeight * gridScale));

            if (mousePressedFlag)
            {
                Point _pointInd = GetIndexFromPoint(MousePX, MousePY);

                if (GridDrawMode == 1)
                    statusMap[_pointInd.X, _pointInd.Y] = 1;
                else if (GridDrawMode == 2)
                    statusMap[_pointInd.X, _pointInd.Y] = 0;
            }

            //Display selected boxes
            for (int i = 0; i < wallWidth; i++)
                for (int j = 0; j < wallHeight; j++)
                {
                    if (statusMap[i, j] == 1)
                        e.Graphics.FillPath(Brushes.Green, gridPath[i, j]);
                }

            //Display merged rectangles
            if (MergeDisplayMode)
            {
                int _x, _y, _w, _h;

                for (int i = 0; i < _MergedBoxList.Count; i++)
                {
                    if (_MergedBoxList[i].Direction == 'H')
                    {
                        _x = _MergedBoxList[i].StartPoint.X * gridScale;
                        _y = _MergedBoxList[i].StartPoint.Y * gridScale;

                        _w = _MergedBoxList[i].Length * gridScale;
                        _h = gridScale;
                    }
                    else
                    {
                        _x = _MergedBoxList[i].StartPoint.X * gridScale;
                        _y = _MergedBoxList[i].StartPoint.Y * gridScale;

                        _w = gridScale;
                        _h = _MergedBoxList[i].Length * gridScale;
                    }

                    e.Graphics.DrawRectangle(Pens.Red, new Rectangle(_x, _y, _w, _h));
                }
            }

            //Display start point
            e.Graphics.FillEllipse(Brushes.Yellow, _startPosition.X - 16, _startPosition.Y - 16, 32, 32);
            e.Graphics.FillEllipse(Brushes.Maroon, _startPosition2.X - 16, _startPosition2.Y - 16, 32, 32);
            e.Graphics.FillEllipse(Brushes.Moccasin, _startPosition3.X - 16, _startPosition3.Y - 16, 32, 32);

            //Display target point
            if (_targetPosition.X >= 0 && _targetPosition.Y >= 0)
            {
                e.Graphics.FillEllipse(Brushes.Blue, _targetPosition.X - 16, _targetPosition.Y - 16, 32, 32);
            }

            //---Diaplsy robot on the grid---
            DisplayRobotOnTheGrid(_robotPosition.X, _robotPosition.Y, 1, ref e);
            DisplayRobotOnTheGrid(_robotPosition2.X, _robotPosition2.Y, 2, ref e);
            DisplayRobotOnTheGrid(_robotPosition3.X, _robotPosition3.Y, 3, ref e);

            if (_targetRegion.IsVisible(_robotPoints))
                IsTargetTouched = true;
            else
                IsTargetTouched = false;
        }

        //---Diaplsy robot on the grid---
        private void DisplayRobotOnTheGrid(float PositionX, float PositionY,int RobotNum, ref PaintEventArgs e)
        {
            if (PositionX > -999999.0f && PositionY > -999999.0f)
            {
                int px = (int)((PositionX + 5.0f) / 0.02f);
                int py = (int)((PositionY + 5.0f) / 0.02f);

                if (RobotNum == 1)
                {
                    e.Graphics.FillEllipse(Brushes.Red, px - gridScale, py - gridScale, gridScale * 2, gridScale * 2);

                    //calc robot region
                    _robotPoints.X = px - gridScale;
                    _robotPoints.Y = py - gridScale;
                    _robotPoints.Width = gridScale * 2;
                    _robotPoints.Height = gridScale * 2;
                }
                else if (RobotNum == 2)
                {
                    e.Graphics.FillEllipse(Brushes.DarkViolet, px - gridScale, py - gridScale, gridScale * 2, gridScale * 2);

                    //calc robot region
                    _robotPoints2.X = px - gridScale;
                    _robotPoints2.Y = py - gridScale;
                    _robotPoints2.Width = gridScale * 2;
                    _robotPoints2.Height = gridScale * 2;
                }
                else if (RobotNum == 3)
                {
                    e.Graphics.FillEllipse(Brushes.LightPink, px - gridScale, py - gridScale, gridScale * 2, gridScale * 2);

                    //calc robot region
                    _robotPoints3.X = px - gridScale;
                    _robotPoints3.Y = py - gridScale;
                    _robotPoints3.Width = gridScale * 2;
                    _robotPoints3.Height = gridScale * 2; 
                }
            }           
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressedFlag)
            {
                MousePX = e.X;
                MousePY = e.Y;

                this.Invalidate();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            mousePressedFlag = true;

            MousePX = e.X;
            MousePY = e.Y;

            if (GridDrawMode == 3)
            {
                _targetPosition.X = e.X;
                _targetPosition.Y = e.Y;

                CalcTargetRegion();
            }
            else if (GridDrawMode == 4)
            {
                _startPosition.X = e.X;
                _startPosition.Y = e.Y;
            }
            else if (GridDrawMode == 5)
            {
                _startPosition2.X = e.X;
                _startPosition2.Y = e.Y;
            }
            else if (GridDrawMode == 6)
            {
                _startPosition3.X = e.X;
                _startPosition3.Y = e.Y;
            }
            this.Invalidate();

        }

        public void CalcTargetRegion()
        {
            _targetPoints[0] = new PointF(_targetPosition.X - 15, _targetPosition.Y - 15);
            _targetPoints[1] = new PointF(_targetPosition.X + 15, _targetPosition.Y - 15);
            _targetPoints[2] = new PointF(_targetPosition.X + 15, _targetPosition.Y + 15);
            _targetPoints[3] = new PointF(_targetPosition.X - 15, _targetPosition.Y + 15);
            _targetPoints[4] = new PointF(_targetPosition.X - 15, _targetPosition.Y - 15);

            _targetPath.Reset();
            _targetPath.AddLines(_targetPoints);
            _targetPath.CloseFigure();

            _targetRegion.Dispose();
            _targetRegion = new Region(_targetPath);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            mousePressedFlag = false;
            this.Invalidate();
        }

        private Point GetIndexFromPoint(int x, int y)
        {
            int indX = x / gridScale;
            int indY = y / gridScale;

            if (indX < 0) indX = 0;
            if (indX >= wallWidth) indX = wallWidth - 1;

            if (indY < 0) indY = 0;
            if (indY >= wallHeight) indY = wallHeight - 1;

            Point _point = new Point(indX, indY);

            return _point;
        }

        public void CleareGrid()
        {
            for (int i = 0; i < wallWidth; i++)
                for (int j = 0; j < wallHeight; j++)
                    statusMap[i, j] = 0;

            //Draw outer wall
            for (int i = 0; i < wallWidth; i++)
            {
                statusMap[i, 0] = 1;
                statusMap[i, wallHeight - 1] = 1;
            }

            for (int i = 0; i < wallHeight; i++)
            {
                statusMap[0, i] = 1;
                statusMap[wallWidth - 1, i] = 1;
            }

            _targetPosition.X = -1000;
            _targetPosition.Y = -1000;

            MergeDisplayMode = false;

            this.Invalidate();
        }

        public void MergeBox()
        {
            int _horLength = 0;
            int _verLength = 0;

            _MergedBoxList.Clear();

            for (int i = 0; i < wallWidth; i++)
                for (int j = 0; j < wallHeight; j++)
                    statusMerge[i, j] = 0;


            for (int j = 0; j < wallHeight; j++)
                for (int i = 0; i < wallWidth; i++)
                {
                    if (statusMap[i, j] == 1 && statusMerge[i, j] != 1)
                    {
                        _horLength = 0;
                        _verLength = 0;

                        //Check horizontal direction
                        for (int k = i; k < wallWidth; k++)
                        {
                            if (statusMap[k, j] == 1 && statusMerge[k, j] != 1)
                                _horLength++;
                            else
                                break;
                        }

                        //Check vertical direction
                        for (int k = j; k < wallHeight; k++)
                        {
                            if (statusMap[i, k] == 1 && statusMerge[i, k] != 1)
                                _verLength++;
                            else
                                break;
                        }

                        if (_horLength >= _verLength)
                        {
                            MergedBoxClass mb = new MergedBoxClass();
                            mb.Direction = 'H';
                            mb.StartPoint = new Point(i, j);
                            mb.Length = _horLength;
                            _MergedBoxList.Add(mb);

                            for (int k = 0; k < _horLength; k++)
                                statusMerge[k + i, j] = 1;
                        }
                        else
                        {
                            MergedBoxClass mb = new MergedBoxClass();
                            mb.Direction = 'V';
                            mb.StartPoint = new Point(i, j);
                            mb.Length = _verLength;
                            _MergedBoxList.Add(mb);

                            for (int k = 0; k < _verLength; k++)
                                statusMerge[i, k + j] = 1;
                        }
                    }
                }

            MergeDisplayMode = true;
            this.Invalidate();
        }
    }
}
