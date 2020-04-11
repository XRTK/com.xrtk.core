// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;

namespace XRTK.Services.CameraSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's default implementation of the <see cref="IMixedRealityCameraSystem"/>.
    /// </summary>
    public class MixedRealityCameraSystem : BaseSystem, IMixedRealityCameraSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityCameraSystem(MixedRealityCameraProfile profile)
            : base(profile)
        {
            DefaultHeadHeight = profile.DefaultHeadHeight;

            if (profile.CameraRigType?.Type == null)
            {
                throw new Exception("Camera rig type cannot be null!");
            }

            cameraRigType = profile.CameraRigType?.Type;
            isCameraPersistent = profile.IsCameraPersistent;

            opaqueQualityLevel = profile.OpaqueQualityLevel;
            transparentQualityLevel = profile.TransparentQualityLevel;

            nearClipPlaneOpaqueDisplay = profile.NearClipPlaneOpaqueDisplay;
            nearClipPlaneTransparentDisplay = profile.NearClipPlaneTransparentDisplay;

            backgroundColorOpaqueDisplay = profile.BackgroundColorOpaqueDisplay;
            backgroundColorTransparentDisplay = profile.BackgroundColorTransparentDisplay;

            cameraClearFlagsOpaqueDisplay = profile.CameraClearFlagsOpaqueDisplay;
            cameraClearFlagsTransparentDisplay = profile.CameraClearFlagsTransparentDisplay;

            bodyAdjustmentAngle = profile.BodyAdjustmentAngle;
            bodyAdjustmentSpeed = profile.BodyAdjustmentSpeed;
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

        /// <inheritdoc />
        public bool IsOpaque
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

        /// <inheritdoc />
        public bool IsStereoscopic => UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRDevice.isPresent;

        /// <inheritdoc />
        public IMixedRealityCameraRig CameraRig { get; private set; }

        private DisplayType currentDisplayType;
        private bool cameraOpaqueLastFrame = false;

        private enum DisplayType
        {
            Opaque = 0,
            Transparent
        }

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
                CameraRig = CameraCache.Main.gameObject.EnsureComponent(cameraRigType) as IMixedRealityCameraRig;
                Debug.Assert(CameraRig != null);
                ResetRigTransforms();
            }

            ApplySettingsForDefaultHeadHeight();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            ResetRigTransforms();

            if (Application.isPlaying &&
                isCameraPersistent)
            {
                CameraCache.Main.transform.root.DontDestroyOnLoad();
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

            var camera = CameraCache.Main;

            if (camera != null)
            {
                camera.transform.SetParent(null);
            }

            if (CameraRig == null) { return; }

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

        /// <summary>
        /// Depending on whether there is an XR device connected,
        /// moves the camera to the setting from the camera profile.
        /// </summary>
        private void ApplySettingsForDefaultHeadHeight()
        {
            if (!IsStereoscopic)
            {
                // If not device attached we'll just use the default head height setting.
                CameraRig.CameraTransform.Translate(0f, DefaultHeadHeight, 0f);
            }
            else
            {
                // If we have a stereoscopic device attached we'll leave
                // height control to the device itself and reset everything to origin.
                ResetRigTransforms();
            }
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        private void ApplySettingsForOpaqueDisplay()
        {
            CameraCache.Main.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraCache.Main.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraCache.Main.backgroundColor = backgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(opaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        private void ApplySettingsForTransparentDisplay()
        {
            CameraCache.Main.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraCache.Main.backgroundColor = backgroundColorTransparentDisplay;
            CameraCache.Main.nearClipPlane = nearClipPlaneTransparentDisplay;
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
