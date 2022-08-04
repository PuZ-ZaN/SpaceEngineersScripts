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
/*
 Автопилот для корабля, следует за игроком
 */
namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        //==========Settings========
        string RemoteControlName = "RC";
        string DisplayName = "Display";
        string CockpitName = "Cockpit";
        StackFSM brain;
        IMyRemoteControl rc;
        IMyTextSurface display;
        IMyCockpit cockpit;

        List<Vector3D> waypoints;
        int posCount = 0;
        bool playInited = false;
        IMyTextSurface MeSurface0;

        Vector3D currentWaypoint;
        public Program()
        {

            MeSurface0 = Me.GetSurface(0);
            rc = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            rc.WaitForFreeWay = false;
            //rc.FlightMode = FlightMode.OneWay;
            rc.SetCollisionAvoidance(true);
            rc.Direction = Base6Directions.Direction.Forward;
            rc.ClearWaypoints();
            currentWaypoint = rc.GetPosition();

            waypoints = new List<Vector3D>();

            cockpit = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyCockpit;
            display = cockpit.GetSurface(0);
            brain = new StackFSM();

        }

        public void Main(string argument, UpdateType updateSource)
        {
            try
            {
                if (argument == "Rec")
                {
                    brain.PushState(Record);
                }
                if (argument == "Play")
                {
                    display.WriteText("Play");
                    brain.PushState(Play);
                }
                if (argument == "GetInfo")
                {
                    var data = waypoints.Count.ToString();
                    data += playInited.ToString() + "\n";
                    if (waypoints.Count > 1)
                    {
                        //data += "zero "
                        //+ Math.Abs((waypoints[0] - rc.GetPosition()).Length()).ToString() + "\n last"
                        //+ Math.Abs((waypoints[waypoints.Count - 1] - rc.GetPosition()).Length()) + " ANA "+ 
                        //(Math.Abs((waypoints[waypoints.Count - 1] - rc.GetPosition()).Length()) < 100).ToString();
                        data += "Remove wp[0]" + (Math.Abs((waypoints[0] - rc.GetPosition()).Length()) < 70).ToString() + "\n";
                        data += "stop " + (Math.Abs((waypoints[0] - rc.GetPosition()).Length()) < 100 && waypoints.Count == 1).ToString();
                    }
                    data += "\n mtdNm" + brain.getCurrentState()?.Method.Name;


                    Echo(data);
                    display.WriteText(data);
                }
                brain.Update();
            }
            catch (Exception e)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                display.WriteText(e.ToString());
            }

        }

        void Record()
        {
            if (Runtime.UpdateFrequency != UpdateFrequency.Update100)
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            var currPos = rc.GetPosition();
            if (waypoints.Count < 1)
                waypoints.Add(currPos);

            if (Math.Abs((waypoints[waypoints.Count - 1] - currPos).Length()) > 100)
                waypoints.Add(currPos);

            if (!cockpit.IsUnderControl)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                brain.PopState();
            }
        }
        void Play()
        {

            if (Runtime.UpdateFrequency != UpdateFrequency.Update100)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
                RcPlayGetReady();
            }

            if (Math.Abs((waypoints[0] - rc.GetPosition()).Length()) < 100 && waypoints.Count == 1)
            {
                RcPlayStop();
                Runtime.UpdateFrequency = UpdateFrequency.None;
                brain.PopState();
            }

            if (!playInited)
            {
                RcPlayGetReady();
                playInited = true;
            }
            //RcPlayGetReady();

            //if (Math.Abs(( - rc.GetPosition()).Length()) < 70)
            //{
            //    RcPlayStop();
            //}
        }
        void RcPlayGetReady()
        {
            
            int i = 0;
            foreach (var waypoint in waypoints)
            {
                rc.ClearWaypoints();
                rc.AddWaypoint(waypoint, i++.ToString());
            }
            //rc.FlightMode = FlightMode.OneWay;
            rc.SetAutoPilotEnabled(true);
        }

        void RcPlayStop()
        {
            rc.SetAutoPilotEnabled(false);
            rc.ClearWaypoints();
        }


        Vector3D WorldToLocal(Vector3D nearestPlayerCrds)
        {
            Vector3D mePosition = rc.CubeGrid.WorldMatrix.Translation;//also CubeGrid.GetPosition();
            Vector3D worldDirection = nearestPlayerCrds - mePosition;
            return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(rc.CubeGrid.WorldMatrix));
        }
        Vector3D LocalToWorld(Vector3D local)
        {
            Vector3D world1Direction = Vector3D.TransformNormal(local, rc.CubeGrid.WorldMatrix);
            Vector3D worldPosition = rc.CubeGrid.WorldMatrix.Translation + world1Direction;
            return worldPosition;
        }
    }
}
