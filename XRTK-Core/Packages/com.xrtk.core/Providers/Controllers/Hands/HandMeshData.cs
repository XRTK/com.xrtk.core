using UnityEngine;

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
        public Vector3[] Vertices { get; }

        /// <summary>
        /// Mesh triangle indices.
        /// </summary>
        public int[] Triangles { get; }

        /// <summary>
        /// Hand mesh normals, in initial coordinate system.
        /// </summary>
        public Vector3[] Normals { get; }

        /// <summary>
        /// UV mapping of the hand. 
        /// </summary>
        public Vector2[] Uvs { get; }

        /// <summary>
        /// Translation to apply to mesh to go from initial coordinates to world coordinates.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Rotation to apply to mesh to go from initial coordinates to world coordinates.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Returns true, if the mesh data is empty.
        /// </summary>
        public bool Empty => Vertices == null || Vertices.Length == 0;
    }
}