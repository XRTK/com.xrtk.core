// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Speech;
using XRTK.Services;

namespace XRTK.Providers.Speech
{
    /// <summary>
    /// Base speech data provider to inherit from when implementing <see cref="IMixedRealitySpeechDataProvider"/>s
    /// </summary>
    public abstract class BaseSpeechDataProvider : BaseDataProvider, IMixedRealitySpeechDataProvider
    {
        /// <inheritdoc />
        protected BaseSpeechDataProvider(string name, uint priority, MixedRealitySpeechCommandsProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            InputSystem = parentService;
        }

        protected readonly IMixedRealityInputSystem InputSystem;

        /// <inheritdoc />
        public virtual bool IsRecognitionActive { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecognition() { }

        /// <inheritdoc />
        public virtual void StopRecognition() { }
    }
}