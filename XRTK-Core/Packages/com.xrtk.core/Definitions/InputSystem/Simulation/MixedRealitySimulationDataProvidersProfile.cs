// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Simulation/Simulation Data Providers Profile", fileName = "MixedRealitySimulationDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.Simulation)]
    public class MixedRealitySimulationDataProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private SimulationDataProviderConfiguration[] registeredInputSimulationDataProviders = new SimulationDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered input simulation data providers for this input system.
        /// </summary>
        public SimulationDataProviderConfiguration[] RegisteredInputSimulationDataProviders => registeredInputSimulationDataProviders;
    }
}