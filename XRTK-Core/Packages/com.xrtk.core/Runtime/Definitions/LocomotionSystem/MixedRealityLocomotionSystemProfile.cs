// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile settings for <see cref="Services.LocomotionSystem.MixedRealityLocomotionSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Locomotion System Profile", fileName = "MixedRealityLocomotionSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityLocomotionSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityLocomotionProvider>
    {
        #region Teleporting

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
