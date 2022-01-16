// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.CameraSystem;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services;
using XRTK.Utilities;

#if !XRTK_USE_LEGACYVR
using UnityEngine.XR;
using System.Collections.Generic;
#endif

namespace XRTK.Providers.CameraSystem
{
    /// <summary>
    /// Base class for all <see cref="IMixedRealityCameraDataProvider"/>s can inherit from.
    /// </summary>
    [System.Runtime.InteropServices.Guid("EA4C0C19-E533-4AE8-91A2-6998CB8905BB")]
    public class BaseCameraDataProvider : BaseDataProvider, IMixedRealityCameraDataProvider
    {
        /// <inheritdoc />
        public BaseCameraDataProvider(string name, uint priority, BaseMixedRealityCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {
            cameraSystem = parentService;

            if (profile.IsNull())
            {
                profile = MixedRealityToolkit.TryGetSystemProfile<IMixedRealityCameraSystem, MixedRealityCameraSystemProfile>(out var cameraSystemProfile)
                    ? cameraSystemProfile.GlobalCameraProfile
                    : throw new ArgumentException($"Unable to get a valid {nameof(MixedRealityCameraSystemProfile)}!");
            }

            if (profile.CameraRigType?.Type == null)
            {
                throw new Exception($"{nameof(profile.CameraRigType)} cannot be null!");
            }

            isCameraPersistent = profile.IsCameraPersistent;
            cameraRigType = profile.CameraRigType.Type;
            applyQualitySettings = profile.ApplyQualitySettings;

#if XRTK_USE_LEGACYVR
            DefaultHeadHeight = profile.DefaultHeadHeight;
#else
            trackingOriginMode = profile.TrackingOriginMode;
            defaultHeadHeight = profile.DefaultHeadHeight > 0f ? profile.DefaultHeadHeight : 1.6f;
#endif

            nearClipPlaneOpaqueDisplay = profile.NearClipPlaneOpaqueDisplay;
            cameraClearFlagsOpaqueDisplay = profile.CameraClearFlagsOpaqueDisplay;
            backgroundColorOpaqueDisplay = profile.BackgroundColorOpaqueDisplay;
            opaqueQualityLevel = profile.OpaqueQualityLevel;

            nearClipPlaneTransparentDisplay = profile.NearClipPlaneTransparentDisplay;
            cameraClearFlagsTransparentDisplay = profile.CameraClearFlagsTransparentDisplay;
            backgroundColorTransparentDisplay = profile.BackgroundColorTransparentDisplay;
            transparentQualityLevel = profile.TransparentQualityLevel;

            bodyAdjustmentAngle = profile.BodyAdjustmentAngle;
            bodyAdjustmentSpeed = profile.BodyAdjustmentSpeed;
        }

        private readonly IMixedRealityCameraSystem cameraSystem;
        private readonly bool isCameraPersistent;
        private readonly Type cameraRigType;
        private readonly bool applyQualitySettings;
        private readonly float nearClipPlaneTransparentDisplay;
        private readonly CameraClearFlags cameraClearFlagsTransparentDisplay;
        private readonly Color backgroundColorTransparentDisplay;
        private readonly int transparentQualityLevel;
        private readonly float nearClipPlaneOpaqueDisplay;
        private readonly CameraClearFlags cameraClearFlagsOpaqueDisplay;
        private readonly Color backgroundColorOpaqueDisplay;
        private readonly int opaqueQualityLevel;
        private readonly float bodyAdjustmentSpeed;
        private readonly double bodyAdjustmentAngle;
        private bool cameraOpaqueLastFrame;

#if XRTK_USE_LEGACYVR
        /// <summary>
        /// The fallback value if the <see cref="DefaultHeadHeight"/> is zero.
        /// </summary>
        private const float BodyHeightFallback = 1.6f;
#else
        private static List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        private TrackingOriginModeFlags trackingOriginMode;
        private readonly float defaultHeadHeight;
        private bool trackingOriginInitialized = false;
        private bool trackingOriginInitializing = false;
#endif

        /// <inheritdoc />
        public virtual bool IsOpaque => XRDeviceUtilities.IsDisplayOpaque;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => CameraRig.PlayerCamera.stereoEnabled;

#if XRTK_USE_LEGACYVR
        /// <inheritdoc />
        public virtual bool HeadHeightIsManagedByDevice => XRDeviceUtilities.IsDevicePresent;
#endif

        /// <inheritdoc />
        public IMixedRealityCameraRig CameraRig { get; private set; }

#if XRTK_USE_LEGACYVR
        /// <inheritdoc />
        public float DefaultHeadHeight { get; }
#endif


#if XRTK_USE_LEGACYVR
        private float headHeight;
#endif

        /// <inheritdoc />
        public virtual float HeadHeight
        {
#if XRTK_USE_LEGACYVR
            get => headHeight;
            set
            {
                if (value.Equals(headHeight))
                {
                    return;
                }

                headHeight = value;
                CameraRig.CameraPoseDriver.originPose = new Pose(new Vector3(0f, headHeight, 0f), Quaternion.identity);
            }
#else
            get => CameraRig.CameraTransform.localPosition.y;
#endif
        }

        #region IMixedRealitySerivce Implementation

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (CameraRig == null)
            {
                if (CameraCache.Main.transform.parent.IsNull())
                {
                    var rigTransform = new GameObject().transform;
                    CameraCache.Main.transform.SetParent(rigTransform);
                }

                CameraRig = CameraCache.Main.transform.parent.gameObject.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;
                Debug.Assert(CameraRig != null);
            }

#if XRTK_USE_LEGACYVR
            ApplySettingsForDefaultHeadHeight();
#else
            // We attempt to intialize the camera tracking origin, which might
            // fail at this point if the subystems are not ready, in which case,
            // we set a flag to keep trying.
            trackingOriginInitialized = SetupTrackingOrigin();
            trackingOriginInitializing = !trackingOriginInitialized;
#endif

            cameraOpaqueLastFrame = IsOpaque;

            if (applyQualitySettings)
            {
                if (IsOpaque)
                {
                    ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ApplySettingsForTransparentDisplay();
                }
            }

            cameraSystem.RegisterCameraDataProvider(this);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (Application.isPlaying &&
                isCameraPersistent)
            {
                CameraRig.PlayerCamera.transform.root.DontDestroyOnLoad();
            }

#if XRTK_USE_LEGACYVR
            ApplySettingsForDefaultHeadHeight();
#endif
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying)
            {
                return;
            }

            if (cameraOpaqueLastFrame != IsOpaque)
            {
                cameraOpaqueLastFrame = IsOpaque;

#if XRTK_USE_LEGACYVR
                ApplySettingsForDefaultHeadHeight();
#endif

                if (applyQualitySettings)
                {
                    if (IsOpaque)
                    {
                        ApplySettingsForOpaqueDisplay();
                    }
                    else
                    {
                        ApplySettingsForTransparentDisplay();
                    }
                }
            }

#if !XRTK_USE_LEGACYVR
            // We keep trying to intiailze the tracking origin,
            // until it worked, because at application launch the
            // subystems might not be ready yet.
            if (trackingOriginInitializing && !trackingOriginInitialized)
            {
                trackingOriginInitialized = SetupTrackingOrigin();
                trackingOriginInitializing = !trackingOriginInitialized;
            }
#endif
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            if (!Application.isPlaying) { return; }

            SyncRigTransforms();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (CameraRig == null ||
                CameraRig.GameObject.IsNull())
            {
                return;
            }

            ResetRigTransforms();

            if (!CameraRig.PlayerCamera.IsNull() &&
                !CameraRig.PlayerCamera.transform.IsNull())
            {
                var cameraTransform = CameraRig.PlayerCamera.transform;
                cameraTransform.SetParent(null);
                cameraTransform.position = Vector3.one;
                cameraTransform.rotation = Quaternion.identity;
            }

            if (CameraRig.RigTransform != null)
            {
                CameraRig.RigTransform.gameObject.Destroy();
            }

            if (CameraRig is Component component &&
                component is IMixedRealityCameraRig)
            {
                component.Destroy();
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            cameraSystem.UnRegisterCameraDataProvider(this);
        }

        #endregion IMixedRealitySerivce Implementation

#if !XRTK_USE_LEGACYVR
        #region Tracking Origin Setup

        private bool SetupTrackingOrigin()
        {
            SubsystemManager.GetInstances(inputSubsystems);

            // We assume the tracking mode to be set, that way
            // when in editor and no subsystems are connected / running
            // we can still keep going and assume everything is ready.
            var trackingOriginModeSet = true;

            if (inputSubsystems.Count != 0)
            {
                for (int i = 0; i < inputSubsystems.Count; i++)
                {
                    var result = SetupTrackingOrigin(inputSubsystems[i]);
                    if (result)
                    {
                        inputSubsystems[i].trackingOriginUpdated -= XRInputSubsystem_OnTrackingOriginUpdated;
                        inputSubsystems[i].trackingOriginUpdated += XRInputSubsystem_OnTrackingOriginUpdated;
                    }

                    trackingOriginModeSet &= result;
                }
            }
            else
            {
                // No subsystems available, we are probably running in editor without a device
                // connected, position the camera at the configured default offset.
                UpdateCameraHeightOffset(defaultHeadHeight);
            }

            return trackingOriginModeSet;
        }

        private bool SetupTrackingOrigin(XRInputSubsystem subsystem)
        {
            if (subsystem == null)
            {
                return false;
            }

            var trackingOriginModeSet = false;
            var supportedModes = subsystem.GetSupportedTrackingOriginModes();
            var requestedMode = trackingOriginMode;

            if (requestedMode == TrackingOriginModeFlags.Floor)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Floor | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the tracking origin to floor, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode);
                }
            }
            else if (requestedMode == TrackingOriginModeFlags.Device)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Device | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the camera system tracking origin to device, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode) && subsystem.TryRecenter();
                }
            }

            if (trackingOriginModeSet)
            {
                UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());
            }

            return trackingOriginModeSet;
        }

        private void XRInputSubsystem_OnTrackingOriginUpdated(XRInputSubsystem subsystem) => UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());

        private void UpdateTrackingOrigin(TrackingOriginModeFlags trackingOriginModeFlags)
        {
            trackingOriginMode = trackingOriginModeFlags;

            UpdateCameraHeightOffset(trackingOriginMode == TrackingOriginModeFlags.Device ? defaultHeadHeight : 0.0f);
            ResetRigTransforms();
            SyncRigTransforms();
        }

        #endregion Tracking Origin Setup
#endif

#if XRTK_USE_LEGACYVR
        /// <summary>
        /// Depending on whether there is an XR device connected,
        /// moves the camera to the setting from the camera profile.
        /// </summary>
        protected virtual void ApplySettingsForDefaultHeadHeight()
        {
            // We need to check whether the application is playing or not here.
            // Since this code is executed even when not in play mode, we want
            // to definitely apply the head height configured in the editor, when
            // not in play mode. It helps with working in the editor and visualizing
            // the user's perspective. When running though, we need to make sure we do
            // not interfere with any platform provided head pose tracking.
            if (!Application.isPlaying || !HeadHeightIsManagedByDevice)
            {
                HeadHeight = DefaultHeadHeight;
            }
            // If we are running and the device/platform provides the head pose,
            // we need to make sure to reset any applied head height while in edit mode.
            else if (Application.isPlaying && HeadHeightIsManagedByDevice)
            {
                HeadHeight = 0f;
            }

            ResetRigTransforms();
            SyncRigTransforms();
        }
#endif

#if !XRTK_USE_LEGACYVR
        /// <summary>
        /// Updates the camera height offset to the specified value.
        /// </summary>
        protected virtual void UpdateCameraHeightOffset(float heightOffset = 0f)
        {
            CameraRig.CameraTransform.localPosition = new Vector3(
                CameraRig.CameraTransform.localPosition.x,
                heightOffset,
                CameraRig.CameraTransform.localPosition.z);
        }
#endif

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForOpaqueDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(opaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForTransparentDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorTransparentDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(transparentQualityLevel, false);
        }

        /// <summary>
        /// Resets the <see cref="IMixedRealityCameraRig.RigTransform"/>, <see cref="IMixedRealityCameraRig.CameraTransform"/>,
        /// and <see cref="IMixedRealityCameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void ResetRigTransforms()
        {
            CameraRig.RigTransform.position = Vector3.zero;
            CameraRig.RigTransform.rotation = Quaternion.identity;

            // If the camera is a 2d camera then we can adjust the camera's height to match the head height.
            CameraRig.CameraTransform.position = IsStereoscopic ? Vector3.zero : new Vector3(0f, HeadHeight, 0f);

            CameraRig.CameraTransform.rotation = Quaternion.identity;
            CameraRig.BodyTransform.position = Vector3.zero;
            CameraRig.BodyTransform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Called each <see cref="LateUpdate"/> to sync the <see cref="IMixedRealityCameraRig.RigTransform"/>,
        /// <see cref="IMixedRealityCameraRig.CameraTransform"/>, and <see cref="IMixedRealityCameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void SyncRigTransforms()
        {
            var cameraLocalPosition = CameraRig.CameraTransform.localPosition;
            var bodyLocalPosition = CameraRig.BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;

#if XRTK_USE_LEGACYVR
            bodyLocalPosition.y = HeadHeight > 0f
                ? cameraLocalPosition.y - HeadHeight
                : cameraLocalPosition.y - BodyHeightFallback;
#else
            bodyLocalPosition.y = cameraLocalPosition.y - Math.Abs(HeadHeight);
#endif

            bodyLocalPosition.z = cameraLocalPosition.z;

            CameraRig.BodyTransform.localPosition = bodyLocalPosition;

            var bodyRotation = CameraRig.BodyTransform.rotation;
            var headRotation = CameraRig.CameraTransform.rotation;
            var currentAngle = Mathf.Abs(Quaternion.Angle(bodyRotation, headRotation));

            if (currentAngle > bodyAdjustmentAngle)
            {
                CameraRig.BodyTransform.rotation = Quaternion.Slerp(bodyRotation, headRotation, Time.deltaTime * bodyAdjustmentSpeed);
            }
        }
    }
}
