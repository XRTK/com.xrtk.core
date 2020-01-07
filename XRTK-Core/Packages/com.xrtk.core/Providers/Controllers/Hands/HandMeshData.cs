using UnityEngine;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand mesh information.
    /// </summary>
    public class HandMeshData
    {
        /// <summary>
        /// Constructs a new empty hand mesh data snapshot.
        /// </summary>
        public HandMeshData() { }

        /// <summary>
        /// Constructs a new hand mesh data snapshot.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="normals"></param>
        /// <param name="uvs"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public HandMeshData(Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uvs, Vector3 position, Quaternion rotation)
        {
            Vertices = vertices;
            Triangles = triangles;
            Normals = normals;
            Uvs = uvs;
            Position = position;
            Rotation = rotation;
        }

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