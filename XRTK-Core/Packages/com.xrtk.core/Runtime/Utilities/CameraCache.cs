// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Services;

namespace XRTK.Utilities
{
    /// <summary>
    /// The purpose of this class is to provide a cached reference to the main camera. Calling Camera.main
    /// executes a FindByTag on the scene, which will get worse and worse with more tagged objects.
    /// </summary>
    public static class CameraCache
    {
        private static Camera cachedCamera;

        /// <summary>
        /// Returns a cached reference to the main camera and uses Camera.main if it hasn't been cached yet.
        /// </summary>
        public static Camera Main
        {
            get
            {
                var mainCamera = Camera.main;

                if (mainCamera == null)
                {
                    if (MixedRealityToolkit.IsApplicationQuitting)
                    {
                        return null;
                    }

                    mainCamera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();
                }

                mainCamera = cachedCamera == null ? Refresh(mainCamera) : cachedCamera;

                return mainCamera;
            }
        }

        /// <summary>
        /// Set the cached camera to a new reference and return it
        /// </summary>
        /// <param name="newMain">New main camera to cache</param>
        public static Camera Refresh(Camera newMain)
        {
            if (newMain == null)
            {
                Debug.LogWarning("A null camera was passed into CameraCache.Refresh()");
            }

            return cachedCamera = newMain;
        }
    }
}
