// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Quaternion struct.
    /// </summary>
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Checks if a <see cref="Quaternion"/> instance is valid. A <see cref="Quaternion"/> is considered
        /// valid, if none of its components is <see cref="float.IsInfinity(float)"/> nor <see cref="float.IsNaN(float)"/>.
        /// </summary>
        /// <param name="rotation">The <see cref="Quaternion"/> to validate.</param>
        /// <returns>True, if valid rotation.</returns>
        public static bool IsValidRotation(this Quaternion rotation)
        {
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) &&
                   !float.IsInfinity(rotation.x) && !float.IsInfinity(rotation.y) && !float.IsInfinity(rotation.z) && !float.IsInfinity(rotation.w);
        }

        /// <summary>
        /// Checks the <see cref="Quaternion"/> can be approximately considered the same rotation when compared
        /// to another <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The original <see cref="Quaternion"/>.</param>
        /// <param name="other">The <see cref="Quaternion"/> to compare against.</param>
        /// <param name="threshold">Threshold in degrees to consider rotations the same.</param>
        /// <returns>True, if both rotations are approximately the same.</returns>
        public static bool Approximately(this Quaternion quaternion, Quaternion other, float threshold)
        {
            var qEuler = quaternion.eulerAngles;
            var otherEuler = other.eulerAngles;

            return
                Math.Abs(qEuler.x - otherEuler.x) <= threshold &&
                Math.Abs(qEuler.y - otherEuler.y) <= threshold &&
                Math.Abs(qEuler.z - otherEuler.z) <= threshold;
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