// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand mesh information for use with <see cref="HandRenderingMode.Mesh"/>.
    /// </summary>
    public struct HandMeshData
    {
        /// <summary>
        /// Constructs a new hand mesh data snapshot.
        /// </summary>
        /// <param name="vertices">The vertices of the hand mesh in the initial coordinate system.</param>
        /// <param name="triangles">Mesh triangle indices.</param>
        /// <param name="normals">Hand mesh normals, in initial coordinate system.</param>
        /// <param name="uvs">UV mapping of the hand.</param>
        public HandMeshData(
            Vector3[] vertices,
            int[] triangles,
            Vector3[] normals,
            Vector2[] uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            Normals = normals;
            Uvs = uvs;
        }

        /// <summary>
        /// The default value for hand mesh data.
        /// </summary>
        public static HandMeshData Empty { get; } = new HandMeshData(null, null, null, null);

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
        /// Returns true, if the mesh data is empty.
        /// </summary>
        public bool IsEmpty => Vertices == null || Vertices.Length == 0;
    }
}