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
        const string AntennaName = "Антенна";
        IMyRadioAntenna antenna;

        IMyBroadcastListener bl;
        string blTag = "blt";

        public Program()
        {
            bl = IGC.RegisterBroadcastListener(blTag);
            bl.SetMessageCallback(blTag);
            //antenna = GridTerminalSystem.GetBlockWithName(AntennaName) as IMyRadioAntenna;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Terminal)
                IGC.SendBroadcastMessage(blTag, argument);
        }
    }
}
