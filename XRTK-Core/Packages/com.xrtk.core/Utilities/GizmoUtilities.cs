// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace XRTK.Utilities
{
    /// <summary>
    /// Gizmo drawing utilities.
    /// </summary>
    public static class GizmoUtilities
    {
#if UNITY_EDITOR

        /// <summary>
        /// Renders a capsule wire gizmo.
        /// </summary>
        /// <param name="position">Center position of the capsule.</param>
        /// <param name="rotation">Center rotation of the capsule.</param>
        /// <param name="radius">Capsule radius.</param>
        /// <param name="height">Capsule height.</param>
        /// <param name="color">Optional color override, will use <see cref="Gizmos.color"/> by default.</param>
        public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color color = default)
        {
            if (color != default)
            {
                Handles.color = color;
            }
            else
            {
                Handles.color = Gizmos.color;
            }

            Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;

                // Draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);

                // Draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);

                // Draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);

            }
        }

#endif
    }
}