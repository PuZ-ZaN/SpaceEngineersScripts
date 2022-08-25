using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SpaceEngineers.Game.ModAPI.Ingame;

using System;
using System.Collections;
using System.Collections.Generic;
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
        IMyMotorStator rotorXminus;
        IMyMotorStator rotorXplus;
        List<IMyMotorStator> rotorsY;
        IMyCockpit cockpit;
        IMyTextSurface LCD;
        bool parking = true;//"Локальная инициализация" см if-ы
        public Program()
        {
            rotorXplus = GridTerminalSystem.GetBlockWithName("rotorXplus") as IMyMotorStator;
            rotorXminus = GridTerminalSystem.GetBlockWithName("rotorXminus") as IMyMotorStator;
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            LCD = cockpit.GetSurface(0);
            LCD.Alignment = TextAlignment.CENTER;
            rotorsY = new List<IMyMotorStator>();
            GridTerminalSystem.GetBlocksOfType(rotorsY, a => a.CustomName.Contains("RotorY"));
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            //LCD.WriteText(parking.ToString());
            //LCD.WriteText(Runtime.UpdateFrequency.ToString(), true);
            if (cockpit.IsUnderControl)
            {
                if (parking)
                {
                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    LCD.ContentType = ContentType.TEXT_AND_IMAGE;
                    LCD.WriteText("UNDER CONTROL");
                    parking = false;
                }
                rotorXminus.TargetVelocityRPM = -cockpit.RotationIndicator.X;
                rotorXplus.TargetVelocityRPM = +cockpit.RotationIndicator.X;
                rotorsY.ForEach(a => a.TargetVelocityRPM = cockpit.RotationIndicator.Y);
            }
            else
            {
                if (!parking)
                {
                    LCD.ContentType = ContentType.NONE;
                    rotorXminus.TargetVelocityRPM = 0;
                    rotorXplus.TargetVelocityRPM = 0;
                    rotorsY.ForEach(a => a.TargetVelocityRPM = 0);
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    parking = true;
                }
            }
        }
    }
}
