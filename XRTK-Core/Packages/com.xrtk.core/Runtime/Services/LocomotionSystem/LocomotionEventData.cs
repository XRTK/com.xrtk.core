// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Describes a locomotion event raised by the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public class LocomotionEventData : GenericBaseEventData
    {
        /// <summary>
        /// The pointer that raised the event.
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <summary>
        /// The teleport hot spot.
        /// </summary>
        public IMixedRealityTeleportHotSpot HotSpot { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem">Typically will be <see cref="EventSystem.current"/></param>
        public LocomotionEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="target"></param>
        public void Initialize(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot target)
        {
            BaseInitialize(pointer.InputSourceParent);
            Pointer = pointer;
            HotSpot = target;
        }
    }
}
