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

/*
 Эксперименты с преобразованием локальных координат в глобальные и наоборот
 */
namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        const string RemoteControlName = "RemoteControl";
        const string DisplayName = "Display";
        IMyRemoteControl remoteControl;
        IMyTextPanel display;
        StringBuilder DisplayStr;
        public Program()
        {
            //find remote control
            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            //setting up remote control
            remoteControl.WaitForFreeWay = true;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.SpeedLimit = 40;
            remoteControl.SetCollisionAvoidance(true);
            remoteControl.Direction = Base6Directions.Direction.Forward;

            display = GridTerminalSystem.GetBlockWithName(DisplayName) as IMyTextPanel;
            DisplayStr = new StringBuilder();

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            DisplayStr.Clear();
            var Up = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Up);
            var Down = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Down);
            var Forward = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Forward);
            var Backward = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Backward);
            var Left = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Left);
            var Right = LocalToWorld(remoteControl.CubeGrid.WorldMatrix.Right);

            var ListVectors = new List<KeyValuePair<string, Vector3D>>() {
                new KeyValuePair<string, Vector3D>("Up",Up),
                new KeyValuePair<string, Vector3D>("Down",Down),
                new KeyValuePair<string, Vector3D>("Forward",Forward),
                new KeyValuePair<string, Vector3D>("Backward",Backward),
                new KeyValuePair<string, Vector3D>("Left",Left),
                new KeyValuePair<string, Vector3D>("Right",Right),
            };

            DisplayStr.Append(String.Format("\nGPS:Up:{0}:{1}:{2}:#FF75C9F1:", Up.X, Up.Y, Up.Z));
            DisplayStr.Append(String.Format("\nGPS:Down:{0}:{1}:{2}:#FF75C9F1:", Down.X, Down.Y, Down.Z));
            DisplayStr.Append(String.Format("\nGPS:Forward:{0}:{1}:{2}:#FF75C9F1:", Forward.X, Forward.Y, Forward.Z));
            DisplayStr.Append(String.Format("\nGPS:Backward:{0}:{1}:{2}:#FF75C9F1:", Backward.X, Backward.Y, Backward.Z));
            DisplayStr.Append(String.Format("\nGPS:Left:{0}:{1}:{2}:#FF75C9F1:", Left.X, Left.Y, Left.Z));
            DisplayStr.Append(String.Format("\nGPS:Right:{0}:{1}:{2}:#FF75C9F1:", Right.X, Right.Y, Right.Z));

            display.WriteText(DisplayStr);
            Echo(DisplayStr.ToString());
        }
/*        Vector3D WorldToLocal(Vector3D nearestPlayerCrds)
        {
            Vector3D mePosition = remoteControl.CubeGrid.WorldMatrix.Translation;
            Vector3D worldDirection = nearestPlayerCrds - mePosition;
            return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(remoteControl.CubeGrid.WorldMatrix));
        }*/
        Vector3D LocalToWorld(Vector3D local)
        {
            Vector3D world1Direction = Vector3D.TransformNormal(local, remoteControl.WorldMatrix);
            return remoteControl.CubeGrid.WorldMatrix.Translation - world1Direction;
            //return Vector3D.TransformNormal(local, remoteControl.WorldMatrix);
        }
    }
}
