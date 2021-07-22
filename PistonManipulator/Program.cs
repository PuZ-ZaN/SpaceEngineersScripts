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
        IMyCockpit cockpit;
        List<IMyPistonBase> pistonX;
        List<IMyPistonBase> pistonY;
        IMyMotorStator rotorA, rotorB, rotorC;
        IMyTextSurface LCD;

        Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("WorkerCockpit") as IMyCockpit;
            pistonX = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonX, b => b.CustomName.Contains("PistonX"));
            pistonY = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonY, b => b.CustomName.Contains("PistonY"));
            rotorA = GridTerminalSystem.GetBlockWithName("RotorA") as IMyMotorStator;
            rotorB = GridTerminalSystem.GetBlockWithName("RotorB") as IMyMotorStator;
            rotorC = GridTerminalSystem.GetBlockWithName("RotorC") as IMyMotorStator;
            LCD = cockpit.GetSurface(0);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        void Main()
        {
            //код для нескольких поршней
            float r = 17.5f;
            foreach (var p in pistonX)
            {
                r += p.CurrentPosition;
            }
            //float r = 17.5f + pistonX.CurrentPosition;// 17.5 - это расстояние между осями RotorA и RotorB при сложенном горизонтальном поршне
            //Моментальная скорость для горизонтального поршня
            //Теперь учитываем углы и ротора B, и ротора C

            float dX = (float)((cockpit.MoveIndicator.Z * Math.Cos(rotorC.Angle) - cockpit.MoveIndicator.Y * Math.Sin(rotorC.Angle)) * Math.Cos(rotorB.Angle) + cockpit.MoveIndicator.X * Math.Cos(rotorB.Angle + Math.PI / 2));
            //А это для ротора А
            float dR = (float)((cockpit.MoveIndicator.Z * Math.Cos(rotorC.Angle) - cockpit.MoveIndicator.Y * Math.Sin(rotorC.Angle)) * Math.Sin(rotorB.Angle) + cockpit.MoveIndicator.X * Math.Sin(rotorB.Angle + Math.PI * 0.5)) / r;
            //ТожеМое
            pistonX.ForEach(a => a.Velocity = -dX * 2 / pistonX.Count);
            rotorA.TargetVelocityRad = dR * 2;
            //Вертикальные поршни реагируют на проекции сигналов W S Space C на вертикаль
            foreach (IMyPistonBase p in pistonY)
            {
                p.Velocity = -2 * (float)(cockpit.MoveIndicator.Z * Math.Sin(rotorC.Angle) + cockpit.MoveIndicator.Y * Math.Cos(rotorC.Angle)) / pistonY.Count;
            }
            //Скорость ротора, на котором подвешен рабочий кокпит, складывается из компенсации вращения основного ротора и сигнала влево-вправо с кокпита
            rotorB.TargetVelocityRad = dR * 2 - cockpit.RotationIndicator.Y / 24;
            //Наклон инструмента.
            rotorC.TargetVelocityRad = cockpit.RotationIndicator.X / 24;

            LCD.WriteText("\nAngle: " + Math.Round(rotorA.Angle * 180 / Math.PI, 2), false);
            LCD.WriteText("\nRadius: " + Math.Round(pistonX.First().CurrentPosition * pistonX.Count, 2), true);
            LCD.WriteText("\nHeight: " + Math.Round(pistonY[0].CurrentPosition * pistonY.Count, 2), true);
        }
    }
}