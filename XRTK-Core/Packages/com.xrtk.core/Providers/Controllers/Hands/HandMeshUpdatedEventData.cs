using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// See BaseHandVisualizer.OnHandMeshUpdated for an example of how to use the
    /// hand mesh info to render a mesh.
    /// </summary>
    public class HandMeshUpdatedEventData
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
    }
}