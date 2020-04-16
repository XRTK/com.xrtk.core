// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// A hand controller pose definition with recorded hand joint data.
    /// Defined hand poses can be recognized and trigger input actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Hand Controller Pose Definition", fileName = "HandControllerPoseDefinition", order = (int)CreateProfileMenuItemIndices.Input)]
    public class HandControllerPoseDefinition : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Assign a unique ID to the pose. Can be used as a descriptive name, but must be unique!")]
        private string id = string.Empty;

        /// <summary>
        /// A unique identifier for the hand pose.
        /// </summary>
        public string Id => id;

        [SerializeField]
        [Tooltip("Describe the hand pose.")]
        private string description = string.Empty;

        /// <summary>
        /// Gets the gesture's description.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Is this the default (idle) hand pose?")]
        private bool isDefault = false;

        /// <summary>
        /// Gets whether this hand pose is the hand's idle pose.
        /// </summary>
        public bool IsDefault => isDefault;

        [SerializeField]
        [Tooltip("Key used to trigger the gesture simulation.")]
        private KeyCode keyCode = KeyCode.None;

        /// <summary>
        /// Gets the key code used to trigger this gesture simulation.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Assign JSON definition file containing simulated gesture information.")]
        private TextAsset data = null;

        /// <summary>
        /// Gets the gesture definition's joint information used to simulate the gesture.
        /// </summary>
        public TextAsset Data => data;
    }
}