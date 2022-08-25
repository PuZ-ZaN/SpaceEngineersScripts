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
        const string CockpitName = "Cockpit";
        private IMyShipController cockpit;
        private List<IMyGyro> gyrolist;
        private bool isGyroOver = false;
        Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyShipController;
            gyrolist = new List<IMyGyro>();
            GridTerminalSystem.GetBlocksOfType(gyrolist, gyro => gyro.IsSameConstructAs(cockpit));
        }

        private void KeepHorizon()
        {
            Vector3D gravityVector = Vector3D.Normalize(cockpit.GetNaturalGravity());
            Vector3D horizontalDirection = gravityVector.Cross(cockpit.WorldMatrix.Down);
            if (gravityVector.Dot(cockpit.WorldMatrix.Down) < 0)
            {
                horizontalDirection = Vector3D.Normalize(horizontalDirection);
            }
            SetGyro(horizontalDirection);
        }

        private void SetGyro(Vector3D axis)
        {
            foreach (IMyGyro gyro in gyrolist)
            {
                gyro.Yaw = (float)axis.Dot(gyro.WorldMatrix.Up);
                gyro.Pitch = (float)axis.Dot(gyro.WorldMatrix.Right);
                gyro.Roll = (float)axis.Dot(gyro.WorldMatrix.Backward);
            }
        }

        private void MakeGyroOver(bool over)
        {
            foreach (IMyGyro gyro in gyrolist)
            {
                gyro.Yaw = 0;
                gyro.Pitch = 0;
                gyro.Roll = 0;
                gyro.GyroOverride = over;
            }
            isGyroOver = over;
        }

        void Main(string arg, UpdateType uType)
        {
            if (uType == UpdateType.Update10)
            {
                KeepHorizon();
            }
            else
            {
                if (!isGyroOver)
                {
                    MakeGyroOver(true);
                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                }
                else
                {
                    MakeGyroOver(false);
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                }
            }
        }
    }
}
