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
/// <summary>
/// LocalMatrixTest - тест векторов и матриц
/// </summary>
namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        const string TextPanel = "Прозрачная ЖК-панель 3";
        const string CameraName = "Камера";

        IMyTextPanel textPanel;
        IMyCameraBlock cameraForRaycast;

        VectorsCasting vc;

        public Program()
        {
            cameraForRaycast = GridTerminalSystem.GetBlockWithName(CameraName) as IMyCameraBlock;
            cameraForRaycast.EnableRaycast = true;

            textPanel = GridTerminalSystem.GetBlockWithName(TextPanel) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            vc = new VectorsCasting();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            textPanel.WriteText("\n", false);
            var Forward = new Vector3D(0, 0, -100);
            var Up = new Vector3D(0, 1, 0);
            var Right = new Vector3D(1, 0, 0);
            for (int i = -100; i <= 100; i+=10)
            {
                for (int j = -100; j <= 100; j+=10)
                {
                    Right.X = i;
                    Up.Y = j;
                    var vec = vc.LocalToWorld(Forward + Right + Up, cameraForRaycast);
                    //textPanel.WriteText(vc.VectorToGPS(vec, $"F100 R{Right.X} Up{Up.Y} - cam"), true);
                    //textPanel.WriteText("\n", true);
                    var ray = cameraForRaycast.Raycast(vec);
                    if (!ray.IsEmpty())
                    {
                        textPanel.WriteText(vc.VectorToGPS(vec, $"R{Right.X} Up{Up.Y} - not empty"), true);
                        textPanel.WriteText("\n", true);
                    }

                }
            }
            textPanel.WriteText($"RX = {Right.X}", true);
            textPanel.WriteText("\n", true);
        }
        void SpiralRaycast(IMyCubeBlock camera, double lengthToHit, int sizeInPoints, int distanceBtwPoints)
        {
            //Начиная с центра, обойти по спирали все элементы квадратной матрицы,
            //выводя их в порядке обхода.
            //Обход выполнять против часовой стрелки, первый ход - вправо.
            int i = sizeInPoints / 2; //начнем с среднего элемента
            int j = sizeInPoints / 2;
            int direction = 0;// 0 - вправо, 1 - вверх, 2 - влево, 3 - вниз
            int counter = 0;

            var Forward = new Vector3D(0, 0, -lengthToHit);//минус - особенность worldMAtrix камеры
            var Up = new Vector3D(0, 1, 0);
            var Right = new Vector3D(1, 0, 0);

            do
            {
                for (int k = 1; k <= (direction + 2) / 2; ++k)
                {
                    switch (direction % 4)
                    {
                        case 0:
                            ++j;//вправо
                            break;
                        case 1:
                            --i;//вверх
                            break;
                        case 2:
                            --j;//влево
                            break;
                        case 3:
                            ++i;//вниз
                            break;
                    }
                    Right.X = i;
                    Up.Y = j;
                    var vec = vc.LocalToWorld(Forward + Right + Up, cameraForRaycast);
                    textPanel.WriteText(vc.VectorToGPS(vec, counter++.ToString()) + "\n", true);
                }
                ++direction;

            } while (0 <= i && i < sizeInPoints && 0 <= j && j < sizeInPoints);//пока не вышли за пределы
        }
    }
}
