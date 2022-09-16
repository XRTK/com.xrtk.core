// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Data structure for mapping Voice Commands that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public class SpeechCommands
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The Keyword.</param>
        public SpeechCommands(string keyword)
        {
            this.keyword = keyword;
        }

        [SerializeField]
        [Tooltip("The Keyword to listen for.")]
        private string keyword;

        /// <summary>
        /// The Keyword to listen for.
        /// </summary>
        public string Keyword => keyword;
    }
}
