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
        VectorsCasting vc = new VectorsCasting();
        Gyros gyros;

        IMyCockpit cockpit;
        List<IMyGyro> gyrolist;
        IMyTextPanel textPanel;
        
        Vector3D TargetVector;
        public Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            gyrolist = new List<IMyGyro>();
            GridTerminalSystem.GetBlocksOfType(gyrolist, gyro => gyro.IsSameConstructAs(cockpit));

            gyros = new Gyros(cockpit, gyrolist);

            textPanel = GridTerminalSystem.GetBlockWithName("Display") as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            textPanel.WriteText("");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!argument.Contains("stop"))
            {
                if (argument != "")
                {
                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    TargetVector = vc.GPSToVector(argument);
                }
                var locvec = vc.WorldToLocal(TargetVector, cockpit);

                textPanel.WriteText($"{locvec}\n");
                textPanel.WriteText($"{gyros.IsShipOrientedNormalized(locvec,2)}\n",true);
                textPanel.WriteText($"{Vector3D.Normalize(locvec)}",true);

                gyros.KeepDirection(locvec);
            }
            else
            {
                gyros.EndKeepDirection();
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }
    }
}
