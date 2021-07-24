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
        //==========Settings========
        const string RemoteControlName = "RemoteControl";
        const bool UseDisplay = true;
        const string DisplayName = "Display";
        const int MaxSpeed_0_100 = 50;
        const int StopDistanceFromPlayer = 7;
        const bool WaitForFreeWay = true;
        const bool IsServer = false;
        //========EndSettings========
        IMyRemoteControl remoteControl;
        IMyTextPanel display;

        #region Vars which used in Main
        string DispStr;
        double distanceToPlayer;
        double height;
        Vector3D thisPos;
        Vector3D gravGrid;
        Vector3D GoalCoords;
        Vector3D nearestPlayerCrds;
        #endregion

        bool stage = false;
        public Program()
        {
            if (IsServer)
                Echo = (a) => { };//optimization for server

            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            remoteControl.WaitForFreeWay = WaitForFreeWay;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.SpeedLimit = MaxSpeed_0_100;
            remoteControl.SetCollisionAvoidance(true);
            remoteControl.Direction = Base6Directions.Direction.Forward;

            if (UseDisplay)
            {
                display = GridTerminalSystem.GetBlockWithName(DisplayName) as IMyTextPanel;
                if (display != null)
                {
                    display.ContentType = ContentType.TEXT_AND_IMAGE;
                    display.FontSize = display.SurfaceSize.Length() / 300f;
                }
                else
                    Echo("Display not founded");
            }

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            #region Ping
            stage = !stage;
            if (stage)
                Echo("*");
            else
                Echo("'");
            #endregion
            if (remoteControl.GetNearestPlayer(out nearestPlayerCrds))
            {
                thisPos = Me.CubeGrid.GetPosition();
                distanceToPlayer = (nearestPlayerCrds - thisPos).Length();
                gravGrid = remoteControl.GetNaturalGravity();
                GoalCoords = nearestPlayerCrds;
                if (gravGrid.Length() > 0)
                {
                    double elevationFromSurface;
                    if (remoteControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevationFromSurface))
                    {
                        height = distanceToPlayer / (elevationFromSurface == 0 ? 1 : elevationFromSurface);
                        GoalCoords = nearestPlayerCrds - gravGrid * (height > 1 ? height : 0.6f);//gravGrid * 0.2f;
                        remoteControl.SpeedLimit = (float)(distanceToPlayer > MaxSpeed_0_100 ? MaxSpeed_0_100 : distanceToPlayer < 20 ? distanceToPlayer * 0.7: distanceToPlayer);
                        DispStr = "(distanceToPlayer / elevationFromSurface): \n" + Math.Round((distanceToPlayer / elevationFromSurface),4);
                        DispStr += "\n distanceToPlayer\n" + Math.Round(distanceToPlayer,2);
                        Echo(DispStr);
                        if (UseDisplay)
                            display.WriteText(DispStr);

                    }
                }

                if (distanceToPlayer <= 10)
                    remoteControl.SetCollisionAvoidance(false);
                else
                    remoteControl.SetCollisionAvoidance(true);

                if (distanceToPlayer > StopDistanceFromPlayer)
                {
                    remoteControl.ClearWaypoints();
                    remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", GoalCoords));
                    remoteControl.SetAutoPilotEnabled(true);
                }
                Echo(String.Format("NearestPlayer in {0} meters", distanceToPlayer));
            }
            else
            {
                Echo("NearestPlayer not found!");
            }
        }
        //
    }
}
