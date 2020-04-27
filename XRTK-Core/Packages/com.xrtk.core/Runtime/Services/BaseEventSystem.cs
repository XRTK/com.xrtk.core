// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions;
using XRTK.Interfaces.Events;
using XRTK.Utilities.Async;

namespace XRTK.Services
{
    /// <summary>
    /// Base Event System that can be inherited from to give other system features event capabilities.
    /// </summary>
    public abstract class BaseEventSystem : BaseSystem, IMixedRealityEventSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        protected BaseEventSystem(BaseMixedRealityProfile profile)
            : base(profile)
        {
        }

        #region IMixedRealityEventSystem Implementation

        private static int eventExecutionDepth = 0;

        private readonly List<GameObject> eventListeners = new List<GameObject>();

        /// <inheritdoc />
        public IReadOnlyList<GameObject> EventListeners => eventListeners;

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);
            eventExecutionDepth++;

            for (int i = EventListeners.Count - 1; i >= 0; i--)
            {
                var eventListener = EventListeners[i];
                Debug.Assert(eventListener != null, $"An object at index {i} has been destroyed but remains in the event handler list for {Name}.BaseEventSystem");
                ExecuteEvents.Execute(eventListener, eventData, eventHandler);
            }

            eventExecutionDepth--;
        }

        /// <inheritdoc />
        public virtual async void Register(GameObject listener)
        {
            if (eventListeners.Contains(listener)) { return; }

            if (eventExecutionDepth > 0)
            {
                try
                {
                    await eventExecutionDepth.WaitUntil(depth => eventExecutionDepth == 0);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                    return;
                }
            }

            eventListeners.Add(listener);
        }

        /// <inheritdoc />
        public virtual async void Unregister(GameObject listener)
        {
            if (!eventListeners.Contains(listener)) { return; }

            if (eventExecutionDepth > 0)
            {
                try
                {
                    await eventExecutionDepth.WaitUntil(depth => eventExecutionDepth == 0);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                    return;
                }
            }

            eventListeners.Remove(listener);
        }

        #endregion IMixedRealityEventSystem Implementation

        // Example Event Pattern #############################################################

        //public void RaiseGenericEvent(IEventSource eventSource)
        //{
        //    genericEventData.Initialize(eventSource);
        //    HandleEvent(genericEventData, GenericEventHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<IEventHandler> GenericEventHandler =
        //    delegate (IEventHandler handler, BaseEventData eventData)
        //    {
        //        var casted = ExecuteEvents.ValidateEventData<GenericBaseEventData>(eventData);
        //        handler.OnEventRaised(casted);
        //    };

        // Example Event Pattern #############################################################
    }
}
