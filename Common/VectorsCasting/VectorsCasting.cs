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
    partial class Program
    {
        public class VectorsCasting
        {
            /// <summary>
            /// Cast world vector to local
            /// </summary>
            /// <param name="worldCoordinats">World vector</param>
            /// <returns>Local vector</returns>
            public Vector3D WorldToLocal(Vector3D worldCoordinats, IMyCubeBlock cubeBlock)
            {
                Vector3D mePosition = cubeBlock.CubeGrid.WorldMatrix.Translation;//also CubeGrid.GetPosition();
                Vector3D worldDirection = worldCoordinats - mePosition;
                return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(cubeBlock.CubeGrid.WorldMatrix));
            }

            /// <summary>
            /// Cast local direction vector to global
            /// </summary>
            /// <param name="local">Local direction vector</param>
            /// <returns>Global direction vector</returns>
            public Vector3D LocalToWorldDirection(Vector3D local, IMyCubeBlock cubeBlock)
            {
                Vector3D world1Direction = Vector3D.TransformNormal(local, cubeBlock.CubeGrid.WorldMatrix);
                Vector3D worldPosition = cubeBlock.CubeGrid.WorldMatrix.Translation + world1Direction;
                return worldPosition;
            }

            /// <summary>
            /// Cast local position vector to global
            /// </summary>
            /// <param name="local">Position vector</param>
            /// <returns>Global position vector</returns>
            public Vector3D LocalToWorld(Vector3D local, IMyCubeBlock cubeBlock)
            {
                return Vector3D.Transform(local, cubeBlock.WorldMatrix);
            }

            /// <summary>
            /// Cast local position vector to global
            /// </summary>
            /// <param name="local">Position vector</param>
            /// <returns>Global position vector</returns>
            public Vector3D LocalToWorld(Vector3D local, IMyCubeGrid grid)
            {
                return Vector3D.Transform(local, grid.WorldMatrix);
            }

            /// <summary>
            /// Cast Vector3D to string GPS format
            /// </summary>
            /// <param name="waypoint">Vector</param>
            /// <param name="name">Waypoint name</param>
            /// <param name="color">Waypoint color</param>
            /// <returns></returns>
            public string VectorToGPS(Vector3D waypoint, string name = "NAVOTHER", string color = "#FF75C9F1")
            {
                return $"GPS:{name}:{waypoint.X}:{waypoint.Y}:{waypoint.Z}:{color}";
            }

            /// <summary>
            /// Cast string GPS format to Vector3D 
            /// </summary>
            /// <param name="GPSCoordinats">GPS String</param>
            /// <returns>Vector</returns>
            public Vector3D GPSToVector(string GPSCoordinats)
            {
                var strarr = GPSCoordinats.Split(':');
                var name = strarr[1];
                var x = double.Parse(strarr[2]);
                var y = double.Parse(strarr[3]);
                var z = double.Parse(strarr[4]);
                var vector = new Vector3D(x, y, z);
                return vector;
            }
        }
    }
}
