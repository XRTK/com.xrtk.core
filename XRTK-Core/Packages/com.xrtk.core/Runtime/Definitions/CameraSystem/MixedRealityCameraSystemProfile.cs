// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.CameraSystem;

namespace XRTK.Definitions.CameraSystem
{
    /// <summary>
    /// This <see cref="BaseMixedRealityProfile"/> to configuring your applications <see cref="IMixedRealityCameraDataProvider"/>s.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Camera System Profile", fileName = "MixedRealityCameraSystemProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityCameraDataProvider>
    {
        [SerializeField]
        [Tooltip("The Global Camera Profile Settings.")]
        private BaseMixedRealityCameraDataProviderProfile globalCameraProfile = null;

        /// <summary>
        /// The default camera data provider profile <see cref="IMixedRealityCameraDataProvider"/>s will use if no profile is assigned.
        /// </summary>
        public BaseMixedRealityCameraDataProviderProfile GlobalCameraProfile => globalCameraProfile;
    }
}