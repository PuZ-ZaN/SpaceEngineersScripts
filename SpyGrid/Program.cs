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
        IMyCameraBlock cameraBlock;
        IMyTextPanel LCD;
        MyDetectedEntityInfo entityInfo;

        List<IMyGyro> gyros = new List<IMyGyro>();

        const string CamName = "cam";
        const string LCDName = "dis";
        const int CamDist = 10000;

        public Program()
        {
            cameraBlock = GridTerminalSystem.GetBlockWithName(CamName) as IMyCameraBlock;
            
            GridTerminalSystem.GetBlocksOfType(gyros);
            cameraBlock.EnableRaycast = true;

            LCD = GridTerminalSystem.GetBlockWithName(LCDName) as IMyTextPanel;
            LCD.ContentType = ContentType.TEXT_AND_IMAGE;
            LCD.WriteText(cameraBlock.RaycastDistanceLimit.ToString() + "\n");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument.Contains("shot"))
            {
                entityInfo = cameraBlock.Raycast(CamDist);

            }
            LCD.WriteText($"{vc.VectorToGPS(entityInfo.Position, entityInfo.Name)}\n", true);
            if (argument.Contains("clear"))
            {
                LCD.WriteText("");
            }
        }
    }
}
