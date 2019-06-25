// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Simulation/Eye Tracking Input Simulation Profile", fileName = "MixedRealityEyeTrackingInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    public class MixedRealityEyeTrackingInputSimulationDataProviderProfile : BaseMixedRealityInputSimulationDataProviderProfile
    {
        [Header("Eye Simulation")]
        [SerializeField]
        [Tooltip("Enable eye simulation")]
        private bool simulateEyePosition = false;
        public bool SimulateEyePosition => simulateEyePosition;
    }
}