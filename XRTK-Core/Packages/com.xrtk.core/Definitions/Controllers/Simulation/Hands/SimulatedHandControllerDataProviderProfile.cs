// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Simulation.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Simulated Hand Controller Data Provider Profile", fileName = "SimulatedHandControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class SimulatedHandControllerDataProviderProfile : SimulatedControllerDataProviderProfile
    {
        [SerializeField]
        [Tooltip("Hand pose definitions.")]
        private List<SimulatedHandControllerPoseData> poseDefinitions = new List<SimulatedHandControllerPoseData>();

        /// <summary>
        /// Hand pose definitions.
        /// </summary>
        public IReadOnlyList<SimulatedHandControllerPoseData> PoseDefinitions => poseDefinitions;

        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handPoseAnimationSpeed = 8.0f;

        /// <summary>
        /// Pose interpolation per second.
        /// </summary>
        public float HandPoseAnimationSpeed => handPoseAnimationSpeed;
    }
}