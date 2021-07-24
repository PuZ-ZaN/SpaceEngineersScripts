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
        /*IMyCameraBlock camera;
        IMyRemoteControl remoteControl;
        IMyCockpit cockpit;
        IMyTextSurface display;
        public Program()
        {
            camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            remoteControl = GridTerminalSystem.GetBlockWithName("RemoteControl") as IMyRemoteControl;
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            
            display = cockpit.GetSurface(0);
            camera.EnableRaycast = true;

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

       
        public void Main(string argument, UpdateType updateSource)
        {

            var strRes = new StringBuilder();

            var canScan = camera.CanScan(1000);
            
            strRes.Append("\nCan scan?: " + canScan.ToString());
            strRes.Append("\nAvailableScanRange: " + camera.AvailableScanRange);
            if (canScan)
            {
                var rayObj = camera.Raycast(camera.RaycastDistanceLimit);
                strRes.Append("\nName " + rayObj.Name);
                strRes.Append("\nDist " + (rayObj.Position + Me.CubeGrid.GetPosition()).ToString());
                strRes.Append("\nType: " + rayObj.Type.ToString());
            }
            display.WriteText(strRes);
        }*/

        //===============================================
        IMyCameraBlock camera;
        IMyTextSurface display;

        const double SCAN_DISTANCE = 1;
        const float PITCH = 0;
        const float YAW = 0;

        private StringBuilder sb = new StringBuilder();
        private StringBuilder enemies = new StringBuilder();
        private MyDetectedEntityInfo info;
        private List<MyDetectedEntityInfo> detectedEnemies = new List<MyDetectedEntityInfo>();
        public Program()
        {
            camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            display = GridTerminalSystem.GetBlockWithName("Display") as IMyTextSurface;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            Echo("wrf");
            if (camera.CanScan(SCAN_DISTANCE))
                info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);
            else
                info = new MyDetectedEntityInfo();

            Echo(camera.EnableRaycast.ToString());
            Echo(camera.AvailableScanRange.ToString());
            sb.AppendLine();
            sb.Append(camera.CustomName);
            sb.AppendLine();
            sb.Append("EntityID: " + info.EntityId);
            sb.AppendLine();
            sb.Append("Name: " + info.Name);
            sb.AppendLine();
            sb.Append("Type: " + info.Type);
            sb.AppendLine();
            sb.Append("Velocity: " + info.Velocity.ToString("0.000"));
            sb.AppendLine();
            sb.Append("Relationship: " + info.Relationship);
            sb.AppendLine();
            sb.Append("Size: " + info.BoundingBox.Size.ToString("0.000"));
            sb.AppendLine();
            sb.Append("Position: " + info.Position.ToString("0.000"));

            if (info.HitPosition.HasValue)
            {
                int index = detectedEnemies.FindIndex(f => f.EntityId == info.EntityId);
                if (index >= 0)
                    Echo("already have that enemy grid, skiping");
                else
                    detectedEnemies.Add(info);
            }

            sb.AppendLine();
            sb.Append("Range: " + camera.AvailableScanRange.ToString());
            sb.AppendLine();
            foreach (var enem in detectedEnemies)
            {
                enemies.AppendLine();
                enemies.Append(enem.Name);
                enemies.AppendLine();
                enemies.Append("Hit: " + info.HitPosition.Value.ToString("0.000"));
                enemies.AppendLine();
                enemies.Append("Distance: " + Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value).ToString("0.00"));
                enemies.AppendLine();
            }
            display.WriteText(sb.ToString());
            display.WriteText(enemies.ToString());
            Echo(sb.ToString());
            Echo(enemies.ToString());
            sb.Clear();
            enemies.Clear();
        }
    }
}
