// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Extensions.XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's <see cref="Collider"/>
    /// </summary>
    public static class ColliderExtensions
    {
        /// <summary>
        /// Gets all the corner points of the collider's bounds in world space.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="transform"></param>
        /// <param name="positions"></param>
        public static void GetCornerPositionsWorldSpace(this Collider collider, Transform transform, ref Vector3[] positions)
        {
            // Store current rotation then zero out the rotation so that the bounds
            // are computed when the object is in its 'axis aligned orientation'.
            var currentRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            Physics.SyncTransforms(); // Update collider bounds

            var bounds = collider.bounds;
            bounds.GetCornerPositions(ref positions);

            // After bounds are computed, restore rotation...
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.rotation = currentRotation;
            Physics.SyncTransforms();

            // Rotate our points in case the object is also rotated.
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = positions[i].RotateAroundPoint(bounds.center, transform.rotation);
            }
        }
    }
}
