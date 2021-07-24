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
        const int MaxSpeed_0_100 = 50;
        const bool WaitForFreeWay = true;
        const bool IsServer = false;
        //========EndSettings========
        IMyRemoteControl remoteControl;
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
            Vector3D nearestPlayerCrds;
            if (remoteControl.GetNearestPlayer(out nearestPlayerCrds))
            {
                Vector3D thisPos = Me.CubeGrid.GetPosition();
                var distanceToPlayer = (nearestPlayerCrds - thisPos).Length();
                var gravGrid = remoteControl.GetNaturalGravity();
                var GoalCoords = nearestPlayerCrds;
                if (gravGrid.Length() > 0)
                {//PlanetBehavios
                    Echo("PlanetBehavior");
                    remoteControl.ClearWaypoints();
                    GoalCoords = nearestPlayerCrds - gravGrid * 0.2f;
                    remoteControl.SpeedLimit = (float)(distanceToPlayer > 25 ? 25 : distanceToPlayer);

                    double elevation;
                    if (remoteControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation)){
                        Echo("elevation " + elevation);
                        //написать поведение в зависимости от высоты
                    }
                }
                if (distanceToPlayer < 15 && distanceToPlayer > 4)
                    remoteControl.SetCollisionAvoidance(false);
                else
                    remoteControl.SetCollisionAvoidance(true);
                if (distanceToPlayer > Me.CubeGrid.GridSize * 7 * (((int)Me.CubeGrid.GridSizeEnum) + 1))
                {
                    remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", GoalCoords));
                    remoteControl.SetAutoPilotEnabled(true);
                }
                Echo(String.Format("NearestPlayer in {0} meters", distanceToPlayer));
                Echo(GoalCoords.ToString());
            }
            else
            {
                Echo("NearestPlayer not found!");
            }
        }
    }
}
