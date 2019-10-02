using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand mesh information.
    /// </summary>
    public class HandMeshData
    {
        /// <summary>
        /// The vertices of the hand mesh in the initial coordinate system.
        /// </summary>
        public Vector3[] Vertices { get; set; }

        /// <summary>
        /// Mesh triangle indices.
        /// </summary>
        public int[] Triangles { get; set; }

        /// <summary>
        /// Hand mesh normals, in initial coordinate system.
        /// </summary>
        public Vector3[] Normals { get; set; }

        /// <summary>
        /// UV mapping of the hand. TODO: Give more specific details about UVs.
        /// </summary>
        public Vector2[] Uvs { get; set; }

        /// <summary>
        /// Translation to apply to mesh to go from initial coordinates to world coordinates.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation to apply to mesh to go from initial coordinates to world coordinates.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Handedness of the updated hand.
        /// </summary>
        public Handedness Handedness { get; set; }

        /// <summary>
        /// Returns true, if the mesh data is empty.
        /// </summary>
        public bool Empty => Vertices == null || Vertices.Length == 0;
    }
}