// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public sealed class SimulatedHandDataConverter : BaseHandDataConverter
    {
        public SimulatedHandDataConverter(Handedness handedness, IReadOnlyList<SimulatedHandControllerPoseData> trackedPoses) : base(handedness, trackedPoses)
        { }

        public void Finalize(HandData handData)
        {
            PostProcess(handData);
        }
    }
}