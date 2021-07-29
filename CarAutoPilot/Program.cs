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
        const string DisplayName = "Display";
        bool UseDisplay = true;
        const bool WaitForFreeWay = true;
        const bool IsServer = false;
        const int MaxHorizontalAndDownSpeed_0_100 = 50;
        const int DecelerationDistance = 30;
        const int DecelerationValue = 3;
        const int StopDistance = 7;
        const int StopDistanceVertical = 2;
        //========EndSettings========
        IMyRemoteControl remoteControl;
        IMyTextPanel display;

        #region Vars which used in Main
        StringBuilder DisplayStr;
        double distanceToPlayer;
        double goalHeight;
        double GridElevationFromSurface;
        double MeElevation;
        double PlayerElevation;
        Vector3D gridPosition;
        Vector3D gravGrid;
        Vector3D goalCoords;
        Vector3D nearestPlayerCrds;
        Vector3D PlayerElevationFromCenter;
        //при неудачной попытке определить высоту останется true, это позволит кораблю остановиться рядом с игроком (см. условие остановки)
        bool IsDiffElevationsWithinStop = true;
        #endregion
        #region PulseVars
        byte pulse = 0;
        List<string> pulseSign;
        #endregion
        public Program()
        {
            #region IsServer
            if (IsServer)
#pragma warning disable CS0162 // Обнаружен недостижимый код
                Echo = (a) => { };//optimization for server
#pragma warning restore CS0162 // Обнаружен недостижимый код
            else
                pulseSign = new List<string>() { @"\", @"|", @"/", @"-" };
            #endregion
            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            remoteControl.WaitForFreeWay = WaitForFreeWay;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.SpeedLimit = MaxHorizontalAndDownSpeed_0_100;
            remoteControl.SetCollisionAvoidance(true);
            remoteControl.Direction = Base6Directions.Direction.Forward;
            DisplayStr = new StringBuilder();
            if (UseDisplay)
            {
                display = GridTerminalSystem.GetBlockWithName(DisplayName) as IMyTextPanel;
                if (display != null)
                {
                    display.ContentType = ContentType.TEXT_AND_IMAGE;
                    display.FontSize = display.SurfaceSize.Length() / 700f;
                }
                else
                    UseDisplay = false;
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            #region Pulse
            if (!IsServer)
                Echo(pulseSign[pulse++]);
            if (pulse >= pulseSign.Count)
                pulse = 0;
            #endregion
            //Если есть игроки рядом
            if (remoteControl.GetNearestPlayer(out nearestPlayerCrds))
            {
                gridPosition = Me.CubeGrid.GetPosition();
                distanceToPlayer = Vector3D.Distance(nearestPlayerCrds, gridPosition);
                gravGrid = remoteControl.GetNaturalGravity();
                goalCoords = nearestPlayerCrds;
                
                //Если грид на планете делает траекторию не прямой
                if (gravGrid.Length() > 0 && remoteControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out GridElevationFromSurface))
                {
                    goalHeight = distanceToPlayer / (GridElevationFromSurface == 0 ? 1 : GridElevationFromSurface);
                    goalCoords = nearestPlayerCrds - gravGrid * goalHeight;//gravGrid * 0.2f;
                    if (remoteControl.TryGetPlanetPosition(out PlayerElevationFromCenter))
                    {
                        MeElevation = Vector3D.Distance(gridPosition, PlayerElevationFromCenter);
                        PlayerElevation = Vector3D.Distance(nearestPlayerCrds, PlayerElevationFromCenter);
                        IsDiffElevationsWithinStop = Math.Abs(PlayerElevation - MeElevation) < Math.Abs(StopDistanceVertical / 2);
                    }
                }

                //Замедление если близко к игроку
                var IsGridNearPlayer = distanceToPlayer <= DecelerationDistance;
                if (IsGridNearPlayer)
                {
                    remoteControl.SpeedLimit = DecelerationValue;
                    remoteControl.SetDockingMode(false);
                    remoteControl.SetCollisionAvoidance(false);
                }
                else
                {
                    remoteControl.SpeedLimit = MaxHorizontalAndDownSpeed_0_100;
                    remoteControl.SetDockingMode(true);
                    remoteControl.SetCollisionAvoidance(true);
                }

                //Решает нужно ли лететь к игроку
                if (distanceToPlayer > StopDistance && !IsDiffElevationsWithinStop)
                {
                    remoteControl.ClearWaypoints();
                    remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", goalCoords));
                    remoteControl.SetAutoPilotEnabled(true);
                }
                #region DisplayAndEchoOutput
                DisplayStr.Append(String.Format("NearestPlayer in {0} meters", distanceToPlayer));
                DisplayStr.Append("\n");
                DisplayStr.Append(distanceToPlayer > StopDistance);
                DisplayStr.Append("\n");
                DisplayStr.Append(IsDiffElevationsWithinStop);
                DisplayStr.Append(String.Format("\nHeight {0}", goalHeight));
                var normalize = Vector3D.Normalize(nearestPlayerCrds);
                var dot = Vector3D.Dot(remoteControl.WorldMatrix.GetDirectionVector(Base6Directions.Direction.Up), WorldToLocal(normalize));
                DisplayStr.Append(String.Format("\nDotIs {0}", dot));
                Echo(DisplayStr.ToString());
                if (UseDisplay)
                    display.WriteText(DisplayStr);
                DisplayStr.Clear();
                #endregion
            }
            else
                display.WriteText("NearestPlayer not found!");
        }
        Vector3D WorldToLocal(Vector3D nearestPlayerCrds)
        {
            Vector3D mePosition = remoteControl.CubeGrid.WorldMatrix.Translation;//also CubeGrid.GetPosition();
            Vector3D worldDirection = nearestPlayerCrds - mePosition;
            return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(remoteControl.CubeGrid.WorldMatrix));
        }
        Vector3D LocalToWorld(Vector3D local)
        {
            Vector3D world1Direction = Vector3D.TransformNormal(local, remoteControl.CubeGrid.WorldMatrix);
            Vector3D worldPosition = remoteControl.CubeGrid.WorldMatrix.Translation + world1Direction;
            return worldPosition;
        }
        //
    }
}
