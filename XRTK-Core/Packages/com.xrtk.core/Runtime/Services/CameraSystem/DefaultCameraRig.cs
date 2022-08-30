// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
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

        public const string Default_XRRigName = "XRRig";
        public const string Default_CameraOffsetName = "TrackingSpace";
        public const string Default_BodyName = "PlayerBody";

        [SerializeField]
        private string rigName = Default_XRRigName;

        [SerializeField]
        private Transform rigTransform;

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
                return rigTransformLookup.IsNull()
                    ? new GameObject(rigName).transform
                    : rigTransformLookup.transform;
            }
            internal set => rigTransform = value;
        }

        [SerializeField]
        private string trackingSpaceName = Default_CameraOffsetName;

        [SerializeField]
        private Transform trackingSpace;

        public Transform TrackingSpace
        {
            get
            {
                if (trackingSpace != null)
                {
                    return trackingSpace;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                trackingSpace = CameraOffset.cameraFloorOffsetObject.transform;

                return trackingSpace;
            }
            internal set => trackingSpace = value;
        }

        [SerializeField]
        private CameraOffset cameraOffset;

        internal CameraOffset CameraOffset
        {
            get
            {
                if (cameraOffset != null)
                {
                    return cameraOffset;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                cameraOffset = RigTransform.gameObject.EnsureComponent<CameraOffset>();
                cameraOffset.TrackingOriginMode = TrackingOriginModeFlags.Device;

                var cameraOffsetGo = GameObject.Find(trackingSpaceName);

                if (cameraOffsetGo == null)
                {
                    cameraOffsetGo = new GameObject(trackingSpaceName);
                }

                trackingSpace = cameraOffsetGo.transform;

                if (trackingSpace.parent != RigTransform)
                {
                    trackingSpace.SetParent(RigTransform);
                }

                cameraOffset.cameraFloorOffsetObject = cameraOffsetGo;

                return cameraOffset;
            }
            set => cameraOffset = value;
        }

        /// <inheritdoc />
        public Transform CameraTransform => PlayerCamera == null ? null : playerCamera == null ? null : playerCamera.transform;

        [SerializeField]
        private Camera playerCamera;

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

                // this should only really happen if we've manually
                // added the camera rig.
                if (playerCamera.transform.parent != TrackingSpace)
                {
                    var cameraPose = new Pose();
                    cameraPose.position = playerCamera.transform.position;
                    cameraPose.position.y = 0.0f;
                    cameraPose.rotation = playerCamera.transform.rotation;
                    playerCamera.transform.parent = TrackingSpace;

                    playerCamera.nearClipPlane = 0.01f;
                    playerCamera.transform.position = Vector3.zero;
                    playerCamera.transform.rotation = Quaternion.identity;

                    RigTransform.position = cameraPose.position;
                    RigTransform.rotation = cameraPose.rotation;
                }

                return playerCamera;
            }
            internal set => playerCamera = value;
        }

        [SerializeField]
        private TrackedPoseDriver trackedPoseDriver;

        /// <inheritdoc />
        public TrackedPoseDriver TrackedPoseDriver
        {
            get
            {
                if (trackedPoseDriver != null)
                {
                    return trackedPoseDriver;
                }

                if (MixedRealityToolkit.IsApplicationQuitting)
                {
                    return null;
                }

                trackedPoseDriver = PlayerCamera.gameObject.EnsureComponent<TrackedPoseDriver>();
                trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Center);
                trackedPoseDriver.UseRelativeTransform = false;

                return trackedPoseDriver;
            }
            internal set => trackedPoseDriver = value;
        }

        [SerializeField]
        private string playerBodyName = Default_BodyName;

        [SerializeField]
        private Transform bodyTransform;

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

                bodyTransform = RigTransform.Find(playerBodyName);

                if (bodyTransform == null)
                {
                    bodyTransform = new GameObject(playerBodyName).transform;
                }

                bodyTransform.SetParent(TrackingSpace);

                return bodyTransform;
            }
            internal set => bodyTransform = value;
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

            if (trackingSpace != null &&
                !trackingSpace.name.Equals(trackingSpaceName))
            {
                trackingSpace.name = trackingSpaceName;
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
