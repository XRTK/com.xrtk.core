// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Quaternion struct.
    /// </summary>
    public static class QuaternionExtensions
    {
        public static bool IsValidRotation(this Quaternion rotation)
        {
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) &&
                   !float.IsInfinity(rotation.x) && !float.IsInfinity(rotation.y) && !float.IsInfinity(rotation.z) && !float.IsInfinity(rotation.w);
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothTo(this Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }
    }
}