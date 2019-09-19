// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Speech
{
    /// <summary>
    /// Base dictation data provider to use when implementing <see cref="IMixedRealityDictationDataProvider"/>s
    /// </summary>
    public abstract class BaseDictationDataProvider : BaseControllerDataProvider, IMixedRealityDictationDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseDictationDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        #region IMixedRealityDictationDataProvider Implementation

        /// <inheritdoc />
        public virtual bool IsListening { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecording(GameObject listener = null, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
        }

        /// <inheritdoc />
        public virtual Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
            return null;
        }

        /// <inheritdoc />
        public virtual void StopRecording()
        {
        }

        /// <inheritdoc />
        public virtual Task<AudioClip> StopRecordingAsync()
        {
            return null;
        }

        #endregion IMixedRealityDictationDataProvider Implementation
    }
}