// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Input Simulation Data Providers Profile", fileName = "MixedRealityInputSimulationDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    public class MixedRealityInputSimulationDataProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private InputSystemDataProviderConfiguration[] registeredInputSimulationDataProviders = new InputSystemDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered input simulation data providers for this input system.
        /// </summary>
        public InputSystemDataProviderConfiguration[] RegisteredInputSimulationDataProviders => registeredInputSimulationDataProviders;
    }
}