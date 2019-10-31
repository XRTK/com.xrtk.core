// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces;
using XRTK.Utilities;

namespace XRTK.Services.CameraSystem
{
    /// <summary>
    /// The default <see cref="IMixedRealityCameraRig"/> for the XRTK.
    /// </summary>
    [ExecuteAlways]
    public class DefaultCameraRig : MonoBehaviour, IMixedRealityCameraRig
    {
        #region IMixedRealityCameraRig Implementation

        [SerializeField]
        private string playspaceName = "MixedRealityPlayspace";

        [SerializeField]
        private Transform playspaceTransform = null;

        /// <inheritdoc />
        public Transform PlayspaceTransform
        {
            get
            {
                if (playspaceTransform)
                {
                    return playspaceTransform;
                }

                if (PlayerCamera.transform.parent == null)
                {
                    playspaceTransform = new GameObject(playspaceName).transform;
                    PlayerCamera.transform.SetParent(playspaceTransform);
                }
                else
                {
                    if (PlayerCamera.transform.parent.name != playspaceName)
                    {
                        // Since the scene is set up with a different camera parent, its likely
                        // that there's an expectation that that parent is going to be used for
                        // something else. We print a warning to call out the fact that we're
                        // co-opting this object for use with teleporting and such, since that
                        // might cause conflicts with the parent's intended purpose.
                        Debug.LogWarning($"The Mixed Reality Toolkit expected the camera\'s parent to be named {playspaceName}. The existing parent will be renamed and used instead.");
                        // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        PlayerCamera.transform.parent.name = playspaceName;
                    }

                    playspaceTransform = PlayerCamera.transform.parent;
                }

                // It's very important that the MixedRealityPlayspace align with the tracked space,
                // otherwise world-locked things like playspace boundaries won't be aligned properly.
                // For now, we'll just assume that when the playspace is first initialized, the
                // tracked space origin overlaps with the world space origin. If a platform ever does
                // something else (i.e, placing the lower left hand corner of the tracked space at world
                // space 0,0,0), we should compensate for that here.
                return playspaceTransform;
            }
        }

        /// <inheritdoc />
        public Transform CameraTransform => PlayerCamera.transform;

        [SerializeField]
        private Camera playerCamera = null;

        /// <inheritdoc />
        public Camera PlayerCamera
        {
            get
            {
                // Currently the XRTK only supports a single player/user
                // So for now we will always reference the tagged MainCamera.
                if (playerCamera == null)
                {
                    playerCamera = CameraCache.Main;
                }

                return playerCamera;
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
                if (bodyTransform == null)
                {
                    bodyTransform = PlayspaceTransform.Find(playerBodyName);
                }

                if (bodyTransform == null)
                {
                    bodyTransform = new GameObject(playerBodyName).transform;
                    bodyTransform.transform.SetParent(PlayspaceTransform);
                }

                return bodyTransform;
            }
        }

        #endregion IMixedRealityCameraRig Implementation

        #region MonoBehaviour Implementation

        private void OnValidate()
        {
            if (playspaceTransform != null &&
                !playspaceTransform.name.Equals(playspaceName))
            {
                playspaceTransform.name = playspaceName;
            }

            if (bodyTransform != null &&
                !bodyTransform.name.Equals(playerBodyName))
            {
                bodyTransform.name = playerBodyName;
            }
        }

        private void OnDestroy()
        {
            if (bodyTransform != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(bodyTransform.gameObject);
                }
                else
                {
                    DestroyImmediate(bodyTransform.gameObject);
                }
            }
        }

        #endregion MonoBehaviour Implementation
    }
}