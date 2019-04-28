// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Utilities
{
    public static class DebugUtilities
    {
        /// <summary>
        /// Draws a point in the Scene window.
        /// </summary>
        public static void DrawPoint(Vector3 point, Color color, float size = 0.05f)
        {
            DrawPoint(point, Quaternion.identity, color, size);
        }

        /// <summary>
        /// Draws a point with a rotation in the Scene window.
        /// </summary>
        public static void DrawPoint(Vector3 point, Quaternion rotation, Color color, float size = 0.05f)
        {
            Vector3[] axes = { rotation * Vector3.up, rotation * Vector3.right, rotation * Vector3.forward };

            for (int i = 0; i < axes.Length; ++i)
            {
                Vector3 a = point + size * axes[i];
                Vector3 b = point - size * axes[i];
                Debug.DrawLine(a, b, color);
            }
        }
    }
}