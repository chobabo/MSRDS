using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;

using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace MazeExplorer
{
    public partial class ControllerForm : Form
    {
        //initialiation for pc to robot communication using serial port 
        System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();

        //Create Serial Winform
        SerialCommunication SC = new SerialCommunication();

        private MazeExplorerOperations _mainPort;

        drive.DriveOperations _drivePort;
        drive.DriveOperations _drivePort2;
        drive.DriveOperations _drivePort3;

        double _leftWheelSpeed = 0;
        double _rightWheelSpeed = 0;

        public ControllerForm(MazeExplorerOperations mainPort)
        {
            InitializeComponent();

            _mainPort = mainPort;
        }

        #region Control Hardware Motor
        //set of the velocity of hardware robot
        public byte rightVelocity;

        private bool comportCheck()
        {
            if (port.IsOpen == false)
            {
                MessageBox.Show("COM port is not open.. please check your com Port");
            }
            return port.IsOpen;
        }

        private void singleServoControl(byte rightVelocity)
        {
            if (comportCheck())
            {
                string str = Convert.ToString(rightVelocity) + ",";
                char[] servo = str.ToCharArray();
                byte[] bServo = new byte[4];
                bServo.Initialize();

                for (int i = 0; i < servo.Length; i++)
                {
                    bServo[i] = Convert.ToByte(servo[i]);
                }

                port.Write(bServo, 0, bServo.Length);

                //byte[] servo = new byte[2];

                //servo[0] = Convert.ToByte(rightVelocity);
                //servo[1] = Convert.ToByte(',');

                //port.DiscardInBuffer(); //buffer clear
                //port.DiscardOutBuffer(); //buffer clear

                //port.Write(servo, 0, servo.Length);

            }
        }
        #endregion

        public void SetDrivePort(drive.DriveOperations drivePort, drive.DriveOperations drivePort2, drive.DriveOperations drivePort3)
        {
            _drivePort = drivePort;
            _drivePort2 = drivePort2;
            _drivePort3 = drivePort3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();

            EnableDisableSetOfDrive(_select, true);
        }

        void EnableDisableSetOfDrive(string _select, bool _boolSelect)
        {
            if (_select == "FirstRobot(R1)")
            {
                _drivePort.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
            }
            else if (_select == "SecondRobot(R2)")
            {
                _drivePort2.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
            }
            else if (_select == "ThirdRobot(R3)")
            {
                _drivePort3.EnableDrive(new drive.EnableDriveRequest(_boolSelect));
            }
        }

        void SetDriveSpeed(string _select, double _leftSpeed, double _rightSpeed)
        {
            if (_select == "FirstRobot(R1)")
            {
                _drivePort.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
            }
            else if (_select == "SecondRobot(R2)")
            {
                _drivePort2.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
            }
            else if (_select == "ThirdRobot(R3)")
            {
                _drivePort3.SetDriveSpeed(new drive.SetDriveSpeedRequest(_leftSpeed, _rightSpeed));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();
            _leftWheelSpeed = 0;
            _rightWheelSpeed = 0;

            SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
            EnableDisableSetOfDrive(_select, false);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();
            int num = Convert.ToInt32(numericGoAhead.Value);

            _leftWheelSpeed = num * 0.1;
            _rightWheelSpeed = num * 0.1;

            SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        }

        private void numericRight_ValueChanged(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();
            int num = Convert.ToInt32(numericRight.Value);

            _leftWheelSpeed = num * 0.1;
            //_rightWheelSpeed = num * 0.1;

            SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        }

        private void numericLeft_ValueChanged(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();
            int num = Convert.ToInt32(numericLeft.Value);

            //_leftWheelSpeed = num * 0.1;
            _rightWheelSpeed = num * 0.1;
            
            rightVelocity = Convert.ToByte(num * 200);
            //singleServoControl(rightVelocity);

            SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        }

        private void numericBack_ValueChanged(object sender, EventArgs e)
        {
            string _select = comboBox1.SelectedItem.ToString();
            int num = Convert.ToInt32(numericBack.Value);

            _leftWheelSpeed = num * -0.1;
            _rightWheelSpeed = num * -0.1;

            SetDriveSpeed(_select, _leftWheelSpeed, _rightWheelSpeed);
        }

        //private void btnSerialStart_Click(object sender, EventArgs e)
        //{
        //    port.PortName = comboBoxComPort.SelectedItem.ToString();
        //    port.BaudRate = Convert.ToInt32(txtBoxBaudRate.Text);
        //    port.Parity = Parity.None;
        //    port.DataBits = Convert.ToInt32(txtBoxDataBits.Text);
        //    port.StopBits = StopBits.One;

        //    try
        //    {
        //        port.Open();
        //        txtBoxDebugSerial.Text = port.PortName + " Port open is success";
        //    }
        //    catch
        //    {
        //        txtBoxDebugSerial.Text = port.PortName + " Port open is fail";
        //    }
        //}

        private void btnSerialClose_Click(object sender, EventArgs e)
        {
            rightVelocity = 0;
            singleServoControl(rightVelocity);

            port.Close();
        }

        private void serialSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerialSetting();
            SC.Show();
        }

        private void SerialSetting()
        {
            SC.ComPortEvent += new SerialCommunication.ComPortEventHandler(SC_ComPortEvent);
            SC.BaudRateEvent += new SerialCommunication.BaudRateEventHandler(SC_BaudRateEvent);
            port.Parity = Parity.None;
            SC.DataBitsEvent += new SerialCommunication.DataBitsEventHandler(SC_DataBitsEvent);
            port.StopBits = StopBits.One;

            //for Rx Communication
            //port.DataReceived += port_DataReceived;
        }

        private void SC_ComPortEvent(string _name)
        {
            port.PortName = _name;
        }

        private void SC_BaudRateEvent(string _name)
        {
            port.BaudRate = Convert.ToInt32(_name);
        }

        private void SC_DataBitsEvent(string _name)
        {
            port.DataBits = Convert.ToInt32(_name);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                port.Open();
                textBox1.Text = port.PortName + " Port open is success";
            }
            catch
            {
                textBox1.Text = port.PortName + " Port open is fail";
            }
        }

        //private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string line = port.ReadLine();
        //    this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        //}

        //private delegate void LineReceivedEvent(string line);

        //private void LineReceived(string line)
        //{
        //    //what to do with the received line here
        //    int tempValue = Convert.ToInt32(line);
        //    double rps = 0;
        //    double velocity = 0;

        //    rps = RevolutionsPerSencond(tempValue);
        //    velocity = velocityOfMotor(tempValue);

        //    //----select motor of right or left----//
        //    //select to right motor
        //    if (selectMotor == true)
        //    {
        //        //current stopwatch

        //        //add to List for Database
        //        rightPulse.Add(tempValue);
        //        PG.RightPulseData = tempValue;

        //        //debug
        //        label5.Text = Convert.ToString(tempValue);

        //        //rps
        //        rightRps.Add(rps);
        //        label12.Text = Convert.ToString(rps);

        //        //velocity
        //        rightVelocity.Add(velocity);
        //        label13.Text = Convert.ToString(velocity);

        //        //change Motor
        //        selectMotor = false;

        //        //for debug motors pulse value
        //        this.txtRxDataR.Text += Convert.ToString(tempValue) + "\r\n";
        //    }
        //    else
        //    {
        //        //select to left motor
        //        leftPulse.Add(tempValue);
        //        PG.LeftPulseData = tempValue;

        //        //debug
        //        label6.Text = Convert.ToString(tempValue);

        //        //rps
        //        leftRps.Add(rps);
        //        label16.Text = Convert.ToString(rps);

        //        //velocity
        //        leftVelocity.Add(velocity);
        //        label14.Text = Convert.ToString(velocity);

        //        //change Motor
        //        selectMotor = true;

        //        //for debug motors pulse value
        //        this.txtRxDataL.Text += Convert.ToString(tempValue) + "\r\n";
        //    }
        //}
    }
}
