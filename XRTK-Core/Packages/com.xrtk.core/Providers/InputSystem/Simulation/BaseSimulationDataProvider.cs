// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.Providers.InputSystem.Simulation;
using XRTK.Services;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class BaseSimulationDataProvider : BaseDataProvider, IMixedRealityInputSimulationDataProvider
    {
        public BaseSimulationDataProvider(string name, uint priority) : base(name, priority)
        {
        }
    }
}