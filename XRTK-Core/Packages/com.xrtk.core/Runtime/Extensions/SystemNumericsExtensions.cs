// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Extensions
{
    public static class SystemNumericsExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Numerics.Vector3"/> to a Unity engine <see cref="Vector3"/>.
        /// </summary>
        /// <param name="v">Vector value to convert.</param>
        /// <returns>Unity vector.</returns>
        public static Vector3 ToUnity(this System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Y, -v.Z);
        }

        /// <summary>
        /// Converts a <see cref="System.Numerics.Quaternion"/> to a Unity engine <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="q">Quaternion value to convert.</param>
        /// <returns>Unity quaternion.</returns>
        public static Quaternion ToUnity(this System.Numerics.Quaternion q)
        {
            return new Quaternion(-q.X, -q.Y, q.Z, q.W);
        }
    }
}