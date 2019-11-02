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
            this.profile = profile;
            this.initialHeadHeight = profile.InitialHeadHeight;

            if (profile.CameraRigType.Type == null)
            {
                throw new Exception("Camera rig type cannot be null!");
            }
        }

        private readonly MixedRealityCameraProfile profile;

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

        private float initialHeadHeight = 1.6f;

        public float InitialHeadHeight { get => initialHeadHeight; set => initialHeadHeight = value; }
        
        /// <inheritdoc />
        public bool IsStereoscopic => UnityEngine.XR.XRSettings.enabled;

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
                CameraRig = CameraCache.Main.gameObject.EnsureComponent(profile.CameraRigType.Type) as IMixedRealityCameraRig;
                Debug.Assert(CameraRig != null);
                CameraRig.BodyTransform.position = CameraRig.CameraTransform.position;
                CameraRig.BodyTransform.rotation = CameraRig.CameraTransform.rotation;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            // Reset the body rig transform position
            CameraRig.BodyTransform.position = CameraRig.CameraTransform.position;
            CameraRig.BodyTransform.rotation = CameraRig.CameraTransform.rotation;

            if (Application.isPlaying &&
                profile.IsCameraPersistent)
            {
                CameraCache.Main.transform.root.DontDestroyOnLoad();
            }
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

            CameraRig.BodyTransform.position = CameraRig.CameraTransform.position;

            var bodyRotation = CameraRig.BodyTransform.rotation;
            var headRotation = CameraRig.CameraTransform.rotation;

            var currentAngle = Mathf.Abs(Quaternion.Angle(bodyRotation, headRotation));

            if (currentAngle > profile.BodyAdjustmentAngle)
            {
                CameraRig.BodyTransform.rotation = Quaternion.Slerp(bodyRotation, headRotation, Time.deltaTime);
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (CameraRig != null)
            {
                var component = CameraRig as Component;

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
        /// Applies opaque settings from camera profile.
        /// </summary>
        private void ApplySettingsForOpaqueDisplay()
        {
            CameraCache.Main.clearFlags = profile.CameraClearFlagsOpaqueDisplay;
            CameraCache.Main.nearClipPlane = profile.NearClipPlaneOpaqueDisplay;
            CameraCache.Main.backgroundColor = profile.BackgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(profile.OpaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        private void ApplySettingsForTransparentDisplay()
        {
            CameraCache.Main.clearFlags = profile.CameraClearFlagsTransparentDisplay;
            CameraCache.Main.backgroundColor = profile.BackgroundColorTransparentDisplay;
            CameraCache.Main.nearClipPlane = profile.NearClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(profile.TransparentQualityLevel, false);
        }
    }
}