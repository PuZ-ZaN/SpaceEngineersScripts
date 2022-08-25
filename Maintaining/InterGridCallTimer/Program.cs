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

//TODO: доделать
namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        IMyRadioAntenna Antenna;
        public Program()
        {
            Antenna = GridTerminalSystem.GetBlockWithName("Antenna") as IMyRadioAntenna;
        }

        public void Save()
        {
            
        }
        //MyIGCMessage
        //IMyBroadcastListener =
        //IMyUnicastListener
        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.IGC)
            {
                try
                {
                    (GridTerminalSystem.GetBlockWithName(argument) as IMyTimerBlock)?.Trigger();
                }
                catch
                {
                    (Antenna as IMyBroadcastListener).AcceptMessage();
                }
            }
        }
    }
}
