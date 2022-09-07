// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Data structure for mapping Voice and <see cref="InputActionReference"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public class SpeechCommands
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The Keyword.</param>
        /// <param name="inputAction">The Action.</param>
        public SpeechCommands(string keyword, InputActionReference inputAction)
        {
            this.keyword = keyword;
            this.inputAction = inputAction;
            inputAction.action.performed += OnInputAction_Performed;
        }

        ~SpeechCommands()
        {
            inputAction.action.performed -= OnInputAction_Performed;
        }

        [SerializeField]
        [Tooltip("The Keyword to listen for.")]
        private string keyword;

        /// <summary>
        /// The Keyword to listen for.
        /// </summary>
        public string Keyword => keyword;

        [SerializeField]
        [Tooltip("The InputAction to listen for.")]
        private InputActionReference inputAction;

        /// <summary>
        /// The <see cref="InputAction"/> to listen for.
        /// </summary>
        public InputActionReference InputAction => inputAction;

        public event Action<string> OnKeyword;

        private void OnInputAction_Performed(InputAction.CallbackContext callbackContext)
        {
            OnKeyword?.Invoke(keyword);
        }
    }
}
