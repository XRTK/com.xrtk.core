// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using XRTK.Definitions.CameraSystem;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services;
using XRTK.Services.CameraSystem;
using XRTK.Utilities;

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

            eyeTextureResolution = profile.EyeTextureResolution;
            isCameraPersistent = profile.IsCameraPersistent;
            cameraRigType = profile.CameraRigType.Type;
            applyQualitySettings = profile.ApplyQualitySettings;

            trackingOriginMode = profile.TrackingOriginMode;
            defaultHeadHeight = profile.DefaultHeadHeight;

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

        private readonly float eyeTextureResolution;
        private readonly bool isCameraPersistent;
        private readonly Type cameraRigType;
        private readonly bool applyQualitySettings;

        private readonly TrackingOriginModeFlags trackingOriginMode;
        private readonly float defaultHeadHeight;

        private readonly float nearClipPlaneOpaqueDisplay;
        private readonly CameraClearFlags cameraClearFlagsOpaqueDisplay;
        private readonly Color backgroundColorOpaqueDisplay;
        private readonly int opaqueQualityLevel;

        private readonly float nearClipPlaneTransparentDisplay;
        private readonly CameraClearFlags cameraClearFlagsTransparentDisplay;
        private readonly Color backgroundColorTransparentDisplay;
        private readonly int transparentQualityLevel;

        private readonly double bodyAdjustmentAngle;
        private readonly float bodyAdjustmentSpeed;

        private bool cameraOpaqueLastFrame;

        /// <inheritdoc />
        public virtual bool IsOpaque => XRDeviceUtilities.IsDisplayOpaque;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => CameraRig.PlayerCamera.stereoEnabled;

        /// <inheritdoc />
        public IMixedRealityCameraRig CameraRig { get; private set; }

        /// <inheritdoc />
        public virtual float HeadHeight => CameraRig.CameraTransform.localPosition.y;

        #region IMixedRealitySerivce Implementation

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            cameraSystem.RegisterCameraDataProvider(this);

            EnsureCameraRigSetup();

            if (!Application.isPlaying)
            {
                return;
            }

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

            XRSettings.eyeTextureResolutionScale = eyeTextureResolution;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            EnsureCameraRigSetup();

            if (Application.isPlaying &&
                isCameraPersistent)
            {
                CameraRig.PlayerCamera.transform.root.DontDestroyOnLoad();
            }
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

            if (CameraRig is Component component and IMixedRealityCameraRig)
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

        private void EnsureCameraRigSetup()
        {
            if (CameraRig == null)
            {
                var xrCamera = CameraCache.Main;

                if (xrCamera.transform.parent.IsNull())
                {
                    var rigTransform = new GameObject(DefaultCameraRig.Default_XRRigName).transform;
                    var trackingSpace = new GameObject(DefaultCameraRig.Default_CameraOffsetName);
                    trackingSpace.transform.SetParent(rigTransform);

                    var cameraPose = new Pose();
                    cameraPose.position = xrCamera.transform.position;
                    cameraPose.position.y = 0.0f;
                    cameraPose.rotation = xrCamera.transform.rotation;
                    xrCamera.transform.SetParent(trackingSpace.transform);

                    xrCamera.nearClipPlane = 0.01f;
                    xrCamera.transform.localPosition = Vector3.zero;
                    xrCamera.transform.localRotation = Quaternion.identity;

                    rigTransform.position = cameraPose.position;
                    rigTransform.rotation = cameraPose.rotation;

                    var trackedPoseDriver = xrCamera.EnsureComponent<TrackedPoseDriver>();
                    trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Center);
                    trackedPoseDriver.UseRelativeTransform = false;

                    var cameraOffset = rigTransform.EnsureComponent<CameraOffset>();
                    cameraOffset.cameraYOffset = defaultHeadHeight;
                    cameraOffset.TrackingOriginMode = trackingOriginMode;
                    cameraOffset.cameraFloorOffsetObject = trackingSpace;

                    var playerBody = new GameObject(DefaultCameraRig.Default_BodyName).transform;
                    playerBody.transform.SetParent(trackingSpace.transform);

                    CameraRig = rigTransform.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;

                    if (CameraRig is DefaultCameraRig cameraRig)
                    {
                        cameraRig.PlayerCamera = xrCamera;
                        cameraRig.RigTransform = rigTransform;
                        cameraRig.TrackingSpace = trackingSpace.transform;
                        cameraRig.TrackedPoseDriver = trackedPoseDriver;
                        cameraRig.CameraOffset = cameraOffset;
                        cameraRig.BodyTransform = playerBody;
                    }
                }
                else
                {
                    CameraRig = xrCamera.transform.root.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;
                }

                Debug.Assert(CameraRig != null);
            }
        }

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
        /// Called each <see cref="LateUpdate"/> to sync the
        /// <see cref="IMixedRealityCameraRig.RigTransform"/>,
        /// <see cref="IMixedRealityCameraRig.CameraTransform"/>, and
        /// <see cref="IMixedRealityCameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void SyncRigTransforms()
        {
            var cameraLocalPosition = CameraRig.CameraTransform.localPosition;
            var bodyLocalPosition = CameraRig.BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;
            bodyLocalPosition.y = cameraLocalPosition.y - Math.Abs(HeadHeight);
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
