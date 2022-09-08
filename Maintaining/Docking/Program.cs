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
        List<IMyShipConnector> connectors;
        IMyShipConnector ShipConnector;
        IMyShipConnector StationConnector;
        IMyTextPanel textPanel;
        VectorsCasting vc = new VectorsCasting();
        public Program()
        {
            //GridTerminalSystem.GetBlocksOfType(connectors,
            //    block => block.IsSameConstructAs(Me) && block.Status == MyShipConnectorStatus.Connected
            //    );
            //ShipConnector = connectors[0];
            ShipConnector = GridTerminalSystem.GetBlockWithName("ConnUp") as IMyShipConnector;
            StationConnector = ShipConnector.OtherConnector;
            textPanel = GridTerminalSystem.GetBlockWithName("Display") as IMyTextPanel;
            textPanel.WriteText($"{StationConnector == null}");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            textPanel.WriteText("");
            Vector3D ShipConnectorWorldPos = ShipConnector.GetPosition();
            Vector3D ShipConnectorGridPos = ShipConnector.Position;
            Vector3D StationConnectorWorldPos = StationConnector.GetPosition();
            Vector3D StationConnectorGridPos = StationConnector.Position;


            textPanel.WriteText($"ShipConnectorWorldPos = {ShipConnectorWorldPos}\n", true);
            textPanel.WriteText($"ShipConnectorGridPos = {ShipConnectorGridPos}\n", true);
            textPanel.WriteText($"StationConnectorWorldPos = {StationConnectorWorldPos}\n", true);
            textPanel.WriteText($"StationConnectorGridPos = {StationConnectorGridPos}\n", true);
            textPanel.WriteText($"LocalToWorld = {vc.VectorToGPS(vc.LocalToWorld(StationConnector.WorldMatrix.Forward + new Vector3D(0, 0, -1), StationConnector))}\n", true);
            //textPanel.WriteText($"LocalToWorldDir = {vc.VectorToGPS(vc.LocalToWorldDirection(StationConnector.WorldMatrix.Forward, StationConnector))}\n", true);
        }
    }
}
