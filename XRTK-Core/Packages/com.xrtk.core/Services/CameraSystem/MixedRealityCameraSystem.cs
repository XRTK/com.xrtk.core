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
            DefaultHeadHeight = profile.DefaultHeadHeight;

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
                var bodyLocalPosition = CameraRig.BodyTransform.localPosition;
                bodyLocalPosition.y = headHeight;
                CameraRig.BodyTransform.localPosition = bodyLocalPosition;
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

            if (!CameraRig.BodyTransform.localPosition.y.Equals(headHeight))
            {
                var cameraPosition = CameraRig.BodyTransform.localPosition;
                cameraPosition.y = headHeight;
                CameraRig.BodyTransform.localPosition = cameraPosition;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (CameraRig == null) { return; }

            if (CameraRig.CameraTransform != null)
            {
                CameraRig.CameraTransform.SetParent(null);
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