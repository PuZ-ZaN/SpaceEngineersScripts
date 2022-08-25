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
        IMyPistonBase pistonDown;
        List<IMyPistonBase> pistonsUp;
        IMyCockpit cockpit;

        public Program()
        {
            pistonDown = GridTerminalSystem.GetBlockWithName("PistonDown") as IMyPistonBase;
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            pistonsUp = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlocksOfType(pistonsUp, a => a.CustomName.Contains("PistonUp"));
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (cockpit.MoveIndicator.Y != 0)
            {
                pistonDown.Velocity = cockpit.MoveIndicator.Y;
                pistonsUp.ForEach(a => a.Velocity = -cockpit.MoveIndicator.Y);
            }
            else
            {
                pistonDown.Velocity = -cockpit.MoveIndicator.Z;
                pistonsUp.ForEach(a => a.Velocity = -cockpit.MoveIndicator.Z);
            }
        }
    }
}
