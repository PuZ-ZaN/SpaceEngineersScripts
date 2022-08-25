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
 Автопилот для корабля, следует за игроком,
 Владельцем блока автопилота должен быть NPC, иначе функция remoteControl.GetNearestPlayer
 не работает
 */
namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        //==========Settings========
        string RemoteControlName = "RemoteControl (доступ запрещен)";
        string DisplayName = "Display";
        bool UseDisplay = false;
        bool WaitForFreeWay = true;
        bool IsServer = false;
        int MaxHorizontalAndDownSpeed_0_100 = 50;
        int DecelerationDistance = 30;
        int DecelerationValue = 3;
        int StopDistance = 7;
        int StopDistanceVertical = 2;
        //========EndSettings========
        IMyRemoteControl remoteControl;
        IMyTextPanel display;

        StackFSM brain;

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
        bool IsGridNearPlayer;
        IMyTextSurface MeSurface0;
        #endregion
        #region PulseVars
        byte pulse = 0;
        List<string> pulseSign;
        #endregion
        public Program()
        {
            #region SettingArgs
            var settings = Me.CustomData;
            string[] settingsLines;
            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            if (settings == "")
            {
                Me.CustomData = "RemoteControlName = RemoteControl\n" +
                    "DisplayName = Display\n" +
                    "UseDisplay = false\n" +
                    "WaitForFreeWay = true\n" +
                    "IsServer = false\n" +
                    "MaxHorizontalAndDownSpeed_0_100 = 50\n" +
                    "DecelerationDistance = 30\n" +
                    "DecelerationValue = 3\n" +
                    "StopDistance = 7\n" +
                    "StopDistanceVertical = 2";
            }
            settingsLines = settings.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in settingsLines)
            {
                var l = line.Split('=');
                settingsDict.Add(l[0].Trim(), l[1].Trim());
            }

            string RemoteControlName = settingsDict["RemoteControlName"];
            string DisplayName = settingsDict["DisplayName"];
            bool UseDisplay = bool.Parse(settingsDict["UseDisplay"]);
            bool WaitForFreeWay = bool.Parse(settingsDict["WaitForFreeWay"]);
            bool IsServer = bool.Parse(settingsDict["IsServer"]);
            int MaxHorizontalAndDownSpeed_0_100 = int.Parse(settingsDict["MaxHorizontalAndDownSpeed_0_100"]);
            int DecelerationDistance = int.Parse(settingsDict["DecelerationDistance"]);
            int DecelerationValue = int.Parse(settingsDict["DecelerationValue"]);
            int StopDistance = int.Parse(settingsDict["StopDistance"]);
            int StopDistanceVertical = int.Parse(settingsDict["StopDistanceVertical"]);

            #endregion

            #region IsServer
            if (IsServer)
                Echo = (a) => { };//optimization for server
            else
                pulseSign = new List<string>() { @"\", @"|", @"/", @"-" };
            #endregion

            MeSurface0 = Me.GetSurface(0);
            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            if (remoteControl == null)
            {
                Echo("RC is NULL!");
                MeSurface0.WriteText("RC is NULL!");
                throw new ArgumentException("RC is NULL");
            }
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
            brain = new StackFSM();
            brain.PushState(FindPeoplesNear);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            #region Pulse
            if (!IsServer)
            {
                Echo(pulseSign[pulse++]);
                MeSurface0.WriteText(pulseSign[pulse++]);
            }
            if (pulse >= pulseSign.Count)
                pulse = 0;
            #endregion
            brain.Update();
            var mName = brain.getCurrentState().Method.Name;
            Echo(mName);
            MeSurface0.WriteText(mName);
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

        //StatesAsFunction
        void FindPeoplesNear()
        {
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
                brain.PopState();
                brain.PushState(FlyToPlayer);
            }
        }

        void FlyToPlayer()
        {
            IsGridNearPlayer = distanceToPlayer <= DecelerationDistance;
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

            if (distanceToPlayer > StopDistance && !IsDiffElevationsWithinStop)
            {
                remoteControl.ClearWaypoints();
                remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", goalCoords));
                remoteControl.SetAutoPilotEnabled(true);
            }
            brain.PopState();
            brain.PushState(FindPeoplesNear);
        }
    }
}
