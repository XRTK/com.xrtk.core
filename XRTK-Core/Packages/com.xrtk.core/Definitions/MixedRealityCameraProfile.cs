// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Services.CameraSystem;

namespace XRTK.Definitions
{
    /// <summary>
    /// This <see cref="ScriptableObject"/> tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Camera Profile", fileName = "MixedRealityCameraProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private bool isCameraPersistent = false;

        /// <summary>
        /// Should the camera be reused in each scene?
        /// If so, then the camera's root will be flagged so it is not destroyed when the scene is unloaded.
        /// </summary>
        public bool IsCameraPersistent => isCameraPersistent;

        [SerializeField]
        [Tooltip("The near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        /// <summary>
        /// The near clipping plane distance for an opaque display.
        /// </summary>
        public float NearClipPlaneOpaqueDisplay
        {
            get => nearClipPlaneOpaqueDisplay;
            internal set => nearClipPlaneOpaqueDisplay = value;
        }

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsOpaqueDisplay = CameraClearFlags.Skybox;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsOpaqueDisplay => cameraClearFlagsOpaqueDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorOpaqueDisplay = Color.black;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorOpaqueDisplay => backgroundColorOpaqueDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for opaque display.")]
        private int opaqueQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for opaque display.
        /// </summary>
        public int OpaqueQualityLevel => opaqueQualityLevel;

        [SerializeField]
        [Tooltip("The near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        /// <summary>
        /// The near clipping plane distance for a transparent display.
        /// </summary>
        public float NearClipPlaneTransparentDisplay => nearClipPlaneTransparentDisplay;

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsTransparentDisplay = CameraClearFlags.SolidColor;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsTransparentDisplay => cameraClearFlagsTransparentDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorTransparentDisplay = Color.clear;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorTransparentDisplay => backgroundColorTransparentDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for transparent display.")]
        private int transparentQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for transparent display.
        /// </summary>
        public int TransparentQualityLevel => transparentQualityLevel;

        [SerializeField]
        [Tooltip("The concrete type to use for the camera rig.")]
        [Implements(typeof(IMixedRealityCameraRig), TypeGrouping.ByNamespaceFlat)]
        private SystemType cameraRigType = new SystemType(typeof(DefaultCameraRig));

        /// <summary>
        /// The concrete type to use for the camera rig.
        /// </summary>
        public SystemType CameraRigType
        {
            get => cameraRigType;
            internal set => cameraRigType = value;
        }

        [SerializeField]
        [Tooltip("The default head height the rig will start at if a platform doesn't automatically adjust the height for you.")]
        private float defaultHeadHeight = 1.6f;

        /// <summary>
        /// The default head height the rig will start at if a platform doesn't automatically adjust the height for you.
        /// </summary>
        public float DefaultHeadHeight => defaultHeadHeight;

        [SerializeField]
        [Range(0f, 180f)]
        [Tooltip("This is the angle that will be used to adjust the player's body rotation in relation to their head position.")]
        private float bodyAdjustmentAngle = 60f;

        /// <summary>
        /// /// This is the angle that will be used to adjust the player's body rotation in relation to their head position.
        /// </summary>
        public float BodyAdjustmentAngle => bodyAdjustmentAngle;

        [SerializeField]
        [Tooltip("The speed at which the body transform will sync it's rotation with the head transform.")]
        private float bodyAdjustmentSpeed = 1f;

        /// <summary>
        /// The speed at which the body transform will sync it's rotation with the head transform.
        /// </summary>
        public float BodyAdjustmentSpeed => bodyAdjustmentSpeed;
    }
}