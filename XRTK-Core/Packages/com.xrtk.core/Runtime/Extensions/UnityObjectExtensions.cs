// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Object class
    /// </summary>
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Enable Unity objects to skip <see cref="Object.DontDestroyOnLoad"/> when editor isn't playing so test runner passes.
        /// </summary>
        /// <param name="object"></param>
        public static void DontDestroyOnLoad(this Object @object)
        {
#if UNITY_EDITOR
            // ReSharper disable once EnforceIfStatementBraces
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                Object.DontDestroyOnLoad(@object);
        }

        /// <summary>
        /// Destroys a Unity  <see cref="Object"/> appropriately depending if running in in edit or play mode.
        /// </summary>
        /// <param name="object">Unity  <see cref="Object"/> to destroy</param>
        /// <param name="t">Time in seconds at which to destroy the object, if applicable.</param>
        public static void Destroy(this Object @object, float t = 0.0f)
        {
            if (@object.IsNull()) { return; }

            if (Application.isPlaying)
            {
                Object.Destroy(@object, t);
            }
            else
            {
#if UNITY_EDITOR
                // Must use DestroyImmediate in edit mode but it is not allowed when called from
                // trigger/contact, animation event callbacks or OnValidate. Must use Destroy instead.
                // Delay call to counter this issue in editor.
                UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(@object);
#else
                Object.DestroyImmediate(@object);
#endif
            }
        }

        /// <summary>
        /// Checks if a Unity <see cref="Object"/> is null.
        /// </summary>
        /// <param name="object"></param>
        /// <remarks>Checks both the managed object and the underling Unity-managed native object.</remarks>
        /// <returns>True if null, otherwise false.</returns>
        public static bool IsNull(this Object @object)
        {
            try
            {
                return @object == null;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if a Unity <see cref="Object"/> is not null.
        /// </summary>
        /// <param name="object"></param>
        /// <remarks>Checks both the managed object and the underling Unity-managed native object.</remarks>
        /// <returns>True if not null, otherwise false.</returns>
        public static bool IsNotNull(this Object @object) => !@object.IsNull();
    }
}
