// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Simulation.Hands;

namespace XRTK.Definitions.Controllers.Simulation.Hands
{
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

        [SerializeField]
        [Tooltip("If set, hand mesh data will be read and available for visualization. Disable for optimized performance.")]
        private bool handMeshingEnabled = false;

        /// <summary>
        /// If set, hand mesh data will be read and available for visualization. Disable for optimized performance.
        /// </summary>
        public bool HandMeshingEnabled => handMeshingEnabled;

        [SerializeField]
        [Tooltip("If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.")]
        private bool handPhysicsEnabled = false;

        /// <summary>
        /// If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.
        /// </summary>
        public bool HandPhysicsEnabled => handPhysicsEnabled;

        [SerializeField]
        [Tooltip("If set, hand colliders will be setup as triggers.")]
        private bool useTriggers = false;

        /// <summary>
        /// If set, hand colliders will be setup as triggers.
        /// </summary>
        public bool UseTriggers => useTriggers;

        [SerializeField]
        [Tooltip("Set the bounds mode to use for calculating hand bounds.")]
        private HandBoundsMode boundsMode = HandBoundsMode.Hand;

        /// <summary>
        /// Set the bounds mode to use for calculating hand bounds.
        /// </summary>
        public HandBoundsMode BoundsMode => boundsMode;

        [SerializeField]
        [Tooltip("Tracked hand poses for pose detection.")]
        private List<SimulatedHandControllerPoseData> trackedPoses = new List<SimulatedHandControllerPoseData>();

        /// <summary>
        /// Tracked hand poses for pose detection.
        /// </summary>
        public IReadOnlyList<SimulatedHandControllerPoseData> TrackedPoses => trackedPoses;

        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(SimulatedHandController), Handedness.Left),
                new ControllerDefinition(typeof(SimulatedHandController), Handedness.Right),
            };
        }
    }
}