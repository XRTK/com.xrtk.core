// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Extensions
{
    public static class CollisionExtensions
    {
        /// <summary>
        /// Checks if a collision is valid using a prioritized layer mask collection.
        /// </summary>
        /// <param name="collision"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <returns>True, if the collision is valid.</returns>
        public static bool IsValidCollision(this Collision collision, LayerMask[] prioritizedLayerMasks)
        {
            return collision.collider.IsValidCollision(prioritizedLayerMasks);
        }
    }
}
