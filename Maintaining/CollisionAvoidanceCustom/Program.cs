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
        //пока не надо
        //Vector3D targetPoint;
        //на случай если понадобится вернуться домой, в случае неудачи
        //Vector3D homePoint;

        IMyCameraBlock raycaster;
        MyDetectedEntityInfo obstruction;
        IMyTextPanel LCD;
        IMyRemoteControl rc;
        public Program()
        {
            raycaster = GridTerminalSystem.GetBlockWithName("Raycaster") as IMyCameraBlock;
            Echo("Constructor!");
            if (raycaster == null)
                Echo("CAMERA NULL");
            raycaster.EnableRaycast = true;

            //rc = GridTerminalSystem.GetBlockWithName("RC") as IMyRemoteControl;

            LCD = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
            if (LCD == null)
                Echo("LCD NULL");
        }


        public void Main(string argument, UpdateType updateSource)
        {
            if (LCD == null)
                Echo("LCD NULL - RUNTIME");
            //LCD.WriteText("!");
            obstruction = raycaster.Raycast(10000);//targetPoint
            var hit = obstruction.HitPosition;
            if (hit != null) {
                var hitToLoc = WorldToLocal((Vector3D)hit);
                var vec = raycaster.CubeGrid.WorldMatrix.Forward 
                    + raycaster.CubeGrid.WorldMatrix.Up 
                    + raycaster.CubeGrid.WorldMatrix.tr;
                Echo(raycaster.WorldMatrix.ToString());
                //Me.CubeGrid.GridIntegerToWorld
                /*  obstruction.Name + "\n"+        + obstruction.HitPosition?.ToString() + "\n"
                    + VectorToGPS(hitToLoc, 5) + "\n Local " + "\n Wrld "*/
                LCD.WriteText(VectorToGPS(LocalToWorld(vec), 2)//new Vector3D(10,0,10)
                    );
            }
        }

        Vector3D WorldToLocal(Vector3D worldVec)
        {
            Vector3D worldDirection = worldVec - Me.CubeGrid.GetPosition();
            Vector3D localPosition = Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(Me.CubeGrid.WorldMatrix));
            return localPosition;
        }
        Vector3D LocalToWorld(Vector3D localVec)
        {
            Vector3D world1Direction = Vector3D.TransformNormal(localVec, Me.CubeGrid.WorldMatrix);
            Vector3D worldPosition = Me.CubeGrid.WorldMatrix.Translation + world1Direction;
            return worldPosition;
        }
        //CubeGrid.WorldToGridInteger
        //        .GridIntegerToWorld
        public void Save()
        {
            Storage = "";
        }

        Vector3D rotateVecTo90(Vector3D sourceVec)
        {
            return new Vector3D(10, 0, 10);
            var perpendicular = Vector3D.CalculatePerpendicularVector(sourceVec);
            //нужно найти матрицу поворота и умножить на нее sourceVec
            //return sourceVec*Me.CubeGrid.WorldMatrix.
        }

        string VectorToGPS(Vector3D vec,int round)
        {
            return "GPS:TGT:" + 
                Math.Round(vec.X, round) + ":" + 
                Math.Round(vec.Y, round) + ":" + 
                Math.Round(vec.Z, round) + ":#FF75C9F1";
        }

        //возможно понадобится если хранить несколько точек
        void LoadStorage()
        {

        }
    }
}
