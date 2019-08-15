// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Hands", fileName = "MixedRealityHandControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class HandControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        [SerializeField]
        private HandControllerDataProviderConfiguration[] registeredControllerDataProviders = new HandControllerDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered controller data providers for this input system.
        /// </summary>
        public HandControllerDataProviderConfiguration[] RegisteredControllerDataProviders => registeredControllerDataProviders;
    }
}
