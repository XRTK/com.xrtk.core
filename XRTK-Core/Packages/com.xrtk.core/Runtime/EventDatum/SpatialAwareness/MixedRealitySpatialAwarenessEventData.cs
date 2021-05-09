// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;
using XRTK.Interfaces.Providers.SpatialObservers;

namespace XRTK.EventDatum.SpatialAwareness
{
    /// <summary>
    /// Data for spatial awareness events.
    /// </summary>
    public class MixedRealitySpatialAwarenessEventData<T> : GenericBaseEventData
    {
        /// <summary>
        /// Identifier of the object associated with this event.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The spatial object managed by the spatial awareness system, representing the data in this event.
        /// </summary>
        public T SpatialObject { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="spatialAwarenessObserver"></param>
        /// <param name="id"></param>
        /// <param name="spatialObject"></param>
        public void Initialize(IMixedRealitySpatialAwarenessDataProvider spatialAwarenessObserver, Guid id, T spatialObject)
        {
            BaseInitialize(spatialAwarenessObserver);
            Id = id;
            SpatialObject = spatialObject;
        }
    }
}
