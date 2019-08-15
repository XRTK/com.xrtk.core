// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.Providers.InputSystem.Simulation
{
    public interface IHandTrackingSimulationDataProvider : IMixedRealitySimulationDataProvider
    {
        void RaiseHandJointsUpdated(Interfaces.InputSystem.IMixedRealityInputSource source, Definitions.Utilities.Handedness handedness, System.Collections.Generic.IDictionary<Definitions.Utilities.TrackedHandJoint, Definitions.Utilities.MixedRealityPose> jointPoses);

        void RaiseHandMeshUpdated(Interfaces.InputSystem.IMixedRealityInputSource source, Definitions.Utilities.Handedness handedness, XRTK.Providers.Controllers.Hands.HandMeshUpdatedEventData handMeshInfo);

        void Register(UnityEngine.GameObject listener);

        void Unregister(UnityEngine.GameObject listener);
    }
}