// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Input Actions Profile", fileName = "MixedRealityInputActionsProfile", order = (int)CreateProfileMenuItemIndices.InputActions)]
    public class MixedRealityInputActionsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The list of actions users can do in your application.")]
        private MixedRealityInputAction[] inputActions = { };

        /// <summary>
        /// The list of actions users can do in your application.
        /// </summary>
        /// <remarks>Input Actions are device agnostic and can be paired with any number of device inputs across all platforms.</remarks>
        public MixedRealityInputAction[] InputActions => inputActions;
    }
}