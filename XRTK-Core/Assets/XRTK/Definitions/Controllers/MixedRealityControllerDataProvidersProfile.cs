// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.DataProviders.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Data Providers Profiles", fileName = "MixedRealityControllerDataModelsProfile", order = (int)CreateProfileMenuItemIndices.ControllerDataProviders)]
    public class MixedRealityControllerDataProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private ControllerDataProviderConfiguration[] registeredControllerDataProviders = null;

        /// <summary>
        /// The currently registered controller data providers for this input system.
        /// </summary>
        public ControllerDataProviderConfiguration[] RegisteredControllerDataProviders => registeredControllerDataProviders;
    }
}