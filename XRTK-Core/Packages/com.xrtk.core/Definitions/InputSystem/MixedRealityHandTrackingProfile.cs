// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Controllers.Hands;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming gesture based input actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Hand Tracking Profile", fileName = "MixedRealityHandTrackingProfile", order = (int)CreateProfileMenuItemIndices.HandTracking)]
    public class MixedRealityHandTrackingProfile : BaseMixedRealityProfile
    {
        [Header("General Settings")]

        [SerializeField]
        [Tooltip("If set, hand mesh data will be read and available for visualzation. Disable for optimized performance.")]
        private bool handMeshingEnabled = false;
        /// <summary>
        /// If set, hand mesh data will be read and available for visualzation. Disable for optimized performance.
        /// </summary>
        public bool HandMeshingEnabled => handMeshingEnabled;

        [SerializeField]
        [Tooltip("The hand ray concrete type to use when raycasting for hand interaction.")]
        [Implements(typeof(IMixedRealityHandRay), TypeGrouping.ByNamespaceFlat)]
        private SystemType handRayType;

        /// <summary>
        /// The hand ray concrete type to use when raycasting for hand interaction.
        /// </summary>
        public SystemType HandRayType => handRayType;

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