// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
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


        [Header("General Settings")]

        [SerializeField]
        [Tooltip("If set, hand mesh data will be read and available for visualization. Disable for optimized performance.")]
        private bool handMeshingEnabled = false;

        /// <summary>
        /// If set, hand mesh data will be read and available for visualization. Disable for optimized performance.
        /// </summary>
        public bool HandMeshingEnabled => handMeshingEnabled;

        [Header("Hand Physics")]

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
    }
}