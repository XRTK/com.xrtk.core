// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.InputSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Base configuration profile for <see cref="Interfaces.LocomotionSystem.ILocomotionProvider"/>s. Use the <see cref="Providers.LocomotionSystem.BaseLocomotionProvider"/>
    /// base class to get started implementing your own provider.
    /// </summary>
    public class BaseLocomotionProviderProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Sets startup behaviour for this provider.")]
        private AutoStartBehavior startupBehaviour = AutoStartBehavior.ManualStart;

        /// <summary>
        /// Gets startup behaviour for this provider.
        /// </summary>
        public AutoStartBehavior StartupBehaviour => startupBehaviour;

        [SerializeField]
        [Tooltip("Input action to perform locomotion using this provider.")]
        private InputActionReference inputAction = null;

        /// <summary>
        /// Gets input action to perform locomotion using this provider.
        /// </summary>
        public InputActionReference InputAction => inputAction;
    }
}
