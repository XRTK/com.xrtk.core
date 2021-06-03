// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.EventDatum.Input;

namespace XRTK.Services.LocomotionSystem
{
    public class LocomotionProvider : MonoBehaviour,
        IMixedRealityLocomotionHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>
    {
        private MixedRealityLocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected MixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>() as MixedRealityLocomotionSystem);

        /// <summary>
        /// This method is called just before any of the update methods is called the first time.
        /// </summary>
        protected virtual async void Start()
        {
            try
            {
                locomotionSystem = (await MixedRealityToolkit.GetSystemAsync<IMixedRealityLocomotionSystem>()) as MixedRealityLocomotionSystem;
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
        public virtual void OnLocomotionRequest(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnLocomotionRequest(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnLocomotionStarted(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnLocomotionStarted(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnLocomotionCompleted(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnLocomotionCompleted(eventData);
            }
        }

        /// <inheritdoc />
        public virtual void OnLocomotionCanceled(LocomotionEventData eventData)
        {
            for (int i = 0; i < LocomotionSystem.EnabledLocomotionProviders.Count; i++)
            {
                LocomotionSystem.EnabledLocomotionProviders[i].OnLocomotionCanceled(eventData);
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
