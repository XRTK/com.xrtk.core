// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services.CameraSystem;

namespace XRTK.Definitions.CameraSystem
{
    /// <summary>
    /// Provides configuration options for <see cref="IMixedRealityCameraDataProvider"/>s.
    /// </summary>
    public class BaseMixedRealityCameraDataProviderProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking.")]
        private TrackingOriginModeFlags trackingOriginMode = TrackingOriginModeFlags.Unknown;

        /// <summary>
        /// Gets or sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking. Not all devices support all tracking spaces; if the selected tracking space is not set it will fall back to Stationary.
        /// </summary>
        public TrackingOriginModeFlags TrackingOriginMode => trackingOriginMode;

        [SerializeField]
        private bool isCameraPersistent = true;

        /// <summary>
        /// Should the camera be reused in each scene?
        /// If so, then the camera's root will be flagged so it is not destroyed when the scene is unloaded.
        /// </summary>
        public bool IsCameraPersistent => isCameraPersistent;

        [Min(0.0001f)]
        [SerializeField]
        [Tooltip("The near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        /// <summary>
        /// The near clipping plane distance for an opaque display.
        /// </summary>
        public float NearClipPlaneOpaqueDisplay
        {
            get => nearClipPlaneOpaqueDisplay;
            internal set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be greater than zero!");
                }

                nearClipPlaneOpaqueDisplay = value;
            }
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

        [Min(0.0001f)]
        [SerializeField]
        [Tooltip("The near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        /// <summary>
        /// The near clipping plane distance for a transparent display.
        /// </summary>
        public float NearClipPlaneTransparentDisplay
        {
            get => nearClipPlaneTransparentDisplay;
            internal set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be greater than zero!");
                }

                nearClipPlaneTransparentDisplay = value;
            }
        }

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

        [Range(0f, 3f)]
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

        [SerializeField]
        [Tooltip("Set, if you want XRTK to apply quality settings for the camera.")]
        private bool applyQualitySettings = true;

        /// <summary>
        /// If set, XRTK will update the quality settings for the camera as configured in the profile.
        /// </summary>
        public bool ApplyQualitySettings => applyQualitySettings;
    }
}
