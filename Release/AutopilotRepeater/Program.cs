using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SpaceEngineers.Game.ModAPI.Ingame;

using System;
using System.Collections;
using System.Collections.Generic;
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
        const string RemoteControlName = "RC";
        const string CockpitName = "Cockpit";
        const int PointsBtwDist = 100;

        IMyRemoteControl rc;
        IMyCockpit cockpit;
        Vector3D currentWaypoint;
        Vector3D prevPos;

        int WaypNumber = 1;
        State state = State.None;
        public Program()
        {

            rc = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            rc.WaitForFreeWay = true;
            rc.FlightMode = FlightMode.OneWay;
            rc.SetCollisionAvoidance(true);
            rc.Direction = Base6Directions.Direction.Forward;
            rc.ClearWaypoints();
            currentWaypoint = rc.GetPosition();
            cockpit = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyCockpit;

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Terminal || updateSource == UpdateType.Trigger)
            {
                State parsedState;
                if (Enum.TryParse<State>(argument, out parsedState))
                {
                    state = parsedState;
                }
                else
                {
                    state = State.None;
                }
            }
            switch (state)
            {
                case State.None:
                    StopAllDoes();
                    break;
                case State.Record:
                    Record();
                    break;
                case State.Play:
                    Play();
                    break;
                case State.ClrMem:
                    rc.ClearWaypoints();
                    break;
                default:
                    break;
            }
        }

        void Record()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            var currPos = rc.GetPosition();
            if (prevPos == null)
                prevPos = currPos;
            if ((prevPos - currPos).Length() > PointsBtwDist)
            {
                rc.AddWaypoint(new MyWaypointInfo(WaypNumber++.ToString(), currPos));
                prevPos = currPos;
            }
        }

        void Play()
        {
            //Runtime.UpdateFrequency = UpdateFrequency.Update100; //если нужны проверки во время полета
            if (!rc.IsAutoPilotEnabled)
                rc.SetAutoPilotEnabled(true);
        }

        void StopAllDoes()
        {
            Runtime.UpdateFrequency = UpdateFrequency.None;
        }
        enum State
        {
            None = 0,
            Record = 1,
            Play = 2,
            ClrMem = 3,
        }
    }
}
