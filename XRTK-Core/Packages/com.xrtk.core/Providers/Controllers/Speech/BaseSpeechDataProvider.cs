// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Speech
{
    /// <summary>
    /// Base speech data provider to inherit from when implementing <see cref="IMixedRealitySpeechDataProvider"/>s
    /// </summary>
    public abstract class BaseSpeechDataProvider : BaseControllerDataProvider, IMixedRealitySpeechDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseSpeechDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        /// <inheritdoc />
        public virtual bool IsRecognitionActive { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecognition() { }

        /// <inheritdoc />
        public virtual void StopRecognition() { }
    }
}