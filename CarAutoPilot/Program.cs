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
        IMyRemoteControl remoteControl;
        public Program()
        {
            remoteControl = GridTerminalSystem.GetBlockWithName("RemoteControl") as IMyRemoteControl;

            remoteControl.WaitForFreeWay = true;
            remoteControl.SpeedLimit = 10;
            remoteControl.FlightMode = FlightMode.OneWay;
            remoteControl.SetCollisionAvoidance(true);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Vector3D nearestPlayerCrds;
            if (remoteControl.GetNearestPlayer(out nearestPlayerCrds))
            {
                Vector3D thisPos = Me.CubeGrid.GetPosition();
                var distanceToPlayer = (nearestPlayerCrds - thisPos).Length();
                Echo("NearestPlayer Lenght: " + distanceToPlayer);
                remoteControl.ClearWaypoints();
                if (distanceToPlayer > 4)
                {
                    remoteControl.AddWaypoint(new MyWaypointInfo("PlayerCoords", nearestPlayerCrds));
                    remoteControl.SetAutoPilotEnabled(true);
                }
            }
            else
            {
                Echo("NearestPlayer not found!");
            }
        }
    }
    /*public class Behavior
    {
        public Dictionary<Func<bool>, Action> behaviors = new Dictionary<Func<bool>, Action>();

        public IMyCockpit Cockpit;

        public Behavior(IMyCockpit cockpit)
        {
            Cockpit = cockpit;
            FillUpBehaviors();
        }

        public void Exec()
        {
            foreach (var behavior in behaviors)
                if (behavior.Key())
                    behavior.Value();
        }
        public void Add(Func<bool> func, Action action)
        {
            behaviors.Add(func, action);
        }
        public bool Delete(Func<bool> func) =>
            behaviors.Remove(func);

        void FillUpBehaviors()
        {
            //в разработке
            //Add(() => !Cockpit.IsUnderControl, () => Cockpit.DampenersOverride = true);
            Add(new Func<bool>(() => !Cockpit.IsUnderControl), new Action(() => Cockpit.DampenersOverride = true));
        }
    }*/
}
