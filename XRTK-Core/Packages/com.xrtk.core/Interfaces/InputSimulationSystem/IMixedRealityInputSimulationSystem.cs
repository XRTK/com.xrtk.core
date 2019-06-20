// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.InputSimulationSystem;

namespace XRTK.Interfaces.InputSimulationSystem
{
    public interface IMixedRealityInputSimulationSystem : IMixedRealityService
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityInputSimulationSystemProfile InputSimulationProfile { get; }
    }
}