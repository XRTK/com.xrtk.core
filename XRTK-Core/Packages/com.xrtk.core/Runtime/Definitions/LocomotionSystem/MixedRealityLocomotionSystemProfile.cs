// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile settings for <see cref="Services.LocomotionSystem.MixedRealityLocomotionSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Locomotion System Profile", fileName = "MixedRealityLocomotionSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityLocomotionSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityLocomotionDataProvider>
    {
        #region General Settings

        [SerializeField]
        [Tooltip("Configures startup behaviour for locomotion.")]
        private AutoStartBehavior startupBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// Gets the configured locomotion startup behaviour.
        /// </summary>
        public AutoStartBehavior StartupBehavior
        {
            get => startupBehavior;
            internal set => startupBehavior = value;
        }

        #endregion

        #region Teleporting

        [SerializeField]
        [Tooltip("The concrete teleport provider to use for teleportation.")]
        [Implements(typeof(IMixedRealityTeleportProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType teleportProvider;

        /// <summary>
        /// The concrete teleport provider to use for teleportation.
        /// </summary>
        public SystemType TeleportProvider
        {
            get => teleportProvider;
            internal set => teleportProvider = value;
        }

        [SerializeField]
        [Tooltip("Input action to trigger a teleport request.")]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

        /// <summary>
        /// Input action to trigger a teleport request.
        /// </summary>
        public MixedRealityInputAction TeleportAction
        {
            get => teleportAction;
            internal set => teleportAction = value;
        }

        #endregion Teleporting

        #region Movement

        [SerializeField]
        [Tooltip("If set, movement will cancel any teleportation in progress.")]
        private bool movementCancelsTeleport = true;

        /// <summary>
        /// If set, movement will cancel any teleportation in progress.
        /// </summary>
        public bool MovementCancelsTeleport
        {
            get => movementCancelsTeleport;
            internal set => movementCancelsTeleport = value;
        }

        [SerializeField]
        [Tooltip("Speed in meters per second for movement.")]
        [Range(1f, 100f)]
        private float movementSpeed = 5f;

        /// <summary>
        /// Speed in meters per second for movement.
        /// </summary>
        public float MovementSpeed
        {
            get => movementSpeed;
            internal set => movementSpeed = value;
        }

        [SerializeField]
        [Tooltip("The concrete movement provider to use for moving around.")]
        [Implements(typeof(IMixedRealityMovementProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType movementProvider;

        /// <summary>
        /// The concrete movement provider to use for moving around.
        /// </summary>
        public SystemType MovementProvider
        {
            get => movementProvider;
            internal set => movementProvider = value;
        }

        [SerializeField]
        [Tooltip("Input action to move the player.")]
        private MixedRealityInputAction moveAction = MixedRealityInputAction.None;

        /// <summary>
        /// Input action to move the player.
        /// </summary>
        public MixedRealityInputAction MoveAction
        {
            get => moveAction;
            internal set => moveAction = value;
        }

        #endregion
    }
}
