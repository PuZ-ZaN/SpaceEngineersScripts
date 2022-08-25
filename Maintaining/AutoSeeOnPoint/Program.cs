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
        const string ShipControllerName = "Дистанционное управление";
        const string TextPanel = "Прозрачная ЖК-панель 3";


        private IMyShipController cockpit;
        private List<IMyGyro> gyrolist;
        private bool isGyroOver = false;
        Vector3D pointToSeeGlobal;
        VectorsCasting vc = new VectorsCasting();

        IMyTextPanel textPanel;

        Vector3D forward = new Vector3D(0, 0, -1);//TODO: проверить Z координату, нужно ли -1 или 1

        Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName(ShipControllerName) as IMyShipController;
            gyrolist = new List<IMyGyro>();
            GridTerminalSystem.GetBlocksOfType(gyrolist, gyro => gyro.IsSameConstructAs(cockpit));

            textPanel = GridTerminalSystem.GetBlockWithName(TextPanel) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        private void SeeOnPoint()
        {
            //Vector3D gravityVector = Vector3D.Normalize(cockpit.GetNaturalGravity());
            //Vector3D horizontalDirection = gravityVector.Cross(cockpit.WorldMatrix.Down);
            //if (gravityVector.Dot(cockpit.WorldMatrix.Down) < 0)
            //{
            //    horizontalDirection = Vector3D.Normalize(horizontalDirection);
            //}
            var dirPoint = Vector3D.Normalize(vc.WorldToLocal(pointToSeeGlobal, cockpit));
            textPanel.WriteText(dirPoint.ToString());
            var cross = dirPoint.Cross(new Vector3D(0,0,-1));

            if (dirPoint.Dot(cockpit.CubeGrid.WorldMatrix.Forward) < 0)
            {
                cross = Vector3D.Normalize(cross);
            }
            SetGyro(cross);
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
                SeeOnPoint();
            }
            else
            {
                if (!isGyroOver)
                {
                    MakeGyroOver(true);
                    pointToSeeGlobal = vc.GPSToVector(arg);
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
