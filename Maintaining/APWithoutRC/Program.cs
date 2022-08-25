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
        List<IMyThrust> thrusters = new List<IMyThrust>();
        IMyCockpit cockpit;
        IMyTextPanel display;
        Dictionary<Base6Directions.Direction,List<IMyThrust>> thrus = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
        public Program()
        {
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);
            foreach (var th in thrusters)
            {
                if (thrus.ContainsKey(Base6Directions.GetOppositeDirection(th.Orientation.Forward)))
                {
                    thrus[Base6Directions.GetOppositeDirection(th.Orientation.Forward)].Add(th);
                }
                else
                {
                    thrus.Add(Base6Directions.GetOppositeDirection(th.Orientation.Forward), new List<IMyThrust>() { th });
                }
            }
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            display = GridTerminalSystem.GetBlockWithName("Display") as IMyTextPanel;
            if (cockpit == null)
                display.WriteText("cockpit == null");
            if (display == null)
                display.WriteText("display == null");
            display.WriteText("");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            foreach (var types in thrus)
            {
                display.WriteText(types.Key + ":\n", true);
                foreach (var item in types.Value)
                {
                    display.WriteText(" " + item.CustomName + "\n", true);
                }
                
            }
            double F = cockpit.CalculateShipMass().TotalMass * cockpit.GetNaturalGravity().Length();
            double FPerThrust = F/thrus[Base6Directions.Direction.Up].Count;
            display.WriteText($"F = {F}");
            display.WriteText($"\nTotalMass = {cockpit.CalculateShipMass().TotalMass}",true);
            display.WriteText($"\nPhysicalMass = {cockpit.CalculateShipMass().PhysicalMass}",true);
            display.WriteText($"\nGrav = {cockpit.GetNaturalGravity().Length()}",true);
            display.WriteText($"\nF/16 = {F/thrus[Base6Directions.Direction.Up].Count}",true);
            display.WriteText($"\nTcount = {thrus[Base6Directions.Direction.Up].Count}",true);
            display.WriteText($"\nFPerThrust = {FPerThrust}",true);
            
            foreach (var item in thrus[Base6Directions.Direction.Up])
            {
                display.WriteText($"\n{item.CustomName} = {item.CurrentThrust}",true);
            }
            foreach (var item in thrus[Base6Directions.Direction.Up])
            {
                item.ThrustOverride = (float)FPerThrust;
            }
        }
    }
}
