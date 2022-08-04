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

/// <summary>
/// 
/// </summary>
namespace IngameScript
{

    partial class Program : MyGridProgram
    {
        const string CameraName = "Камера";
        const string RemoteControlName = "Дистанционное управление";
        const string TextPanel = "Прозрачная ЖК-панель 3";

        IMyCameraBlock cameraForRaycast;
        IMyRemoteControl remoteControl;
        IMyTextPanel textPanel;

        public Program()
        {
            cameraForRaycast = GridTerminalSystem.GetBlockWithName(CameraName) as IMyCameraBlock;
            cameraForRaycast.EnableRaycast = true;

            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.Direction = Base6Directions.Direction.Forward;

            textPanel = GridTerminalSystem.GetBlockWithName(TextPanel) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        }


        string VectorToGPS(Vector3D wayp, string name = "NAVOTHER", string color = "#FF75C9F1")
        {
            return $"GPS:{name}:{wayp.X}:{wayp.Y}:{wayp.Z}:{color}";
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (updateSource)
            {
                case UpdateType.None:
                    break;
                case UpdateType.Terminal:
                    break;
                case UpdateType.Trigger:
                    break;
                case UpdateType.Mod:
                    break;
                case UpdateType.Script:
                    break;
                case UpdateType.Update1:
                    break;
                case UpdateType.Update10:
                    break;
                case UpdateType.Update100:
                    break;
                case UpdateType.Once:
                    break;
                case UpdateType.IGC:
                    break;
                default:
                    break;
            }
            cameraForRaycast.EnableRaycast = true;
            textPanel.WriteText("",false);
            var rayCrds = LocalToWorld(cameraForRaycast.WorldMatrix.Down * 100000);
            var info = cameraForRaycast.Raycast(rayCrds);
            textPanel.WriteText(VectorToGPS(rayCrds) + "\n");
            if(info.HitPosition == null)
                textPanel.WriteText("HP is null!" + "\n", true);
            else
                textPanel.WriteText("HP: "+ VectorToGPS(info.HitPosition ?? new Vector3D()) + "\n", true);

            textPanel.WriteText(info.IsEmpty().ToString()+"\n", true);
            textPanel.WriteText(info.Name + "\n", true);
            textPanel.WriteText(info.EntityId.ToString() + "\n", true);
        }
        Vector3D WorldToLocal(Vector3D nearestPlayerCrds)
        {
            Vector3D mePosition = remoteControl.CubeGrid.WorldMatrix.Translation;//also CubeGrid.GetPosition();
            Vector3D worldDirection = nearestPlayerCrds - mePosition;
            return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(remoteControl.CubeGrid.WorldMatrix));
        }
        Vector3D LocalToWorldDirection(Vector3D local)
        {
            Vector3D world1Direction = Vector3D.TransformNormal(local, remoteControl.CubeGrid.WorldMatrix);
            Vector3D worldPosition = remoteControl.CubeGrid.WorldMatrix.Translation + world1Direction;
            return worldPosition;
        }
        Vector3D LocalToWorld(Vector3D local)
        {
            return Vector3D.Transform(local, cameraForRaycast.WorldMatrix);
        }
    }
}
