using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using simulation = MazeExplorer;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;
using Microsoft.Robotics.Simulation;
using Microsoft.Robotics.Simulation.Engine;

namespace MazeExplorer
{
    public sealed class Contract
    {
        [DataMember]
        public const string Identifier = "http://schemas.tempuri.org/2010/12/mazeexplorer.html";
    }

    [DataContract]
    public class MazeExplorerState
    {
        [DataMember]
        public List<SingleShapeEntity> SingleShapeEntityList = new List<SingleShapeEntity>();

        private bool _isEnableOfDiffentiaDrive = false;

        [DataMember]
        public bool IsEnableOfDiffentiaDrive
        {
            get { return _isEnableOfDiffentiaDrive; }
            set { _isEnableOfDiffentiaDrive = value; }
        }
    }

    [ServicePort]
    public class MazeExplorerOperations : PortSet<DsspDefaultLookup, DsspDefaultDrop, Get, InsertBox, MoveRobotPosition, UpdateMazeState>
    {
    }

    public class Get : Get<GetRequestType, PortSet<MazeExplorerState, Fault>>
    {
        public Get()
        {
        }

        public Get(GetRequestType body)
            : base(body)
        {
        }

        public Get(GetRequestType body, PortSet<MazeExplorerState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    public class UpdateMazeState : Update<MazeExplorerState, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public UpdateMazeState()
        {
        }

        public UpdateMazeState(MazeExplorerState body)
            : base(body)
        {
        }
    }

    [DataContract()]
    public class BoxState
    {
        [DataMember]
        public string BoxType = "Wall";  //Wall, Start, Target

        [DataMember]
        public double StartPosX = 0f;

        [DataMember]
        public double StartPosZ = 0f;

        [DataMember]
        public double EndPosX = 0f;

        [DataMember]
        public double EndPosZ = 0f;

        [DataMember]
        public double Height = 0f;

        [DataMember]
        public double Mass = 0f;

        [DataMember]
        public string Texture = "";

        [DataMember]
        public Vector4 DiffuseColor = new Vector4(-1f, 0f, 0f, 0f);
    }

    public class InsertBox : Update<List<BoxState>, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public InsertBox()
        {
        }

        public InsertBox(List<BoxState> body)
            : base(body)
        {
        }
    }


    [DataContract()]
    public class RobotPosition
    {
        [DataMember]
        public bool IsInitialPositon = false;

        [DataMember]
        public bool IsInitialPositon2 = false;

        [DataMember]
        public bool IsInitialPositon3 = false;
    }

    public class MoveRobotPosition : Update<RobotPosition, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public MoveRobotPosition()
        {
        }

        public MoveRobotPosition(RobotPosition body)
            : base(body)
        {
        }
    }

    /// <summary>
    /// MazeExplorer subscribe operation
    /// </summary>
    //public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    //{
    //    /// <summary>
    //    /// Creates a new instance of Subscribe
    //    /// </summary>
    //    public Subscribe()
    //    {
    //    }

    //    /// <summary>
    //    /// Creates a new instance of Subscribe
    //    /// </summary>
    //    /// <param name="body">the request message body</param>
    //    public Subscribe(SubscribeRequestType body)
    //        : base(body)
    //    {
    //    }

    //    /// <summary>
    //    /// Creates a new instance of Subscribe
    //    /// </summary>
    //    /// <param name="body">the request message body</param>
    //    /// <param name="responsePort">the response port for the request</param>
    //    public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
    //        : base(body, responsePort)
    //    {
    //    }
    //}
}


