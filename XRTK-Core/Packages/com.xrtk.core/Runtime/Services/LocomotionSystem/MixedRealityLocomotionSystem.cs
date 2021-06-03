// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using XRTK.Extensions;
using XRTK.Utilities;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityLocomotionSystem"/>
    /// </summary>
    [System.Runtime.InteropServices.Guid("9453c088-285e-47aa-bfbb-dafd9109fdd5")]
    public class MixedRealityLocomotionSystem : BaseEventSystem, IMixedRealityLocomotionSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The active <see cref="MixedRealityLocomotionSystemProfile"/>.</param>
        public MixedRealityLocomotionSystem(MixedRealityLocomotionSystemProfile profile)
            : base(profile)
        {
            TeleportAction = profile.TeleportAction;
            MovementCancelsTeleport = profile.MovementCancelsTeleport;
            TeleportationIsEnabled = profile.TeleportStartupBehaviour == AutoStartBehavior.AutoStart;
            LocomotionIsEnabled = profile.MovementStartupBehaviour == AutoStartBehavior.AutoStart;
        }

        private LocomotionEventData teleportEventData;
        private bool isTeleporting = false;

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityLocomotionProvider> EnabledLocomotionProviders => null;

        /// <inheritdoc />
        public bool TeleportationIsEnabled { get; set; }

        /// <inheritdoc />
        public bool LocomotionIsEnabled { get; set; }

        /// <inheritdoc />
        public MixedRealityInputAction TeleportAction { get; private set; }

        /// <summary>
        /// If set, movement will cancel any teleportation in progress.
        /// </summary>
        public bool MovementCancelsTeleport { get; private set; }

        /// <summary>
        /// Gets the currently active locomotion target override, if any.
        /// </summary>
        public LocomotionTargetOverride LocomotionTargetOverride { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                teleportEventData = new LocomotionEventData(EventSystem.current);
            }

            CameraCache.Main.gameObject.EnsureComponent<LocomotionProvider>();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (!Application.isPlaying)
            {
                var component = CameraCache.Main.gameObject.GetComponent(typeof(LocomotionProvider));
                if (!component.IsNull())
                {
                    component.Destroy();
                }
            }

            base.Destroy();
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (isTeleporting)
            {
                Debug.LogError("Teleportation already in progress");
                return;
            }

            isTeleporting = true;
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportStartedHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (!isTeleporting)
            {
                Debug.LogError("No Active Teleportation in progress.");
                return;
            }

            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCompletedHandler);
            isTeleporting = false;
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);
            isTeleporting = false;
        }

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var teleportData = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            base.HandleEvent(teleportData, eventHandler);
        }
    }
}
