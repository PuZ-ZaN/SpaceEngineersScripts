using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SpaceEngineers.Game.ModAPI.Ingame;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;

using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        States SavedState;
        
        public Program()
        {
            SavedState = States.None;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (SavedState)
            {
                case States.None:
                    States stateOrNull = States.None;
                    if (Enum.TryParse<States>(argument, out stateOrNull)) {
                        SavedState = stateOrNull;
                        Runtime.UpdateFrequency = UpdateFrequency.Once;
                    }
                    break;
                case States.Waiting:

                    break;
                case States.Flying:

                    break;
                case States.FlyingSlow:

                    break;
                case States.Docking:

                    break;
                case States.UnDocking:

                    break;
                case States.Landing:

                    break;
                case States.FindingTheWay:

                    break;
                default:
                    break;
            }
        }
        bool Flying()
        {
            //if(Me.CubeGrid.WorldAABB.Distance(StatesArgs.pointToDocking)< )
            if (Me.CubeGrid.WorldAABBHr.Contains(StatesArgs.pointToDocking)!=ContainmentType.Disjoint)
            {
                return true;
            }
            //облет препятствий
            //управление движками
        }
    }
    enum States
    {
        None = 0,
        Waiting = 1,
        Flying = 2,
        FlyingSlow = 3,
        Docking = 4,
        UnDocking=5,
        Landing=6,
        FindingTheWay=7,

    }
    static class StatesArgs
    {
        public static Vector3D pointToDocking;
        public static double DockingSpeed;

        public static Vector3D connectorPoint;
        public static Vector3D pointToFly;

        public static int MaxMass;
    }


}
