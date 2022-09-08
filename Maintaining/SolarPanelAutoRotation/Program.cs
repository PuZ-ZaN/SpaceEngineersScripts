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
        const string rotorXName = "rotorX";
        const string rotorYName = "rotorY";
        const string solarName = "Solar Panel";
        const string LCDName = "LCD";

        IMySolarPanel solarRef;
        IMyTextPanel LCD;
        IMyMotorStator rotorX;
        //IMyMotorStator rotorY;
        //bool isFoundSun = true;//or simply check power
        States state = States.XFoundingMaxPwr;

        float lastPower = 0;
        float RPMs = 0;
        float LastPwr = 1;
        float StartXPosition;
        float pwrDelta = 1;
        float lastPwrDelta = 1;
        bool XFoundingMaxPwrStart = true;
        bool TrackingSunStart = true;
        DateTime lastRuntime = DateTime.Now;
        Queue<float> lastPwrs = new Queue<float>();
        float sumLastPwrs = 0;
        float AvgLastPwrs = 0;
        float maxApLocal = 0;
        public Program()
        {
            rotorX = GridTerminalSystem.GetBlockWithName(rotorXName) as IMyMotorStator;
            //rotorY = GridTerminalSystem.GetBlockWithName(rotorYName) as IMyMotorStator;
            solarRef = GridTerminalSystem.GetBlockWithName(solarName) as IMySolarPanel;
            LCD = GridTerminalSystem.GetBlockWithName(LCDName) as IMyTextPanel;
            rotorX.TargetVelocityRPM = 0;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {

            switch (state)
            {
                case States.XFoundingMaxPwr:
                    if (XFoundingMaxPwrStart)
                    {
                        rotorX.TargetVelocityRPM = 1;
                        StartXPosition = RadiansToDegrees(rotorX.Angle);
                        XFoundingMaxPwrStart = false;
                        lastPwrs.Enqueue(solarRef.MaxOutput);
                        return;
                    }
                    LCD.WriteText("");
                    foreach (var pwr in lastPwrs)
                    {
                        LCD.WriteText($"{pwr},",true);
                        sumLastPwrs += pwr;
                    }
                    LCD.WriteText("\n");
                    maxApLocal = solarRef.MaxOutput;
                    AvgLastPwrs = sumLastPwrs / lastPwrs.Count;
                    RPMs = (maxApLocal - AvgLastPwrs) * 100;
                    rotorX.TargetVelocityRPM = RPMs < 0.05 ? 0 : RPMs;

                    

                    if (lastPwrs.Count > 4)
                    {
                        lastPwrs.Dequeue();
                    }
                    lastPwrs.Enqueue(maxApLocal);
                    break;
                case States.YFoundingMaxPwr:
                    break;
                case States.TrackingSun:
                    if (TrackingSunStart)
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                        TrackingSunStart = false;
                    }

                    break;
                default:
                    break;
            }

            //RPMs = solarRef.CurrentOutput - lastPower;
            //LCD.WriteText(RPMs.ToString());
            //LCD.WriteText(lastPower.ToString(),true);
            //rotorX.TargetVelocityRPM = RPMs;
            //lastPower = solarRef.CurrentOutput;
        }
        enum States
        {
            XFoundingMaxPwr = 0,
            YFoundingMaxPwr = 1,
            TrackingSun = 2
        }
        float RadiansToDegrees(float radians)
        {
            var deg = (Math.Round(radians * (180 / Math.PI)));
            return (float)deg;
        }
    }
}
