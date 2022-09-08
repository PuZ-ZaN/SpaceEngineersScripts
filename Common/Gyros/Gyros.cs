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
    partial class Program
    {
        public class Gyros
        {
            private IMyShipController Cockpit;
            private List<IMyGyro> Gyrolist;
            private bool isGyroOver = false;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="cockpit">Need for get grid oriention</param>
            /// <param name="gyrolist">Grid's gyros</param>
            public Gyros(IMyShipController cockpit, List<IMyGyro> gyrolist)
            {
                Cockpit = cockpit;
                Gyrolist = gyrolist;
            }

            /// <summary>
            /// Using in main cycle of progblock, Update10 recommended
            /// </summary>
            /// <param name="localDirectionVector">Point which you keep in crosshair, local vector, not normalized</param>
            public void KeepDirection(Vector3D localDirectionVector)
            {
                if (!isGyroOver)
                {
                    MakeGyroOver(true);
                }
                Vector3D gravityVector = Vector3D.Normalize(Cockpit.GetNaturalGravity());
                localDirectionVector = Vector3D.Normalize(localDirectionVector);
                localDirectionVector.Z = gravityVector.Dot(Cockpit.WorldMatrix.Left);
                SetGyro(localDirectionVector);
            }

            public bool IsShipOriented(Vector3D localDirectionVector, int accuracy = 15)
            {
                //localDirectionVector = Vector3D.Normalize(localDirectionVector);
                //return (Math.Round(localDirectionVector.X, accuracy) == 0) && (Math.Round(localDirectionVector.Y, accuracy) == 0);
                return Math.Abs(localDirectionVector.X) < accuracy && Math.Abs(localDirectionVector.Y) < accuracy;
            }
            public bool IsShipOrientedNormalized(Vector3D localDirectionVector, int accuracy = 2)
            {
                localDirectionVector = Vector3D.Normalize(localDirectionVector);
                return (Math.Round(localDirectionVector.X, accuracy) == 0) && (Math.Round(localDirectionVector.Y, accuracy) == 0);
            }

            /// <summary>
            /// Release overrided gyros
            /// </summary>
            public void EndKeepDirection()
            {
                MakeGyroOver(false);
            }

            /// <summary>
            /// Set gyros signals
            /// </summary>
            /// <param name="axis"></param>
            private void SetGyro(Vector3D axis)
            {
                foreach (IMyGyro gyro in Gyrolist)
                {
                    gyro.Yaw = (float)axis.X;
                    gyro.Pitch = -(float)axis.Y;//i dont know why is working)
                    gyro.Roll = (float)axis.Z;
                }
            }

            /// <summary>
            /// Control override gyros
            /// </summary>
            /// <param name="over"></param>
            private void MakeGyroOver(bool over)
            {
                foreach (IMyGyro gyro in Gyrolist)
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
}
