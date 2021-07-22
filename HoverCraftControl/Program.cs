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
        private IMyShipController cockpit;
        private List<IMyGyro> gyrolist;
        private float MaxBankAngle = 0.5f;
        private float MaxVelocity = 200f;
        private bool isGyroOver = false;


        Program()
        {
            //Echo = a => { };

            List<IMyShipController> cockpits = new List<IMyShipController>();
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(cockpits);

            if (cockpits[0] == null)
                throw new Exception("No Cockpits");
            else
                cockpit = cockpits[0];

            gyrolist = new List<IMyGyro>();
            GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyrolist, (a) => (a.IsSameConstructAs(cockpit)));
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
                    GyroOver(true);
                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                }
                else
                {
                    GyroOver(false);
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                }
            }
        }

        public void KeepHorizon()
        {
            Vector3D grav = Vector3D.Normalize(cockpit.GetNaturalGravity());

            Vector3D HV = Vector3D.Reject(cockpit.GetShipVelocities().LinearVelocity, grav);
            HV *= MaxBankAngle / MaxVelocity;
            if (HV.Length() > MaxBankAngle)
                HV = Vector3D.Normalize(HV) * MaxBankAngle;
            grav += HV;

            Vector3D axis = (grav + HV).Cross(cockpit.WorldMatrix.Down);
            if (grav.Dot(cockpit.WorldMatrix.Down) < 0)
            {
                axis = Vector3D.Normalize(axis);
            }

            Vector3D signal = cockpit.WorldMatrix.Up * cockpit.MoveIndicator.X +
                              cockpit.WorldMatrix.Left * cockpit.MoveIndicator.Z +
                              cockpit.WorldMatrix.Backward * cockpit.RollIndicator;

            axis += MaxBankAngle * signal;
            SetGyro(axis);
        }

        public void SetGyro(Vector3D axis)
        {
            foreach (IMyGyro gyro in gyrolist)
            {
                gyro.Yaw = (float)axis.Dot(gyro.WorldMatrix.Up);
                gyro.Pitch = (float)axis.Dot(gyro.WorldMatrix.Right);
                gyro.Roll = (float)axis.Dot(gyro.WorldMatrix.Backward);
            }
        }

        public void GyroOver(bool over)
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
    }
}