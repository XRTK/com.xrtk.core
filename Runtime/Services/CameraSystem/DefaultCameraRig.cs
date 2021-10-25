// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SpatialTracking;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Utilities;

namespace XRTK.Services.CameraSystem
{
    /// <summary>
    /// The default <see cref="IMixedRealityCameraRig"/> for the XRTK.
    /// </summary>
    [ExecuteAlways]
    [System.Runtime.InteropServices.Guid("8E0EE4FC-C8A5-4B10-9FCA-EE55B6D421FF")]
    public class DefaultCameraRig : MonoBehaviour, IMixedRealityCameraRig
    {
        #region IMixedRealityCameraRig Implementation

        [SerializeField]
        private string rigName = "XRCameraRig";

        [SerializeField]
        private Transform rigTransform = null;

        /// <inheritdoc />
        public GameObject GameObject
        {
            get
            {
                try
                {
                    return gameObject;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <inheritdoc />
        public Transform RigTransform
        {
            get
            {
                if (rigTransform != null)
                {
                    return rigTransform;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                var rigTransformLookup = GameObject.Find(rigName);

                rigTransform = rigTransformLookup.IsNull()
                    ? new GameObject(rigName).transform
                    : rigTransformLookup.transform;

                if (CameraTransform.parent != rigTransform)
                {
                    CameraTransform.SetParent(rigTransform);
                }

                if (BodyTransform.parent != rigTransform)
                {
                    BodyTransform.SetParent(rigTransform);
                }

                // It's very important that the rig transform aligns with the tracked space,
                // otherwise world-locked things like boundaries won't be aligned properly.
                // For now, we'll just assume that when the rig is first initialized, the
                // tracked space origin overlaps with the world space origin. If a platform ever does
                // something else (i.e, placing the lower left hand corner of the tracked space at world
                // space 0,0,0), we should compensate for that here.
                return rigTransform;
            }
        }

        /// <inheritdoc />
        public Transform CameraTransform => PlayerCamera == null ? null : playerCamera.transform;

        [SerializeField]
        private Camera playerCamera = null;

        /// <inheritdoc />
        public Camera PlayerCamera
        {
            get
            {
                if (playerCamera != null)
                {
                    return playerCamera;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                // Currently the XRTK only supports a single player/user
                // So for now we will always reference the tagged MainCamera.
                if (playerCamera == null)
                {
                    playerCamera = CameraCache.Main;
                }

                if (playerCamera.transform.parent == null)
                {
                    playerCamera.transform.SetParent(RigTransform);
                }
                else
                {
                    if (playerCamera.transform.parent.name != rigName)
                    {
                        // Since the scene is set up with a different camera parent, its likely
                        // that there's an expectation that that parent is going to be used for
                        // something else. We print a warning to call out the fact that we're
                        // co-opting this object for use with teleporting and such, since that
                        // might cause conflicts with the parent's intended purpose.
                        Debug.LogWarning($"The Mixed Reality Toolkit expected the camera\'s parent to be named {rigName}. The existing parent will be renamed and used instead.");
                        // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        playerCamera.transform.parent.name = rigName;
                    }

                    rigTransform = playerCamera.transform.parent;
                }

                Debug.Assert(CameraPoseDriver != null);

                return playerCamera;
            }
        }

        [SerializeField]
        private TrackedPoseDriver cameraPoseDriver = null;

        /// <inheritdoc />
        public TrackedPoseDriver CameraPoseDriver
        {
            get
            {
                if (cameraPoseDriver != null)
                {
                    return cameraPoseDriver;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                cameraPoseDriver = PlayerCamera.gameObject.EnsureComponent<TrackedPoseDriver>();
#if XRTK_USE_LEGACYVR
                cameraPoseDriver.UseRelativeTransform = true;
#else
                cameraPoseDriver.UseRelativeTransform = false;
#endif
                return cameraPoseDriver;
            }
        }

        [SerializeField]
        private string playerBodyName = "PlayerBody";

        [SerializeField]
        private Transform bodyTransform = null;

        /// <inheritdoc />
        public Transform BodyTransform
        {
            get
            {
                if (bodyTransform != null)
                {
                    return bodyTransform;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                if (bodyTransform == null)
                {
                    bodyTransform = RigTransform.Find(playerBodyName);
                }

                if (bodyTransform == null)
                {
                    bodyTransform = new GameObject(playerBodyName).transform;
                    bodyTransform.transform.SetParent(RigTransform);
                }

                return bodyTransform;
            }
        }

        #endregion IMixedRealityCameraRig Implementation

        #region MonoBehaviour Implementation

        private void OnValidate()
        {
            if (rigTransform != null &&
                !rigTransform.name.Equals(rigName))
            {
                rigTransform.name = rigName;
            }

            if (bodyTransform != null &&
                !bodyTransform.name.Equals(playerBodyName))
            {
                bodyTransform.name = playerBodyName;
            }
        }

        #endregion MonoBehaviour Implementation
    }
}
