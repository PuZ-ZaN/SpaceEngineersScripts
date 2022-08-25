using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SpaceEngineers.Game.ModAPI.Ingame;

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Immutable;
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
        double DropHeight = 20000; //Высота сброса бомбы

        IMyCameraBlock Camera; //Объявляем блоки
        IMyTextPanel LCD;
        IMyRemoteControl RC;

        Vector3D PlanetXYZ; //Координаты центра планеты
        Vector3D BaseXYZ; //Координаты базы
        Vector3D DropPointXYZ; //Здесь будут координаты точки сброса

        Dictionary<string, Action> behavior;


        //Конструктор скрипта
        // ------------------------------------------

        public Program()
        {
            behavior = new Dictionary<string, Action>() { 
                { "Detect", () => Detect() },
                { "Calculate", () => CalculateDropPoint() },
                { "Fly", () => FlyToDropPoint() },
            };
            //Находим блоки
            LCD = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
            if (LCD == null)
                throw new Exception("LCD is null!");

            Camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            if (Camera == null)
                throw new Exception("Cam is null!");
            Camera.EnableRaycast = true;

            RC = GridTerminalSystem.GetBlockWithName("RemoteControl") as IMyRemoteControl;
            if (Camera == null)
                throw new Exception("RC is null!");

            PlanetXYZ = new Vector3D();
            BaseXYZ = new Vector3D();
            DropPointXYZ = new Vector3D();
        }

        public void Main(string arg, UpdateType updateSource)
        {
            if (behavior.Keys.Contains(arg))
                behavior[arg]();
            else
                LCD.WriteText("incorrect arg");
            ////Разбор аргументов. Вызов функций raycast и расчета точки сброса.
            //if (arg == "Detect")
            //{
            //    Detect();
            //}
            //else if (arg == "Calculate")
            //{
            //    CalculateDropPoint();
            //}

        }

        // Рейкаст и установка координат центра планеты и базы
        void Detect()
        {
            MyDetectedEntityInfo DetectedObject = Camera.Raycast(10000, 0, 0);

            LCD.WriteText("Обнаружено: \n", false);
            LCD.WriteText("Объект: " + DetectedObject.Name + "\n", true);
            LCD.WriteText("Координаты: \n", true);
            LCD.WriteText("     X: " + DetectedObject.Position.X + "\n", true);
            LCD.WriteText("     Y: " + DetectedObject.Position.Y + "\n", true);
            LCD.WriteText("     Z: " + DetectedObject.Position.Z + "\n", true);

            string GPS = "\nGPS:" + DetectedObject.Name + ":" + DetectedObject.Position.X + ":"
                                  + DetectedObject.Position.Y + ":"
                                  + DetectedObject.Position.Z + ":";
            LCD.WriteText(GPS, true);
            //Если обнаруженный объект - планета, устанавливаем PlanetXYZ
            if (DetectedObject.Type == MyDetectedEntityType.Planet)
            {
                PlanetXYZ = DetectedObject.Position;
            }
            //Если обнаруженный объект - большой грид, устанавливаем BaseXYZ
            else if (DetectedObject.Type == MyDetectedEntityType.LargeGrid)
            {
                BaseXYZ = DetectedObject.Position;
            }
        }

        //Расчет точки сброса
        void CalculateDropPoint()
        {
            //Вектор вертикали, проходящий через базу
            Vector3D VerticalVector = BaseXYZ - PlanetXYZ;
            //Нормализация вертикали
            Vector3D VerticalNorm = Vector3D.Normalize(VerticalVector);
            //Расчет точки сброса
            DropPointXYZ = BaseXYZ + VerticalNorm * DropHeight;
            //Создаем GPS-метку
            string GPS = "GPS:DropPoint:" + DropPointXYZ.X + ":" + DropPointXYZ.Y + ":" + DropPointXYZ.Z + ":";
            LCD.WriteText("Точка сброса:\n" + GPS, false);
        }
        void FlyToDropPoint()
        {
            LCD.WriteText("Лечу в точку сброса");
            RC.ClearWaypoints();
            RC.AddWaypoint(DropPointXYZ, "DropPoint");
            RC.SetAutoPilotEnabled(true);
        }
    }
}
