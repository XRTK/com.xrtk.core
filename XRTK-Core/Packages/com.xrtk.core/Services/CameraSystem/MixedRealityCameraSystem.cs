// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Utilities;
using XRTK.Extensions;

namespace XRTK.Services.CameraSystem
{
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

        public override void Enable()
        {
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