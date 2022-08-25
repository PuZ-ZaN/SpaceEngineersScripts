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
        IMyRemoteControl remoteControl;
        IMyCameraBlock camera;
        bool signIsWorking;
        public Program()
        {
            remoteControl = GridTerminalSystem.GetBlockWithName("RemoteControl") as IMyRemoteControl;
            camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            remoteControl.WaitForFreeWay = true;
            remoteControl.SpeedLimit = 10;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.Direction = Base6Directions.Direction.Forward;
            remoteControl.SetCollisionAvoidance(false);
            signIsWorking = true;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (signIsWorking)
                Echo("*");
            else
                Echo(" ");
            Vector3D nearestPlayerCrds;
            if (remoteControl.GetNearestPlayer(out nearestPlayerCrds))
            {
                Vector3D thisPos = Me.CubeGrid.GetPosition();
                var distanceToPlayer = (nearestPlayerCrds - thisPos).Length();
                remoteControl.ClearWaypoints();
                if (distanceToPlayer > 5)
                {
                    remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", nearestPlayerCrds));
                    remoteControl.SetAutoPilotEnabled(true);
                    remoteControl.SpeedLimit = (float)distanceToPlayer;
                }
                if (camera.CanScan(remoteControl.GetNaturalGravity()))
                {
                    var EntityInfo = camera.Raycast(remoteControl.GetNaturalGravity());
                    var distanceToGrav = (thisPos + EntityInfo.Position).Length();
                    Echo("DistanceToGrav: " + distanceToGrav);
                    Echo("E: " + EntityInfo.Position.Length());
                    Echo("A "+camera.AvailableScanRange);
                }
                Echo("NearestPlayer dist: " + distanceToPlayer);
                Echo("NaturalGrav: " + remoteControl.GetNaturalGravity().Length().ToString());
                Echo("GridSize: " + Me.CubeGrid.GridSize);
            }
            else
            {
                Echo("NearestPlayer not found!");
            }
            signIsWorking = !signIsWorking;
        }
    }
}
