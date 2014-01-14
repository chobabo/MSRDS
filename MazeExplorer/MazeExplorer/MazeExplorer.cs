using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using W3C.Soap;
using System.Drawing.Imaging;
using System.Drawing;

using System.Reflection;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;
using Microsoft.Robotics.Simulation;
using Microsoft.Robotics.Simulation.Engine;
using engineproxy = Microsoft.Robotics.Simulation.Engine.Proxy;
using xna = Microsoft.Xna.Framework;
using xnagrfx = Microsoft.Xna.Framework.Graphics;
using ds = Microsoft.Dss.Services.Directory;
using cons = Microsoft.Dss.Services.Constructor;

using simdrive = Microsoft.Robotics.Services.Simulation.Drive.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using lrf = Microsoft.Robotics.Services.Simulation.Sensors.LaserRangeFinder.Proxy;
//using simbumper = Microsoft.Robotics.Services.Simulation.Sensors.Bumper.Proxy;
using simwebcam = Microsoft.Robotics.Services.Simulation.Sensors.SimulatedWebcam.Proxy;
//using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using submgr = Microsoft.Dss.Services.SubscriptionManager;
using lrfcommon = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

using Microsoft.Ccr.Adapters.WinForms;
using simulation = MazeExplorer;

//using OpenCvSharp;

namespace MazeExplorer
{
    [Contract(Contract.Identifier)]
    [DisplayName("MazeExplorer")]
    [Description("MazeExplorer service (no description provided)")]
    class MazeExplorerService : DsspServiceBase
    {
        #region Create Start MRDS Engine Service
        [ServiceState]
        MazeExplorerState _state = new MazeExplorerState();

        [Partner("Engine",
            Contract = engineproxy.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]

        private engineproxy.SimulationEnginePort _engineServicePort = new engineproxy.SimulationEnginePort();

        [ServicePort("/MazeExplorer", AllowMultipleInstances = true)]
        MazeExplorerOperations _mainPort = new MazeExplorerOperations();

        //[SubscriptionManagerPartner]
        //submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        public MazeExplorerService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        protected override void Start()
        {
            base.Start();

            WinFormsServicePort.Post(new RunForm(CreateForm));
            WinFormsServicePort.Post(new RunForm(CreateSensorForm));
            WinFormsServicePort.Post(new RunForm(CreateSensorForm2));
            WinFormsServicePort.Post(new RunForm(CreateSensorForm3));
            WinFormsServicePort.Post(new RunForm(CreateControllerForm));

            SetupCamera();
            PopulateWorld();

            //For Drive Service
            SpawnIterator(CreateSensorService);

            //using multiple sensor
            Activate(Arbiter.ReceiveWithIterator(false, _dateTimePort, UpdateSensorData));
            TaskQueue.EnqueueTimer(TimeSpan.FromMilliseconds(60), _dateTimePort);
        }

        drive.DriveOperations _drivePort = null;
        drive.DriveOperations _drivePort2 = null;
        drive.DriveOperations _drivePort3 = null;

        //Create winform
        MazeForm _mazeForm = null;
        SensorForm _sensorForm = null;
        SensorForm _sensorForm2 = null;
        SensorForm _sensorForm3 = null;
        ControllerForm _controllerForm = null;

        System.Windows.Forms.Form CreateForm()
        {
            _mazeForm = new MazeForm(_mainPort);
            return _mazeForm;
        }

        System.Windows.Forms.Form CreateSensorForm()
        {
            _sensorForm = new SensorForm(_mainPort, "FirstRobot Sensor Display");
            return _sensorForm;
        }

        System.Windows.Forms.Form CreateSensorForm2()
        {
            _sensorForm2 = new SensorForm(_mainPort, "SecondRobot Sensor Display");
            return _sensorForm2;
        }

        System.Windows.Forms.Form CreateSensorForm3()
        {
            _sensorForm3 = new SensorForm(_mainPort, "ThirdRobot Sensor Display");
            return _sensorForm3;
        }

        System.Windows.Forms.Form CreateControllerForm()
        {
            _controllerForm = new ControllerForm(_mainPort);
            return _controllerForm;
        }
        #endregion

        #region Setup Camera and PopulateWorld(Sky, Ground, Robot)
        private void SetupCamera()
        {
            CameraView view = new CameraView();
            view.EyePosition = new Vector3(0.0f, 13.0f, 0.0f);
            view.LookAtPoint = new Vector3(0.0f, 0.0f, 0.0f);
            SimulationEngine.GlobalInstancePort.Update(view);
        }

        private void PopulateWorld()
        {
            AddSky();
            AddGround();

            AddRobot();
        }

        void AddSky()
        {
            SkyEntity sky = new SkyEntity("sky.dds", "sky_diff.dds");
            SimulationEngine.GlobalInstancePort.Insert(sky);

            LightSourceEntity sun = new LightSourceEntity();
            sun.State.Name = "Sun";
            sun.Type = LightSourceEntityType.Directional;
            sun.Color = new Vector4(0.8f, 0.8f, 0.8f, 1);
            sun.Direction = new Vector3(-1.0f, -1.0f, 0.5f);
            SimulationEngine.GlobalInstancePort.Insert(sun);
        }

        void AddGround()
        {
            HeightFieldShapeProperties hf = new HeightFieldShapeProperties("height field",
                64, // number of rows 
                100, // distance in meters, between rows
                64, // number of columns
                100, // distance in meters, between columns
                1, // scale factor to multiple height values 
                -1000); // vertical extent of the height field. Should be set to large negative values

            hf.HeightSamples = new HeightFieldSample[hf.RowCount * hf.ColumnCount];
            for (int i = 0; i < hf.RowCount * hf.ColumnCount; i++)
            {
                hf.HeightSamples[i] = new HeightFieldSample();
                hf.HeightSamples[i].Height = (short)(Math.Sin(i * 0.01));
            }

            hf.Material = new MaterialProperties("ground", 0.1f, 0.1f, 0.1f);

            SimulationEngine.GlobalInstancePort.Insert(new HeightFieldEntity(hf, "03RamieSc.dds"));
        }

        #endregion

        #region Create Robot Entity
        void AddRobot()
        {
            CameraState();
            AddPioneer3DXRobot(new Vector3(0f, 0f, 4.4f), firstRobot);
            AddPioneer3DXRobot2(new Vector3(1f, 0f, 4.4f), secondRobot);
            AddPioneer3DXRobot3(new Vector3(-1f, 0f, 4.4f), thirdRobot);
        }

        string firstRobot = "P3DXMotorBase";
        string secondRobot = "P3DXMotorBase2";
        string thirdRobot = "P3DXMotorBase3";

        Pioneer3DX robotBaseEntity;
        Pioneer3DX robotBaseEntity2;
        Pioneer3DX robotBaseEntity3;

        void AddPioneer3DXRobot(Vector3 position, string robotName)
        {
            robotBaseEntity = CreateMotorBase(ref position, ref robotName);

            laser1 = CreateLaserRangeFinder("P3DXLaserRangeFinder");

            robotBaseEntity.InsertEntity(laser1);
            robotBaseEntity.InsertEntity(r1Cam1);
            robotBaseEntity.InsertEntity(r1Cam2);
            robotBaseEntity.InsertEntity(r1Cam3);
            robotBaseEntity.InsertEntity(r1Cam4);

            SimulationEngine.GlobalInstancePort.Insert(robotBaseEntity);
        }

        void AddPioneer3DXRobot2(Vector3 position, string robotName)
        {
            robotBaseEntity2 = CreateMotorBase(ref position, ref robotName);

            laser2 = CreateLaserRangeFinder("P3DXLaserRangeFinder2");

            robotBaseEntity2.InsertEntity(laser2);
            robotBaseEntity2.InsertEntity(r2Cam1);
            robotBaseEntity2.InsertEntity(r2Cam2);
            robotBaseEntity2.InsertEntity(r2Cam3);
            robotBaseEntity2.InsertEntity(r2Cam4);

            SimulationEngine.GlobalInstancePort.Insert(robotBaseEntity2);
        }

        void AddPioneer3DXRobot3(Vector3 position, string robotName)
        {
            robotBaseEntity3 = CreateMotorBase(ref position, ref robotName);

            laser3 = CreateLaserRangeFinder("P3DXLaserRangeFinder3");

            robotBaseEntity3.InsertEntity(laser3);
            robotBaseEntity3.InsertEntity(r3Cam1);
            robotBaseEntity3.InsertEntity(r3Cam2);
            robotBaseEntity3.InsertEntity(r3Cam3);
            robotBaseEntity3.InsertEntity(r3Cam4);

            SimulationEngine.GlobalInstancePort.Insert(robotBaseEntity3);
        }

        private Pioneer3DX CreateMotorBase(ref Vector3 position, ref string robotName)
        {
            Pioneer3DX robotBaseEntity = new Pioneer3DX(position);

            robotBaseEntity.State.Assets.Mesh = "Pioneer3dx.bos";
            robotBaseEntity.ChassisShape.State.DiffuseColor = new Vector4(0.8f, 0.25f, 0.25f, 1.0f);

            robotBaseEntity.State.Name = robotName;

            return robotBaseEntity;
        }

        #endregion

        #region Create Service Handler
        //[ServiceHandler]
        //public void SubscribeHandler(Subscribe subscribe)
        //{
        //    SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        //}

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        private Vector3 _startPosition = new Vector3();
        private Vector3 _startPosition2 = new Vector3();
        private Vector3 _startPosition3 = new Vector3();

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> InsertBoxHandler(InsertBox update)
        {
            for (int i = 0; i < _state.SingleShapeEntityList.Count; i++)
                SimulationEngine.GlobalInstancePort.Delete(_state.SingleShapeEntityList[i]);

            _state.SingleShapeEntityList.Clear();


            float _width = 0f;
            float _length = 0f;
            float _height = 0f;
            float _mass = 0f;

            //Vector3 _dim = new Vector3();
            Vector3 _pos = new Vector3();
            Vector3 _pos2 = new Vector3();
            Vector3 _pos3 = new Vector3();


            for (int i = 0; i < update.Body.Count; i++)
            {
                if (update.Body[i].BoxType == "Target")
                {
                    ////Insert target postion box                    
                    //_width = 0.6f;
                    //_length = 0.6f;
                    //_height = 0.01f;
                    //_mass = 0f;

                    //_dim = new Vector3(_width, _height, _length);
                    //_pos = new Vector3((float)update.Body[i].StartPosX, 0f, (float)update.Body[i].StartPosZ);

                    //BoxShapeProperties tshape = null;
                    //tshape = new BoxShapeProperties(_mass, new Pose(), _dim);
                    //tshape.Material = new MaterialProperties("UserBox", 0.1f, 0.1f, 0.1f);
                    //tshape.DiffuseColor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);

                    //SingleShapeEntity tentity = new SingleShapeEntity(new BoxShape(tshape), _pos);

                    //tentity.State.Name = "TargetPosition:" + System.Guid.NewGuid().ToString();
                    //SimulationEngine.GlobalInstancePort.Insert(tentity);

                    //_state.SingleShapeEntityList.Add(tentity);
                }
                else if (update.Body[i].BoxType == "Start")
                {
                    ////Insert start postion box
                    //_width = 0.6f;
                    //_length = 0.6f;
                    //_height = 0.01f;
                    //_mass = 0f;

                    //_dim = new Vector3(_width, _height, _length);

                    //----------Set robot with same position-----------------------------//
                    _pos = new Vector3((float)update.Body[i].StartPosX, 0f, (float)update.Body[i].StartPosZ);
                    _startPosition = _pos;
                    robotBaseEntity.Position = TypeConversion.ToXNA(_pos);

                    //BoxShapeProperties sshape = null;
                    //sshape = new BoxShapeProperties(_mass, new Pose(), _dim);
                    //sshape.Material = new MaterialProperties("UserBox", 0.1f, 0.1f, 0.1f);
                    //sshape.DiffuseColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

                    //SingleShapeEntity sentity = new SingleShapeEntity(new BoxShape(sshape), _pos);

                    //sentity.State.Name = "StartPosition:" + System.Guid.NewGuid().ToString();
                    //SimulationEngine.GlobalInstancePort.Insert(sentity);

                    //_state.SingleShapeEntityList.Add(sentity);
                }
                else if (update.Body[i].BoxType == "Start2")
                {
                    ////Insert start postion box
                    //_width = 0.6f;
                    //_length = 0.6f;
                    //_height = 0.01f;
                    //_mass = 0f;

                    //_dim = new Vector3(_width, _height, _length);

                    //----------Set robot with same position-----------------------------//
                    _pos2 = new Vector3((float)update.Body[i].StartPosX, 0f, (float)update.Body[i].StartPosZ);
                    _startPosition2 = _pos2;
                    robotBaseEntity2.Position = TypeConversion.ToXNA(_pos2);

                    //BoxShapeProperties sshape = null;
                    //sshape = new BoxShapeProperties(_mass, new Pose(), _dim);
                    //sshape.Material = new MaterialProperties("UserBox2", 0.1f, 0.1f, 0.1f);
                    //sshape.DiffuseColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

                    //SingleShapeEntity sentity2 = new SingleShapeEntity(new BoxShape(sshape), _pos2);

                    //sentity2.State.Name = "StartPosition2:" + System.Guid.NewGuid().ToString();
                    //SimulationEngine.GlobalInstancePort.Insert(sentity2);

                    //_state.SingleShapeEntityList.Add(sentity2);
                }
                else if (update.Body[i].BoxType == "Start3")
                {
                    ////Insert start postion box
                    //_width = 0.6f;
                    //_length = 0.6f;
                    //_height = 0.01f;
                    //_mass = 0f;

                    //_dim = new Vector3(_width, _height, _length);

                    //----------Set robot with same position-----------------------------//
                    _pos3 = new Vector3((float)update.Body[i].StartPosX, 0f, (float)update.Body[i].StartPosZ);
                    _startPosition3 = _pos3;
                    robotBaseEntity3.Position = TypeConversion.ToXNA(_pos3);

                    //BoxShapeProperties sshape = null;
                    //sshape = new BoxShapeProperties(_mass, new Pose(), _dim);
                    //sshape.Material = new MaterialProperties("UserBox3", 0.1f, 0.1f, 0.1f);
                    //sshape.DiffuseColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

                    //SingleShapeEntity sentity3 = new SingleShapeEntity(new BoxShape(sshape), _pos3);

                    //sentity3.State.Name = "StartPosition2:" + System.Guid.NewGuid().ToString();
                    //SimulationEngine.GlobalInstancePort.Insert(sentity3);

                    //_state.SingleShapeEntityList.Add(sentity3);
                }
                else if (update.Body[i].StartPosX != 0 && update.Body[i].StartPosZ != 0 && update.Body[i].EndPosX != 0 && update.Body[i].EndPosZ != 0)
                {
                    _width = Math.Abs((float)update.Body[i].EndPosX - (float)update.Body[i].StartPosX);
                    _length = Math.Abs((float)update.Body[i].EndPosZ - (float)update.Body[i].StartPosZ);
                    _height = 2.0f;
                    _mass = 0f;

                    if (!string.IsNullOrEmpty(update.Body[i].Mass.ToString()) && update.Body[i].Mass != 0)
                        _mass = (float)update.Body[i].Mass;

                    if (!string.IsNullOrEmpty(update.Body[i].Height.ToString()) && update.Body[i].Height != 0)
                        _height = (float)update.Body[i].Height;

                    Vector3 dim = new Vector3(_width, _height, _length);
                    Vector3 pos = new Vector3(((float)update.Body[i].EndPosX + (float)update.Body[i].StartPosX) * 0.5f, _height * 0.5f, ((float)update.Body[i].EndPosZ + (float)update.Body[i].StartPosZ) * 0.5f);

                    BoxShapeProperties shape = null;

                    shape = new BoxShapeProperties(_mass, new Pose(), dim);
                    shape.Material = new MaterialProperties("UserBox", 0.1f, 0.1f, 0.1f);

                    SingleShapeEntity entity = new SingleShapeEntity(new BoxShape(shape), pos);

                    entity.State.Name = "Box:" + System.Guid.NewGuid().ToString();
                    SimulationEngine.GlobalInstancePort.Insert(entity);

                    _state.SingleShapeEntityList.Add(entity);
                }
            }

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> MoveRobotPositionHandler(MoveRobotPosition update)
        {
            if (update.Body.IsInitialPositon)
            {
                robotBaseEntity.Position = TypeConversion.ToXNA(_startPosition);
                robotBaseEntity.Rotation = new xna.Vector3(0f, 0f, 0f);
            }
            else
            {
                robotBaseEntity.Rotation = new xna.Vector3(0f, robotBaseEntity.Rotation.Y, 0f);
            }

            if (update.Body.IsInitialPositon2)
            {
                robotBaseEntity2.Position = TypeConversion.ToXNA(_startPosition2);
                robotBaseEntity2.Rotation = new xna.Vector3(0f, 0f, 0f);
            }
            else
            {
                robotBaseEntity2.Rotation = new xna.Vector3(0f, robotBaseEntity2.Rotation.Y, 0f);
            }

            if (update.Body.IsInitialPositon3)
            {
                robotBaseEntity3.Position = TypeConversion.ToXNA(_startPosition3);
                robotBaseEntity3.Rotation = new xna.Vector3(0f, 0f, 0f);
            }
            else
            {
                robotBaseEntity3.Rotation = new xna.Vector3(0f, robotBaseEntity3.Rotation.Y, 0f);
            }

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> UpdateMazeStateHandler(UpdateMazeState update)
        {
            _state.IsEnableOfDiffentiaDrive = update.Body.IsEnableOfDiffentiaDrive;

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        #endregion

        #region Helper Methods
        class CompletionPort : Port<bool> { }
        IEnumerator<ITask> PostOnTaskCompletionHelper(CompletionPort completionPort, IteratorHandler handler)
        {
            yield return new IterativeTask(handler);
            completionPort.Post(true);
        }
        void PostOnTaskCompletion(CompletionPort completionPort, IteratorHandler handler)
        {
            SpawnIterator<CompletionPort, IteratorHandler>(completionPort, handler, PostOnTaskCompletionHelper);
        }

        bool HasError<T>(PortSet<T, Fault> sensorOrFault)
        {
            Fault fault = (Fault)sensorOrFault;
            if (fault != null)
            {
                LogError(fault.ToException());
                return true;
            }
            else
                return false;
        }
        #endregion

        #region UpdateSensorData
        Port<DateTime> _dateTimePort = new Port<DateTime>();

        IEnumerator<ITask> UpdateSensorData(DateTime dateTime)
        {
            WebCamProcess wcp = new WebCamProcess(r1Cam1, r1Cam2, r1Cam3, r1Cam4, _sensorForm, _mazeForm);
            WebCamProcess wcp2 = new WebCamProcess(r2Cam1, r2Cam2, r2Cam3, r2Cam4, _sensorForm2, _mazeForm);
            WebCamProcess wcp3 = new WebCamProcess(r3Cam1, r3Cam2, r3Cam3, r3Cam4, _sensorForm3, _mazeForm);

            LRFSensorProcess lsp = new LRFSensorProcess(_LRFPort, _sensorForm, _mazeForm);
            LRFSensorProcess lsp2 = new LRFSensorProcess(_LRFPort2, _sensorForm2, _mazeForm);
            LRFSensorProcess lsp3 = new LRFSensorProcess(_LRFPort3, _sensorForm3, _mazeForm);

            var resultPort = new CompletionPort();
            PostOnTaskCompletion(resultPort, wcp.UpdateSimulatedWebCam);
            PostOnTaskCompletion(resultPort, wcp2.UpdateSimulatedWebCam);
            PostOnTaskCompletion(resultPort, wcp3.UpdateSimulatedWebCam);
            PostOnTaskCompletion(resultPort, UpdateRobotPosition);
            //PostOnTaskCompletion(resultPort, UpdateCompass);
            PostOnTaskCompletion(resultPort, lsp.UpdateLRF);
            PostOnTaskCompletion(resultPort, lsp2.UpdateLRF);
            PostOnTaskCompletion(resultPort, lsp3.UpdateLRF);
            //PostOnTaskCompletion(resultPort, UpdateSonar);
            //PostOnTaskCompletion(resultPort, UpdateInfrared);
            //PostOnTaskCompletion(resultPort, UpdateWebCamImage);

            Activate(Arbiter.MultipleItemReceive(false, resultPort, 7, allComplete =>
            {
                Activate(Arbiter.ReceiveWithIterator(false, _dateTimePort, UpdateSensorData));
                TaskQueue.EnqueueTimer(TimeSpan.FromMilliseconds(600), _dateTimePort); //60
            }));

            yield break;
        }
        #endregion

        #region Calculate of Sensor
        //realtime update robot position

        //private TrackerState _tState = new TrackerState();
        private Tracker _traker = new Tracker();

        IEnumerator<ITask> UpdateRobotPosition()
        {
            WinFormsServicePort.Post(new FormInvoke(
            delegate()
            {
                if (_mazeForm.trackerStart == true)
                {
                    _traker.UpdateTracker(_mazeForm._tState, robotBaseEntity);
                }

                //Point _point = new Point((int)((robotBaseEntity.Position.X + 5.0f) * 10 / 0.2f), (int)((robotBaseEntity.Position.Z + 5.0f) * 10 / 0.2f));
                PointF _point = new PointF(robotBaseEntity.Position.X, robotBaseEntity.Position.Z);
                PointF _point2 = new PointF(robotBaseEntity2.Position.X, robotBaseEntity2.Position.Z);
                PointF _point3 = new PointF(robotBaseEntity3.Position.X, robotBaseEntity3.Position.Z);

                _mazeForm.RobotPosition = _point;
                _mazeForm.RobotPosition2 = _point2;
                _mazeForm.RobotPosition3 = _point3;
            }));

            yield break;
        }
        #endregion

        #region Create Sensor Service
        CameraEntity r1Cam1 = new CameraEntity(150, 150);
        CameraEntity r1Cam2 = new CameraEntity(150, 150);
        CameraEntity r1Cam3 = new CameraEntity(150, 150);
        CameraEntity r1Cam4 = new CameraEntity(150, 150);

        CameraEntity r2Cam1 = new CameraEntity(150, 150);
        CameraEntity r2Cam2 = new CameraEntity(150, 150);
        CameraEntity r2Cam3 = new CameraEntity(150, 150);
        CameraEntity r2Cam4 = new CameraEntity(150, 150);

        CameraEntity r3Cam1 = new CameraEntity(150, 150);
        CameraEntity r3Cam2 = new CameraEntity(150, 150);
        CameraEntity r3Cam3 = new CameraEntity(150, 150);
        CameraEntity r3Cam4 = new CameraEntity(150, 150);

        float _cameraHeight = 0.7f;

        private void CameraState()
        {
            r1Cam1.State.Name = "FirstRobotCam1";
            r1Cam1.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r1Cam1.FieldOfView = 90;
            r1Cam1.IsRealTimeCamera = true;

            r1Cam2.State.Name = "FirstRobotCam2";
            r1Cam2.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r1Cam2.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.1f, _cameraHeight, 0.0f));
            r1Cam2.FieldOfView = 90;
            r1Cam2.IsRealTimeCamera = true;

            r1Cam3.State.Name = "FirstRobotCam3";
            r1Cam3.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r1Cam3.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.0f, _cameraHeight, 0.1f));
            r1Cam3.FieldOfView = 90;
            r1Cam3.IsRealTimeCamera = true;

            r1Cam4.State.Name = "FirstRobotCam4";
            r1Cam4.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r1Cam4.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(-0.1f, _cameraHeight, 0.0f));
            r1Cam4.FieldOfView = 90;
            r1Cam4.IsRealTimeCamera = true;

            r2Cam1.State.Name = "SecondRobotCam1";
            r2Cam1.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r2Cam1.FieldOfView = 90;
            r2Cam1.IsRealTimeCamera = true;

            r2Cam2.State.Name = "SecondRobotCam2";
            r2Cam2.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r2Cam2.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.1f, _cameraHeight, 0.0f));
            r2Cam2.FieldOfView = 90;
            r2Cam2.IsRealTimeCamera = true;

            r2Cam3.State.Name = "SecondRobotCam3";
            r2Cam3.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r2Cam3.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.0f, _cameraHeight, 0.1f));
            r2Cam3.FieldOfView = 90;
            r2Cam3.IsRealTimeCamera = true;

            r2Cam4.State.Name = "SecondRobotCam4";
            r2Cam4.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r2Cam4.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(-0.1f, _cameraHeight, 0.0f));
            r2Cam4.FieldOfView = 90;
            r2Cam4.IsRealTimeCamera = true;

            r3Cam1.State.Name = "ThirdRobotCam1";
            r3Cam1.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r3Cam1.FieldOfView = 90;
            r3Cam1.IsRealTimeCamera = true;

            r3Cam2.State.Name = "ThirdRobotCam2";
            r3Cam2.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r3Cam2.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.1f, _cameraHeight, 0.0f));
            r3Cam2.FieldOfView = 90;
            r3Cam2.IsRealTimeCamera = true;

            r3Cam3.State.Name = "ThirdRobotCam3";
            r3Cam3.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r3Cam3.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(0.0f, _cameraHeight, 0.1f));
            r3Cam3.FieldOfView = 90;
            r3Cam3.IsRealTimeCamera = true;

            r3Cam4.State.Name = "ThirdRobotCam4";
            r3Cam4.State.Pose.Position = new Vector3(0.0f, _cameraHeight, 0.0f);
            r3Cam4.SetViewParameters(new xna.Vector3(0.0f, _cameraHeight, 0.0f), new xna.Vector3(-0.1f, _cameraHeight, 0.0f));
            r3Cam4.FieldOfView = 90;
            r3Cam4.IsRealTimeCamera = true;
        }

        IEnumerator<ITask> CreateSensorService()
        {
            bool success = true;

            yield return Arbiter.Choice(
                simdrive.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                    "http://localhost/P3DXMotorBase")),
                delegate(CreateResponse createResponse)
                {
                    _drivePort = ServiceForwarder<drive.DriveOperations>(createResponse.Service);
                    _drivePort.EnableDrive(new drive.EnableDriveRequest(false));
                    _state.IsEnableOfDiffentiaDrive = false;
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            if (!success)
            {
                this.Shutdown();
                yield break;
            }

            yield return Arbiter.Choice(
                simdrive.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                    "http://localhost/P3DXMotorBase2")),
                delegate(CreateResponse createResponse)
                {
                    _drivePort2 = ServiceForwarder<drive.DriveOperations>(createResponse.Service);
                    _drivePort2.EnableDrive(new drive.EnableDriveRequest(false));
                    _state.IsEnableOfDiffentiaDrive = false;
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            if (!success)
            {
                this.Shutdown();
                yield break;
            }

            yield return Arbiter.Choice(
                simdrive.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                    "http://localhost/P3DXMotorBase3")),
                delegate(CreateResponse createResponse)
                {
                    _drivePort3 = ServiceForwarder<drive.DriveOperations>(createResponse.Service);
                    _drivePort3.EnableDrive(new drive.EnableDriveRequest(false));
                    _state.IsEnableOfDiffentiaDrive = false;
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            if (!success)
            {
                this.Shutdown();
                yield break;
            }

            yield return Arbiter.Choice(
                lrf.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                "http://localhost/P3DXLaserRangeFinder")),
                delegate(CreateResponse createResponse)
                {
                    _LRFPort = ServiceForwarder<lrfcommon.SickLRFOperations>(createResponse.Service);
                    LogInfo(LogGroups.Console, "((LRF Binding Success -> lrf1))");
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            yield return Arbiter.Choice(
                lrf.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                "http://localhost/P3DXLaserRangeFinder2")),
                delegate(CreateResponse createResponse)
                {
                    _LRFPort2 = ServiceForwarder<lrfcommon.SickLRFOperations>(createResponse.Service);
                    LogInfo(LogGroups.Console, "((LRF Binding Success -> lrf2))");
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            yield return Arbiter.Choice(
                lrf.Contract.CreateService(ConstructorPort,
                Microsoft.Robotics.Simulation.Partners.CreateEntityPartner(
                "http://localhost/P3DXLaserRangeFinder3")),
                delegate(CreateResponse createResponse)
                {
                    _LRFPort3 = ServiceForwarder<lrfcommon.SickLRFOperations>(createResponse.Service);
                    LogInfo(LogGroups.Console, "((LRF Binding Success -> lrf3))");
                },
                delegate(Fault fault)
                {
                    success = false;
                    LogError(fault);
                }
            );

            if (!success)
            {
                this.Shutdown();
                yield break;
            }

            if ((_drivePort != null) && (_drivePort2 != null) && (_drivePort3 != null))
            {
                WinFormsServicePort.FormInvoke(
                delegate() { _controllerForm.SetDrivePort(_drivePort, _drivePort2, _drivePort3); }
                );
            }

            yield break;
        }

        ///---http://cafe.naver.com/msrskorea/1939---///

        //static string r1Laser = "P3DXLaserRangeFinder";

        lrfcommon.SickLRFOperations _LRFPort = null;
        lrfcommon.SickLRFOperations _LRFPort2 = null;
        lrfcommon.SickLRFOperations _LRFPort3 = null;

        LaserRangeFinderEntity laser1;
        LaserRangeFinderEntity laser2;
        LaserRangeFinderEntity laser3;

        private LaserRangeFinderEntity CreateLaserRangeFinder(string _stateName)
        {
            LaserRangeFinderEntity laser = new LaserRangeFinderEntity(
                new Pose(new Vector3(0, 0.15f, 0)));

            laser.State.Name = _stateName;
            //laser.LaserBox.State.DiffuseColor = new Vector4(0.25f, 0.25f, 0.8f, 1.0f);

            //레이저 파인더가 red dots을 뿌려주지 않기 위해 설정하는거다
            laser.Flags |= VisualEntityProperties.DisableRendering;
            
            return laser;
        }
        #endregion

    }//end class

    #region SensorProcess(camera, LRF)
    public class WebCamProcess
    {
        Bitmap _curFrame1 = null;
        Bitmap _curFrame2 = null;
        Bitmap _curFrame3 = null;
        Bitmap _curFrame4 = null;

        PortSet<Bitmap, Exception> result = new PortSet<Bitmap, Exception>();
        PortSet<Bitmap, Exception> result2 = new PortSet<Bitmap, Exception>();
        PortSet<Bitmap, Exception> result3 = new PortSet<Bitmap, Exception>();
        PortSet<Bitmap, Exception> result4 = new PortSet<Bitmap, Exception>();

        MazeForm _mazeForm;
        SensorForm _sensorForm;

        CameraEntity _Cam1;
        CameraEntity _Cam2;
        CameraEntity _Cam3;
        CameraEntity _Cam4;

        public WebCamProcess() : base() { }

        public WebCamProcess(CameraEntity Cam1, CameraEntity Cam2, CameraEntity Cam3, CameraEntity Cam4,
            SensorForm sensorForm, MazeForm mazeForm)
            : base()
        {
            _Cam1 = Cam1;
            _Cam2 = Cam2;
            _Cam3 = Cam3;
            _Cam4 = Cam4;

            _mazeForm = mazeForm;
            _sensorForm = sensorForm;

            //UpdateSimulatedWebCam();
        }

        public IEnumerator<ITask> UpdateSimulatedWebCam()
        {
            _Cam1.CaptureScene(System.Drawing.Imaging.ImageFormat.MemoryBmp, result);
            _Cam2.CaptureScene(System.Drawing.Imaging.ImageFormat.MemoryBmp, result2);
            _Cam3.CaptureScene(System.Drawing.Imaging.ImageFormat.MemoryBmp, result3);
            _Cam4.CaptureScene(System.Drawing.Imaging.ImageFormat.MemoryBmp, result4);

            yield return Arbiter.Choice(result,
                 delegate(Bitmap bmp)
                 {
                     _curFrame1 = bmp;
                 },
                 delegate(Exception fault)
                 {
                     //LogError(fault);
                 });

            yield return Arbiter.Choice(result2,
                 delegate(Bitmap bmp)
                 {
                     _curFrame2 = bmp;
                 },
                 delegate(Exception fault)
                 {
                     //LogError(fault);
                 });

            yield return Arbiter.Choice(result3,
                 delegate(Bitmap bmp)
                 {
                     _curFrame3 = bmp;
                 },
                 delegate(Exception fault)
                 {
                     //LogError(fault);
                 });

            yield return Arbiter.Choice(result4,
                 delegate(Bitmap bmp)
                 {
                     _curFrame4 = bmp;
                 },
                 delegate(Exception fault)
                 {
                     //LogError(fault);
                 });

            if (_mazeForm.camStart == true)
            {
                _sensorForm.imageProcessing(_curFrame1, _curFrame2, _curFrame3, _curFrame4, _mazeForm.filterName);
            }

            yield break;
        }
    }//end class

    public class LRFSensorProcess
    {
        lrfcommon.SickLRFOperations _LRFPort;
        SensorForm _sensorForm;
        MazeForm _mazeForm;

        public LRFSensorProcess() : base() { }

        public LRFSensorProcess(lrfcommon.SickLRFOperations lrfPort, SensorForm sensorForm, MazeForm mazeForm)
            : base()
        {
            _LRFPort = lrfPort;
            _sensorForm = sensorForm;
            _mazeForm = mazeForm;
        }

        public IEnumerator<ITask> UpdateLRF()
        {
            if (_mazeForm.lrfStart == true)
            {
                var sensorOrFault = _LRFPort.Get();

                yield return sensorOrFault.Choice();

                if (_LRFPort != null)
                {
                    Microsoft.Robotics.Services.Sensors.SickLRF.Proxy.State state =
                        (Microsoft.Robotics.Services.Sensors.SickLRF.Proxy.State)sensorOrFault;

                    _sensorForm.createTop(state);
                }
            }

            yield break;
        }
    }//end class
    #endregion
}


