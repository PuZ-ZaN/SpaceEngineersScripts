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
        IMyMotorStator rotorXminus;
        IMyMotorStator rotorXplus;
        List<IMyMotorStator> rotorsY;
        IMyCockpit cockpit;
        IMyTextSurface LCD;
        public Program()
        {
            rotorXplus = GridTerminalSystem.GetBlockWithName("rotorXplus") as IMyMotorStator;
            rotorXminus = GridTerminalSystem.GetBlockWithName("rotorXminus") as IMyMotorStator;
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            LCD = cockpit.GetSurface(0);
            rotorsY = new List<IMyMotorStator>();
            GridTerminalSystem.GetBlocksOfType(rotorsY, a => a.CustomName.Contains("RotorY"));
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (cockpit.IsUnderControl)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                LCD.WriteText(String.Format("X: {0}\nY: {1}", cockpit.RotationIndicator.X, cockpit.RotationIndicator.Y));
                rotorXminus.TargetVelocityRPM = -cockpit.RotationIndicator.X;
                rotorXplus.TargetVelocityRPM = +cockpit.RotationIndicator.X;
                rotorsY.ForEach(a => a.TargetVelocityRPM = cockpit.RotationIndicator.Y);
            }
            else
            {
                LCD.ClearImagesFromSelection();
                rotorXminus.TargetVelocityRPM = 0;
                rotorXplus.TargetVelocityRPM = 0;
                rotorsY.ForEach(a => a.TargetVelocityRPM = 0);
                Runtime.UpdateFrequency = UpdateFrequency.Once;
            }
        }
    }
}
