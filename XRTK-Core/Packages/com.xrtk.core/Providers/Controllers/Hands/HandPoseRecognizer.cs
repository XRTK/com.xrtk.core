// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    public sealed class HandPoseRecognizer
    {
        private readonly SimulatedHandControllerPoseData[] trackedPoses;

        public HandPoseRecognizer(IReadOnlyList<SimulatedHandControllerPoseData> trackedPoses)
        {
            this.trackedPoses = trackedPoses.ToArray();
        }

        public void ResolvePose(Handedness handedness, IReadOnlyDictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {

        }
    }
}