// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Services;
using XRTK.Utilities.Async;

namespace XRTK.SDK.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see cref="GameObject"/> when focus or gaze is not required.
    /// </summary>
    public abstract class InputSystemGlobalListener : MonoBehaviour
    {
        private bool lateInitialize = true;

        protected readonly WaitUntil WaitUntilInputSystemValid = new WaitUntil(() => MixedRealityToolkit.InputSystem != null);

        protected virtual void OnEnable()
        {
            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.InputSystem != null && !lateInitialize)
            {
                MixedRealityToolkit.InputSystem.Register(gameObject);
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                await WaitUntilInputSystemValid;

                // We've been destroyed during the await.
                if (this == null) { return; }

                lateInitialize = false;
                MixedRealityToolkit.InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            MixedRealityToolkit.InputSystem?.Unregister(gameObject);
        }
    }
}
