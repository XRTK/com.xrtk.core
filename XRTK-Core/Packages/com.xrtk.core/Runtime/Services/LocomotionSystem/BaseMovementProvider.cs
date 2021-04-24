// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for handling <see cref="IMixedRealityLocomotionSystem"/> movement events in a
    /// <see cref="MonoBehaviour"/> component.
    /// </summary>
    public abstract class BaseMovementProvider : MonoBehaviour, IMixedRealityMovementProvider
    {
        private bool lateInitialize = true;

        private MixedRealityLocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected MixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>() as MixedRealityLocomotionSystem);

        private IMixedRealityInputSystem inputSystem = null;
        /// <summary>
        /// Gets the active <see cref="IMixedRealityInputSystem"/> implementation instance.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
            => inputSystem ?? (inputSystem = MixedRealityToolkit.GetSystem<IMixedRealityInputSystem>());

        protected virtual void OnEnable()
        {
            if (!lateInitialize &&
                MixedRealityToolkit.IsInitialized)
            {
                InputSystem?.Register(gameObject);
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                try
                {
                    inputSystem = await MixedRealityToolkit.GetSystemAsync<IMixedRealityInputSystem>();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }

                // We've been destroyed during the await.
                if (this == null) { return; }

                lateInitialize = false;
                InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            InputSystem?.Unregister(gameObject);
        }

        protected virtual void OnDestroy()
        {
            InputSystem?.Unregister(gameObject);
        }
    }
}
