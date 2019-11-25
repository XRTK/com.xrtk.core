// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// A hand pose definition with recorded hand joint data for use with <see cref="SimulationHandControllerDataProvider"/>.
    /// Assign a collection of these poses to the data provider's profile to simulate the pose.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Hands/Unity Editor Hand Pose", fileName = "UnityEditorHandPoseData", order = (int)CreateProfileMenuItemIndices.Input)]
    public class SimulationHandPoseData : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Name the hand gesture.")]
        private string gestureName = string.Empty;

        /// <summary>
        /// Gets the gesture's name.
        /// </summary>
        public string GestureName => gestureName;

        [SerializeField]
        [Tooltip("Describe the hand gesture.")]
        private string gestureDescription = string.Empty;

        /// <summary>
        /// Gets the gesture's description.
        /// </summary>
        public string GestureDescription => gestureDescription;

        [SerializeField]
        [Tooltip("Is this the default (idle) hand gesture?")]
        private bool isDefault = false;

        /// <summary>
        /// Gets whether this hand gesture is the hand's idle gesture.
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