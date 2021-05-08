// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Extensions
{
    public static class SystemNumericsExtensions
    {
        /// <summary>
        /// Converts a Numerics <see cref="System.Numerics.Vector3"/> to a Unity <see cref="UnityEngine.Vector3"/>.
        /// </summary>
        /// <param name="v">Vector value to convert.</param>
        /// <returns><see cref="UnityEngine.Vector3"/>.</returns>
        public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, -v.Z);
        }

        /// <summary>
        /// Converts a Unity <see cref="UnityEngine.Vector3"/> to a Numerics <see cref="System.Numerics.Vector3"/>.
        /// </summary>
        /// <param name="v">Vector value to convert.</param>
        /// <returns><see cref="System.Numerics.Vector3"/>.</returns>
        public static System.Numerics.Vector3 ToNumericsVector3(this UnityEngine.Vector3 v)
        {
            return new System.Numerics.Vector3(v.x, v.y, -v.z);
        }

        /// <summary>
        /// Converts a Numerics <see cref="System.Numerics.Quaternion"/> to a Unity <see cref="UnityEngine.Quaternion"/>.
        /// </summary>
        /// <param name="q">Quaternion value to convert.</param>
        /// <returns><see cref="UnityEngine.Quaternion"/>.</returns>
        public static UnityEngine.Quaternion ToUnity(this System.Numerics.Quaternion q)
        {
            return new UnityEngine.Quaternion(-q.X, -q.Y, q.Z, q.W);
        }

        /// <summary>
        /// Converts a Unity <see cref="UnityEngine.Quaternion"/> to a Numerics <see cref="System.Numerics.Quaternion"/>.
        /// </summary>
        /// <param name="q">Quaternion value to convert.</param>
        /// <returns><see cref="System.Numerics.Quaternion"/>.</returns>
        public static System.Numerics.Quaternion ToNumericsQuaternion(this UnityEngine.Quaternion q)
        {
            return new System.Numerics.Quaternion(-q.x, -q.y, q.z, q.w);
        }
    }
}
