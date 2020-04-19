// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.CameraSystem;

namespace XRTK.Definitions.CameraSystem
{
    /// <summary>
    /// This <see cref="BaseMixedRealityProfile"/> tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Camera System Profile", fileName = "MixedRealityCameraSystemProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The Global Camera Profile Settings.")]
        private BaseMixedRealityCameraDataProviderProfile globalCameraProfile = null;

        /// <summary>
        /// The default camera data provider profile <see cref="IMixedRealityCameraDataProvider"/>s will use if no profile is assigned.
        /// </summary>
        public BaseMixedRealityCameraDataProviderProfile GlobalCameraProfile => globalCameraProfile;

        [SerializeField]
        [Tooltip("The list of registered Camera Data Providers for each platform.")]
        private MixedRealityCameraDataProvidersProfile cameraDataProvidersProfile = null;

        /// <summary>
        /// The list of registered <see cref="IMixedRealityCameraDataProvider"/>s for each platform.
        /// </summary>
        public MixedRealityCameraDataProvidersProfile CameraDataProvidersProfile => cameraDataProvidersProfile;
    }
}