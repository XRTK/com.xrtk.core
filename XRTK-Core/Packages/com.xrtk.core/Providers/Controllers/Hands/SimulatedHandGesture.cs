// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;

namespace XRTK.Services.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Simulation/Simulated Hand Gesture", fileName = "MixedRealitySimulatedHandGesture", order = (int)CreateProfileMenuItemIndices.Input)]
    public class SimulatedHandGesture : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Name the hand gesture.")]
        private string gestureName;

        /// <summary>
        /// Gets the gesture's name.
        /// </summary>
        public string GestureName => gestureName;

        [SerializeField]
        [Tooltip("Describe the hand gesture.")]
        private string gestureDescription;

        /// <summary>
        /// Gets the gesture's description.
        /// </summary>
        public string GestureDescription => gestureDescription;

        [SerializeField]
        [Tooltip("Is this the default (idle) hand gesture?")]
        private bool isDefault;

        /// <summary>
        /// Gets whether this hand gesture is the hand's idle gesture.
        /// </summary>
        public bool IsDefault => isDefault;

        [SerializeField]
        [Tooltip("Key used to trigger the gesture simulation.")]
        private KeyCode keyCode;

        /// <summary>
        /// Gets the key code used to trigger this gesture simulation.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Assign JSON defintion file containing simulated gesture information.")]
        private TextAsset data;

        /// <summary>
        /// Gets the gesture definition's joint information used to simulate the gesture.
        /// </summary>
        public TextAsset Data => data;
    }
}