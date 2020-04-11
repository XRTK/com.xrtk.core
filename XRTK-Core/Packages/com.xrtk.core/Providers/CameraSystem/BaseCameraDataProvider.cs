// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.CameraSystem;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services;
using XRTK.Utilities;

namespace XRTK.Providers.CameraSystem
{
    /// <summary>
    /// Base class for all <see cref="IMixedRealityCameraDataProvider"/>s can inherit from.
    /// </summary>
    public class BaseCameraDataProvider : BaseDataProvider, IMixedRealityCameraDataProvider
    {
        /// <inheritdoc />
        public BaseCameraDataProvider(string name, uint priority, BaseMixedRealityCameraDataProviderProfile profile)
            : base(name, priority)
        {
            if (profile == null)
            {
                throw new ArgumentException($"Missing the profile for {base.Name}!");
            }

            if (profile.CameraRigType?.Type == null)
            {
                throw new Exception($"{nameof(profile.CameraRigType)} cannot be null!");
            }

            var globalProfile = MixedRealityToolkit.Instance.ActiveProfile.CameraSystemProfile;

            isCameraPersistent = profile.IsCameraPersistent != globalProfile.IsCameraPersistent
                ? profile.IsCameraPersistent
                : globalProfile.IsCameraPersistent;
            cameraRigType = profile.CameraRigType.Type != globalProfile.CameraRigType.Type
                ? profile.CameraRigType.Type
                : globalProfile.CameraRigType.Type;
            DefaultHeadHeight = !profile.DefaultHeadHeight.Approximately(globalProfile.DefaultHeadHeight, 0.01f)
                ? profile.DefaultHeadHeight
                : globalProfile.DefaultHeadHeight;

            nearClipPlaneOpaqueDisplay = !profile.NearClipPlaneOpaqueDisplay.Approximately(globalProfile.NearClipPlaneOpaqueDisplay, 0.01f)
                ? profile.NearClipPlaneOpaqueDisplay
                : globalProfile.NearClipPlaneOpaqueDisplay;
            cameraClearFlagsOpaqueDisplay = profile.CameraClearFlagsOpaqueDisplay != globalProfile.CameraClearFlagsOpaqueDisplay
                ? profile.CameraClearFlagsOpaqueDisplay
                : globalProfile.CameraClearFlagsOpaqueDisplay;
            backgroundColorOpaqueDisplay = profile.BackgroundColorOpaqueDisplay != globalProfile.BackgroundColorOpaqueDisplay
                ? profile.BackgroundColorOpaqueDisplay
                : globalProfile.BackgroundColorOpaqueDisplay;
            opaqueQualityLevel = profile.OpaqueQualityLevel != globalProfile.OpaqueQualityLevel
                ? profile.OpaqueQualityLevel
                : globalProfile.OpaqueQualityLevel;

            nearClipPlaneTransparentDisplay = !profile.NearClipPlaneTransparentDisplay.Approximately(globalProfile.NearClipPlaneTransparentDisplay, 0.01f)
                ? profile.NearClipPlaneTransparentDisplay
                : globalProfile.NearClipPlaneTransparentDisplay;
            cameraClearFlagsTransparentDisplay = profile.CameraClearFlagsTransparentDisplay != globalProfile.CameraClearFlagsTransparentDisplay
                ? profile.CameraClearFlagsTransparentDisplay
                : globalProfile.CameraClearFlagsTransparentDisplay;
            backgroundColorTransparentDisplay = profile.BackgroundColorTransparentDisplay != globalProfile.BackgroundColorTransparentDisplay
                ? profile.BackgroundColorTransparentDisplay
                : globalProfile.BackgroundColorTransparentDisplay;
            transparentQualityLevel = profile.TransparentQualityLevel != globalProfile.TransparentQualityLevel
                ? profile.TransparentQualityLevel
                : globalProfile.TransparentQualityLevel;

            bodyAdjustmentAngle = !profile.BodyAdjustmentAngle.Approximately(globalProfile.BodyAdjustmentAngle, 0.01f)
                ? profile.BodyAdjustmentAngle
                : globalProfile.BodyAdjustmentAngle;
            bodyAdjustmentSpeed = !profile.BodyAdjustmentSpeed.Approximately(globalProfile.BodyAdjustmentSpeed, 0.01f)
                ? profile.BodyAdjustmentSpeed
                : globalProfile.BodyAdjustmentSpeed;
        }

        private readonly Type cameraRigType;

        private readonly bool isCameraPersistent;

        private readonly int opaqueQualityLevel;
        private readonly int transparentQualityLevel;

        private readonly float nearClipPlaneOpaqueDisplay;
        private readonly float nearClipPlaneTransparentDisplay;

        private readonly Color backgroundColorOpaqueDisplay;
        private readonly Color backgroundColorTransparentDisplay;

        private readonly CameraClearFlags cameraClearFlagsOpaqueDisplay;
        private readonly CameraClearFlags cameraClearFlagsTransparentDisplay;

        private readonly float bodyAdjustmentSpeed;
        private readonly double bodyAdjustmentAngle;

        private bool cameraOpaqueLastFrame;
        private DisplayType currentDisplayType;

        private enum DisplayType
        {
            Opaque = 0,
            Transparent
        }

        /// <inheritdoc />
        public virtual bool IsOpaque
        {
            get
            {
                currentDisplayType = DisplayType.Opaque;
#if UNITY_WSA
                if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
                {
                    currentDisplayType = DisplayType.Transparent;
                }
#elif PLATFORM_LUMIN
                currentDisplayType = DisplayType.Transparent;
#endif
                return currentDisplayType == DisplayType.Opaque;
            }
        }

        /// <inheritdoc />
        public virtual bool IsStereoscopic => UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRDevice.isPresent;

        /// <inheritdoc />
        public IMixedRealityCameraRig CameraRig { get; private set; }

        /// <inheritdoc />
        public float DefaultHeadHeight { get; }

        private float headHeight;

        /// <inheritdoc />
        public float HeadHeight
        {
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
        }

        #region IMixedRealitySerivce Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            cameraOpaqueLastFrame = IsOpaque;

            if (IsOpaque)
            {
                ApplySettingsForOpaqueDisplay();
            }
            else
            {
                ApplySettingsForTransparentDisplay();
            }

            if (CameraRig == null)
            {
                // TODO Currently we get always get the main camera. Should we provide a tag to search for alts?
                CameraRig = CameraCache.Main.gameObject.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;
                Debug.Assert(CameraRig != null);
                ResetRigTransforms();
            }

            ApplySettingsForDefaultHeadHeight();

            MixedRealityToolkit.CameraSystem.RegisterCameraDataProvider(this);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            ResetRigTransforms();

            if (Application.isPlaying &&
                isCameraPersistent)
            {
                CameraRig.PlayerCamera.transform.root.DontDestroyOnLoad();
            }

            ApplySettingsForDefaultHeadHeight();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (IsOpaque != cameraOpaqueLastFrame)
            {
                cameraOpaqueLastFrame = IsOpaque;

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

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            SyncRigTransforms();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (CameraRig == null) { return; }

            if (CameraRig.PlayerCamera != null &&
                CameraRig.PlayerCamera.transform != null)
            {
                CameraRig.PlayerCamera.transform.SetParent(null);
            }

            if (CameraRig.PlayspaceTransform != null)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(CameraRig.PlayspaceTransform.gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(CameraRig.PlayspaceTransform.gameObject);
                }
            }

            if (CameraRig is Component component &&
                component is IMixedRealityCameraRig)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(component);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(component);
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            MixedRealityToolkit.CameraSystem.UnRegisterCameraDataProvider(this);
        }

        #endregion IMixedRealitySerivce Implementation

        /// <summary>
        /// Depending on whether there is an XR device connected,
        /// moves the camera to the setting from the camera profile.
        /// </summary>
        private void ApplySettingsForDefaultHeadHeight()
        {
            headHeight = DefaultHeadHeight;
            ResetRigTransforms();
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        private void ApplySettingsForOpaqueDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(opaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        private void ApplySettingsForTransparentDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorTransparentDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(transparentQualityLevel, false);
        }

        private void ResetRigTransforms()
        {
            CameraRig.PlayspaceTransform.position = Vector3.zero;
            CameraRig.PlayspaceTransform.rotation = Quaternion.identity;
            CameraRig.CameraTransform.position = Vector3.zero;
            CameraRig.CameraTransform.rotation = Quaternion.identity;
            CameraRig.BodyTransform.position = Vector3.zero;
            CameraRig.BodyTransform.rotation = Quaternion.identity;
        }

        private void SyncRigTransforms()
        {
            var cameraPosition = CameraRig.CameraTransform.localPosition;
            var bodyLocalPosition = CameraRig.BodyTransform.localPosition;

            bodyLocalPosition.x = cameraPosition.x;
            bodyLocalPosition.y = cameraPosition.y - HeadHeight;
            bodyLocalPosition.z = cameraPosition.z;

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
