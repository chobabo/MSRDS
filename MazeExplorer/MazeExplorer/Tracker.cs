using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Net;
using System.IO;
//using System.Net.Mime;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using Microsoft.Robotics.Simulation.Engine;
using Microsoft.Robotics.PhysicalModel;

namespace MazeExplorer
{
    /// <summary>
    /// SimplePosition State
    /// </summary>
    public class SimplePositionState
    {
        /// <summary>
        /// Current X position estimate in meters
        /// </summary>
        public double X;

        /// <summary>
        /// Current Y position estimate in meters
        /// </summary>
        public double Y;

        /// <summary>
        /// Current angle estimate in radians
        /// </summary>
        public double Theta;

        /// <summary>
        /// Time stamp of this estimate
        /// </summary>
        public DateTime TimeStamp;
    }

    /// <summary>
    /// The Tracker State
    /// </summary>
    public class TrackerState
    {
        public SimplePositionState CurrentPosition;

        public List<SimplePositionState> PositionHistory;
    }

    class Tracker
    {
        //constants for drawing
        private const float maxX = 4;  //meters 13
        private const float minX = -4;  //meters -2
        private const float maxY = 4;   //meters 1
        private const float minY = -4; //meters -11

        private const float pixelspermeter = 50;    //50

        private const int robothalfwidth = 10; //pixels

        private const float deltamovement = 0.1f; //meters

        public void UpdateTracker(TrackerState _state, Pioneer3DX robotBaseEntity)
        {
            SimplePositionState lastposition = new SimplePositionState();
            SimplePositionState newState = new SimplePositionState();

            newState.TimeStamp = DateTime.Now;
            newState.X = robotBaseEntity.State.Pose.Position.X;
            newState.Y = -robotBaseEntity.State.Pose.Position.Z;
            newState.Theta = ConvertQuaternion(robotBaseEntity.State.Pose.Orientation);

            if (_state.PositionHistory.Count > 0)
                lastposition = _state.PositionHistory[_state.PositionHistory.Count - 1];

            _state.CurrentPosition = newState;

            if (ComparePositions(lastposition, _state.CurrentPosition) > deltamovement)
                _state.PositionHistory.Add(_state.CurrentPosition);
        }

        private double ComparePositions(SimplePositionState one, SimplePositionState two)
        {
            if (one == null || two == null)
                return Double.MaxValue;

            return Math.Sqrt((one.X - two.X) * (one.X - two.X) + (one.Y - two.Y) * (one.Y - two.Y));
        }

        /// <summary>
        /// Convert a Quaternion to an angle around the y axis from 0 to 2 Pi
        /// </summary>
        private double ConvertQuaternion(Quaternion q)
        {
            AxisAngle a = new AxisAngle();
            a = Quaternion.ToAxisAngle(q);

            if (float.IsNaN(a.Angle))
                return 0;
            else if (Math.Sign(a.Axis.Y) < 0)
                return 2.0 * Math.PI - a.Angle;

            return a.Angle;
        }

        private Stream GenerateImage(TrackerState _state)
        {
            MemoryStream memory = null;
            int imgWidth = (int)Math.Round(pixelspermeter * (maxX - minX));
            int imgHeight = (int)Math.Round(pixelspermeter * (maxY - minY));

            int Npoints = _state.PositionHistory.Count;

            if (Npoints == 0)
                return null;

            using (Bitmap bmp = new Bitmap(imgWidth, imgHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightGray);

                    Pen pen = new Pen(Color.DarkBlue);

                    Point[] points = new Point[Npoints];

                    //draw lines
                    if (Npoints > 1)
                    {
                        for (int i = 0; i < Npoints; i++)
                        {
                            int xpos = (int)Math.Round(_state.PositionHistory[i].X * pixelspermeter - minX * pixelspermeter);
                            int ypos = imgHeight - (int)Math.Round(_state.PositionHistory[i].Y * pixelspermeter - minY * pixelspermeter);
                            points[i] = new Point(xpos, ypos);
                        }
                        g.DrawLines(pen, points);
                    }

                    //draw dots
                    foreach (Point p in points)
                    {
                        g.DrawEllipse(pen, p.X - 1, p.Y - 1, 2, 2);
                    }

                    //draw robot
                    int robotX = (int)Math.Round(_state.CurrentPosition.X * pixelspermeter - minX * pixelspermeter);
                    int robotY = imgHeight - (int)Math.Round(_state.CurrentPosition.Y * pixelspermeter - minY * pixelspermeter);
                    Point robotupperleft = new Point(robotX - robothalfwidth, robotY - robothalfwidth);
                    Point robotcenter = new Point(robotX, robotY);
                    Point robotfront = new Point((int)Math.Round(robotX - robothalfwidth * Math.Sin(_state.CurrentPosition.Theta)),
                                                 (int)Math.Round(robotY - robothalfwidth * Math.Cos(_state.CurrentPosition.Theta)));
                    g.DrawEllipse(new Pen(Color.Red), new Rectangle(robotupperleft, new Size(2 * robothalfwidth, 2 * robothalfwidth)));
                    g.DrawLine(new Pen(Color.Red), robotcenter, robotfront);


                    //write info
                    Font f = new Font(FontFamily.GenericMonospace, 12);
                    g.DrawString(Npoints + " points", f, Brushes.DarkBlue, 5, 5);
                }

                memory = new MemoryStream();
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;

            }
            return memory;
        }

        public void SendJpeg(TrackerState _state, string _name)
        {
            System.Drawing.Image image = System.Drawing.Bitmap.FromStream(
                GenerateImage(_state));

            //image.Save("FirstRobotLogger.bmp");
            image.Save(_name);
        }
    }
}
