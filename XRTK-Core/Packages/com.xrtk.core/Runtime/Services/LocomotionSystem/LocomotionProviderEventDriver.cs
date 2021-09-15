// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// This component is attached to the main <see cref="Camera"/> by the <see cref="ILocomotionSystem"/>
    /// and provides an event bridge to active <see cref="ILocomotionProvider"/> implementations.
    /// It has a hard dependency on the <see cref="ILocomotionSystem"/> as well as the <see cref="IMixedRealityInputSystem"/>
    /// and cannot work without both being active and enabled in the application.
    /// Furthermore it expects that the <see cref="GameObject"/> it is attached to is a global <see cref="IMixedRealityInputSystem"/> listener. It will
    /// not take care of registration itself.
    /// </summary>
    public class LocomotionProviderEventDriver : MonoBehaviour,
        ILocomotionSystemHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>
    {
        private LocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="Services.LocomotionSystem.LocomotionSystem"/> instance.
        /// </summary>
        protected LocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<ILocomotionSystem>() as LocomotionSystem);

        /// <summary>
        /// This method is called just before any of the update methods is called the first time.
        /// </summary>
        protected virtual async void Start()
        {
            try
            {
                locomotionSystem = (await MixedRealityToolkit.GetSystemAsync<ILocomotionSystem>()) as LocomotionSystem;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }

            // We've been destroyed during the await.
            if (this == null) { return; }

            LocomotionSystem?.Register(gameObject);
        }

        /// <summary>
        /// This method is called when the behaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy() => LocomotionSystem?.Unregister(gameObject);

        /// <inheritdoc />
        public virtual void OnTeleportTargetRequested(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnTeleportTargetRequested(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnTeleportStarted(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnTeleportCompleted(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnTeleportCanceled(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnInputChanged(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector2> eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnInputChanged(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnInputDown(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnInputUp(eventData);
            }
        }
    }
}
