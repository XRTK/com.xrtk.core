// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Data class that maps <see cref="InputActionReference"/>s to <see cref="UnityEngine.Events.UnityEvent"/>s wired up in the inspector.
    /// </summary>
    [Serializable]
    public class InputActionEventPair
    {
        [SerializeField]
        [Tooltip("The InputAction to listen for to invoke the UnityEvent.")]
        private InputActionReference inputAction = null;

        /// <summary>
        /// The <see cref="UnityEngine.InputSystem.InputAction"/> to listen for to invoke the <see cref="UnityEngine.Events.UnityEvent"/>.
        /// </summary>
        public InputAction InputAction => inputAction;

        [SerializeField]
        [Tooltip("The UnityEvent to invoke when InputAction is raised.")]
        private UnityEvent unityEvent = null;

        /// <summary>
        /// The <see cref="UnityEngine.Events.UnityEvent"/> to invoke when <see cref="UnityEngine.InputSystem.InputAction"/> is raised.
        /// </summary>
        public UnityEvent UnityEvent => unityEvent;
    }
}
