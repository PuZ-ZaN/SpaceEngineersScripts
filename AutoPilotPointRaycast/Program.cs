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

using static IngameScript.Program.VectorsCasting;

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

        VectorsCasting vc = new VectorsCasting();
        Vector3D DestinationWaypoint;


        private List<IMyGyro> gyrolist;
        private bool isGyroOver = false;

        private double CubeGridRadius;//TODO: лучше ли float?
        public Program()
        {
            cameraForRaycast = GridTerminalSystem.GetBlockWithName(CameraName) as IMyCameraBlock;
            cameraForRaycast.EnableRaycast = true;

            remoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.Direction = Base6Directions.Direction.Forward;
            remoteControl.SetCollisionAvoidance(true);

            textPanel = GridTerminalSystem.GetBlockWithName(TextPanel) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            textPanel.WriteText("");

            gyrolist = new List<IMyGyro>();
            GridTerminalSystem.GetBlocksOfType(gyrolist, gyro => gyro.IsSameConstructAs(remoteControl));

            CubeGridRadius = cameraForRaycast.CubeGrid.WorldVolume.Radius;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (updateSource)
            {
                case UpdateType.None:
                    break;
                case UpdateType.Terminal:
                    DestinationWaypoint = vc.GPSToVector(argument);
                    Runtime.UpdateFrequency = UpdateFrequency.Once;
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
                    if (!isGyroOver)
                    {
                        //MakeGyroOver(true);
                    }
                    //KeepDirectionTo(DestinationWaypoint);
                    if (ShipInProxyPoint())
                    {
                        //MakeGyroOver(false);
                        Runtime.UpdateFrequency = UpdateFrequency.Once;
                    }
                    break;
                case UpdateType.Once:
                    if (!checkFreeWay(DestinationWaypoint))
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    }
                    textPanel.WriteText("checkFreeWay true\n", true);
                    break;
                case UpdateType.IGC:
                    break;
                default:
                    break;
            }

        }

        bool checkFreeWay(Vector3D DestinationWaypoint)
        {
            var detectedSomething = cameraForRaycast.Raycast(DestinationWaypoint);
            if (detectedSomething.IsEmpty())
            {
                textPanel.WriteText("Fly...\n",true);
                FlyTo(DestinationWaypoint);
                return true;
            }
            else
            {
                textPanel.WriteText("Find...\n", true);
                //Сюда алгоритм нахождения иной точки
                var hitPos = (Vector3D)detectedSomething.HitPosition;
                SearchNFly(hitPos, DestinationWaypoint);
                return false;
            }
        }


        bool ShipInProxyPoint()
        {
            //remoteControl.CubeGrid.WorldAABB.Distance()
            if ((remoteControl.CurrentWaypoint.Coords - remoteControl.CubeGrid.GetPosition()).Length() < CubeGridRadius*3)
            {
                remoteControl.SetAutoPilotEnabled(false);
                remoteControl.ClearWaypoints();
                return true;
            }
            return false;
        }

        private void SearchNFly(Vector3D hitPos, Vector3D DestinationWaypoint)
        {
            var lengthToHitPos = (hitPos - cameraForRaycast.GetPosition()).Length();
            
            for (int i = 10; i < 1000; i+=10)
            {
                if(SpiralRaycast(cameraForRaycast, lengthToHitPos, i, cameraForRaycast.CubeGrid.WorldVolume.Radius))
                {
                    return;
                }
            }
            textPanel.WriteText("Error - no valid points, are you in pit/canyon?\n", true);
            throw new Exception("Error - no valid points, are you in pit/canyon?");
        }

        /// <summary>
        /// Спирадевидный опрос точек из центра
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lengthToHit"></param>
        /// <param name="sizeInPoints"></param>
        /// <param name="distanceBtwPoints"></param>
        /// <returns></returns>
        bool SpiralRaycast(IMyCameraBlock camera, double lengthToHit, int sizeInPoints, double distanceBtwPoints)
        {
            //Начиная с центра, обойти по спирали все элементы квадратной матрицы,
            //выводя их в порядке обхода.
            //Обход выполнять против часовой стрелки, первый ход - вправо.
            int i = 0;
            int j = 0;
            int direction = 0;// 0 - вправо, 1 - вверх, 2 - влево, 3 - вниз

            //var Forward = vc.WorldToLocal(DestinationWaypoint,camera);
            //Forward.Z = -lengthToHit + CubeGridRadius;
            var Forward = new Vector3D(0, 0, -lengthToHit + CubeGridRadius);//минус - особенность worldMAtrix камеры
            var Up = new Vector3D(0, 1, 0);
            var Right = new Vector3D(1, 0, 0);

            int distanceBtwPointsInt = (int)distanceBtwPoints;
            int sizeInMeters = sizeInPoints * (int)distanceBtwPoints;

            Vector3D predNotEmptyDotHitPoint= cameraForRaycast.GetPosition();
            do
            {
                for (int k = 1; k <= (direction + 2) / 2; ++k)
                {
                    Right.X = i;
                    Up.Y = j;
                    var vec = vc.LocalToWorld(Forward + Right + Up, camera);
                    //textPanel.WriteText(vc.VectorToGPS(vec, counter++.ToString()) + "\n", true);

                    var ray = camera.Raycast(vec);

                    if (ray.IsEmpty())//TODO: возможно нужна проверка что dot между локальными векторами положительный
                    {
                        //TODO: возможно нужно из набора точек выбрать с максимальной дальностью от HitPos

                        //Проверка что расстояние от предидущего хитпоинта больше радиуса boundingBox грида
                        if ((predNotEmptyDotHitPoint - vec).Length() > CubeGridRadius * 5)
                        {
                            textPanel.WriteText(vc.VectorToGPS(vec, $"R{Right.X} Up{Up.Y} - Летим сюда"), true);
                            textPanel.WriteText("\n", true);
                            FlyTo(vec);
                            return true;
                        }
                    }
                    else
                    {
                        predNotEmptyDotHitPoint = (Vector3D)ray.HitPosition;
                    }

                    switch (direction % 4)
                    {
                        case 0:
                            j+= distanceBtwPointsInt;//вправо
                            break;
                        case 1:
                            i-= distanceBtwPointsInt;//вверх
                            break;
                        case 2:
                            j-= distanceBtwPointsInt;//влево
                            break;
                        case 3:
                            i+= distanceBtwPointsInt;//вниз
                            break;
                    }
                }
                ++direction;

            } while (-sizeInMeters <= i && i <= sizeInMeters && -sizeInMeters <= j && j <= sizeInMeters);//пока не вышли за пределы
            return false;
        }

        private void FlyTo(Vector3D position, string waypointName= "AP Enabled")
        {
            remoteControl.ClearWaypoints();
            remoteControl.AddWaypoint(position, waypointName);
            remoteControl.SetAutoPilotEnabled(true);
        }


        //========================

        private void KeepDirectionTo(Vector3D pointToSeeWorld)
        {
            textPanel.WriteText("keep dir", true);
            Vector3D horizontalDirection = pointToSeeWorld.Cross(remoteControl.WorldMatrix.Forward);
            if (pointToSeeWorld.Dot(remoteControl.WorldMatrix.Forward) < 0)
            {
                horizontalDirection = Vector3D.Normalize(horizontalDirection);
            }
            SetGyro(horizontalDirection);
        }

        private void SetGyro(Vector3D axis)
        {
            foreach (IMyGyro gyro in gyrolist)
            {
                gyro.Yaw = (float)axis.Dot(gyro.CubeGrid.WorldMatrix.Up);
                gyro.Pitch = (float)axis.Dot(gyro.CubeGrid.WorldMatrix.Right);
                gyro.Roll = (float)axis.Dot(gyro.CubeGrid.WorldMatrix.Backward);
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


        /*private void KeepHorizon()
{
    Vector3D gravityVector = Vector3D.Normalize(remoteControl.GetNaturalGravity());
    Vector3D horizontalDirection = gravityVector.Cross(remoteControl.WorldMatrix.Down);
    if (gravityVector.Dot(remoteControl.WorldMatrix.Down) < 0)
    {
        horizontalDirection = Vector3D.Normalize(horizontalDirection);
    }
    SetGyro(horizontalDirection);
}*/
        /*void MainGyro(string arg, UpdateType uType)
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
        }*/


        //Ниже построчный опрос
        /*var Forward = new Vector3D(0, 0, -100);
        var Up = new Vector3D(0, 1, 0);
        var Right = new Vector3D(1, 0, 0);
        for (int i = -100; i <= 100; i += 10)
        {
            for (int j = -100; j <= 100; j += 10)
            {
                Right.X = i;
                Up.Y = j;
                var vec = vc.LocalToWorld(Forward + Right + Up, cameraForRaycast);
                var ray = cameraForRaycast.Raycast(vec);

                if (ray.IsEmpty())//TODO: возможно нужна проверка что dot между локальными векторами положительный
                {
                    //TODO: возможно нужно из набора точек выбрать с максимальной дальностью от HitPos

                    //Проверка что расстояние от предидущего хитпоинта больше радиуса boundingBox грида
                    if ((predNotEmptyDotHitPoint - vec).Length() > cameraForRaycast.CubeGrid.WorldVolume.Radius * 2)
                    {
                        textPanel.WriteText(vc.VectorToGPS(vec, $"R{Right.X} Up{Up.Y} - Летим сюда"), true);
                        textPanel.WriteText("\nDot normalized " + Vector3D.Normalize(Forward + Right + Up).Dot(new Vector3D(0, 0, -1)).ToString() + "\n", true);
                        textPanel.WriteText("\n", true);
                        FlyTo(vec);
                        return;
                    }
                }
                else
                {
                    predNotEmptyDotHitPoint = (Vector3D)ray.HitPosition;
                }


            }
        }
        textPanel.WriteText("Error - no valid points, are you in pit/canyon?", true);
        throw new Exception("Error - no valid points, are you in pit/canyon?");
        */
    }
}
