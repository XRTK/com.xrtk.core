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
        public BaseCameraDataProvider(string name, uint priority, BaseMixedRealityCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {
            cameraSystem = MixedRealityToolkit.CameraSystem;
            var globalProfile = MixedRealityToolkit.Instance.ActiveProfile.CameraSystemProfile.GlobalCameraProfile;

            if (profile != null)
            {
                if (profile.CameraRigType?.Type == null)
                {
                    throw new Exception($"{nameof(profile.CameraRigType)} cannot be null!");
                }
            }
            else
            {
                if (globalProfile.CameraRigType?.Type == null)
                {
                    throw new Exception($"{nameof(globalProfile.CameraRigType)} cannot be null!");
                }
            }

            isCameraPersistent = profile != null
                ? profile.IsCameraPersistent
                : globalProfile.IsCameraPersistent;
            cameraRigType = profile != null
                ? profile.CameraRigType.Type
                : globalProfile.CameraRigType.Type;
            DefaultHeadHeight = profile != null
                ? profile.DefaultHeadHeight
                : globalProfile.DefaultHeadHeight;

            nearClipPlaneOpaqueDisplay = profile != null
                ? profile.NearClipPlaneOpaqueDisplay
                : globalProfile.NearClipPlaneOpaqueDisplay;
            cameraClearFlagsOpaqueDisplay = profile != null
                ? profile.CameraClearFlagsOpaqueDisplay
                : globalProfile.CameraClearFlagsOpaqueDisplay;
            backgroundColorOpaqueDisplay = profile != null
                ? profile.BackgroundColorOpaqueDisplay
                : globalProfile.BackgroundColorOpaqueDisplay;
            opaqueQualityLevel = profile != null
                ? profile.OpaqueQualityLevel
                : globalProfile.OpaqueQualityLevel;

            nearClipPlaneTransparentDisplay = profile != null
                ? profile.NearClipPlaneTransparentDisplay
                : globalProfile.NearClipPlaneTransparentDisplay;
            cameraClearFlagsTransparentDisplay = profile != null
                ? profile.CameraClearFlagsTransparentDisplay
                : globalProfile.CameraClearFlagsTransparentDisplay;
            backgroundColorTransparentDisplay = profile != null
                ? profile.BackgroundColorTransparentDisplay
                : globalProfile.BackgroundColorTransparentDisplay;
            transparentQualityLevel = profile != null
                ? profile.TransparentQualityLevel
                : globalProfile.TransparentQualityLevel;

            bodyAdjustmentAngle = profile != null
                ? profile.BodyAdjustmentAngle
                : globalProfile.BodyAdjustmentAngle;
            bodyAdjustmentSpeed = profile != null
                ? profile.BodyAdjustmentSpeed
                : globalProfile.BodyAdjustmentSpeed;
        }

        private readonly IMixedRealityCameraSystem cameraSystem = null;

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

        /// <inheritdoc />
        public virtual bool IsOpaque => true;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => CameraRig.PlayerCamera.stereoEnabled;

        /// <inheritdoc />
        public IMixedRealityCameraRig CameraRig { get; private set; }

        /// <inheritdoc />
        public float DefaultHeadHeight { get; }

        private float headHeight;

        /// <inheritdoc />
        public virtual float HeadHeight
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

            if (CameraRig == null)
            {
                // TODO Currently we get always get the main camera. Should we provide a tag to search for alts?
                CameraRig = CameraCache.Main.gameObject.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;
                Debug.Assert(CameraRig != null);
            }

            ApplySettingsForDefaultHeadHeight();
            cameraOpaqueLastFrame = IsOpaque;

            if (IsOpaque)
            {
                ApplySettingsForOpaqueDisplay();
            }
            else
            {
                ApplySettingsForTransparentDisplay();
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

            ApplySettingsForDefaultHeadHeight();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying) { return; }

            if (cameraOpaqueLastFrame != IsOpaque)
            {
                cameraOpaqueLastFrame = IsOpaque;

                ApplySettingsForDefaultHeadHeight();

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

            if (!Application.isPlaying) { return; }

            SyncRigTransforms();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (CameraRig == null ||
                CameraRig.GameObject == null)
            {
                return;
            }

            ResetRigTransforms();

            if (CameraRig.PlayerCamera != null &&
                CameraRig.PlayerCamera.transform != null)
            {
                var cameraTransform = CameraRig.PlayerCamera.transform;
                cameraTransform.SetParent(null);
                cameraTransform.position = Vector3.one;
                cameraTransform.rotation = Quaternion.identity;
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

            cameraSystem.UnRegisterCameraDataProvider(this);
        }

        #endregion IMixedRealitySerivce Implementation

        /// <summary>
        /// Depending on whether there is an XR device connected,
        /// moves the camera to the setting from the camera profile.
        /// </summary>
        protected virtual void ApplySettingsForDefaultHeadHeight()
        {
            HeadHeight = DefaultHeadHeight;
            ResetRigTransforms();
            SyncRigTransforms();
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
        /// Resets the <see cref="IMixedRealityCameraRig.PlayspaceTransform"/>, <see cref="IMixedRealityCameraRig.CameraTransform"/>,
        /// and <see cref="IMixedRealityCameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void ResetRigTransforms()
        {
            CameraRig.PlayspaceTransform.position = Vector3.zero;
            CameraRig.PlayspaceTransform.rotation = Quaternion.identity;
            // If the camera is a 2d camera when we can adjust the camera's height to match the head height.
            CameraRig.CameraTransform.position = IsStereoscopic ? Vector3.zero : new Vector3(0f, HeadHeight, 0f);
            CameraRig.CameraTransform.rotation = Quaternion.identity;
            CameraRig.BodyTransform.position = Vector3.zero;
            CameraRig.BodyTransform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Called each <see cref="LateUpdate"/> to the sync the <see cref="IMixedRealityCameraRig.PlayspaceTransform"/>,
        /// <see cref="IMixedRealityCameraRig.CameraTransform"/>, and <see cref="IMixedRealityCameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void SyncRigTransforms()
        {
            var cameraLocalPosition = CameraRig.CameraTransform.localPosition;
            var bodyLocalPosition = CameraRig.BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;

            if (HeadHeight > 0f)
            {
                bodyLocalPosition.y = cameraLocalPosition.y - HeadHeight;
            }
            else
            {
                bodyLocalPosition.y = cameraLocalPosition.y - 1.6f;
            }

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
