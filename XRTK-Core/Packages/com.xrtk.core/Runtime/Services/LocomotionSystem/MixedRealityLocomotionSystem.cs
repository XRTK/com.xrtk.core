// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEngine;
using XRTK.Extensions;
using XRTK.Utilities;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using XRTK.Definitions.LocomotionSystem;
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
            MovementCancelsTeleport = profile.MovementCancelsTeleport;
        }

        private LocomotionEventData teleportEventData;
        private bool isTeleporting = false;
        private readonly Dictionary<Type, List<IMixedRealityLocomotionProvider>> enabledLocomotionProviders = new Dictionary<Type, List<IMixedRealityLocomotionProvider>>()
        {
            { typeof(IMixedRealityFreeLocomotionProvider), new List<IMixedRealityLocomotionProvider>() },
            { typeof(IMixedRealityTeleportLocomotionProvider), new List<IMixedRealityLocomotionProvider>() },
            { typeof(IMixedRealityOnRailsLocomotionProvider), new List<IMixedRealityLocomotionProvider>() }
        };

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityLocomotionProvider> EnabledLocomotionProviders => enabledLocomotionProviders.SelectMany(kv => kv.Value).ToList();

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

            CameraCache.Main.gameObject.EnsureComponent<LocomotionProviderEventDriver>();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            CameraCache.Main.gameObject.EnsureComponent<LocomotionProviderEventDriver>().enabled = true;
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EnableLocomotionProvider<Providers.LocomotionSystem.BlinkTeleportLocomotionProvider>();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EnableLocomotionProvider<Providers.LocomotionSystem.DashTeleportLocomotionProvider>();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EnableLocomotionProvider<Providers.LocomotionSystem.InstantTeleportLocomotionProvider>();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                EnableLocomotionProvider<Providers.LocomotionSystem.TransformPathOnRailsLocomotionProvider>();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            CameraCache.Main.EnsureComponent<LocomotionProviderEventDriver>().enabled = false;
            base.Disable();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            CameraCache.Main.gameObject.EnsureComponentDestroyed<LocomotionProviderEventDriver>();
            base.Destroy();
        }

        /// <inheritdoc />
        public void EnableLocomotionProvider<T>() where T : IMixedRealityLocomotionProvider
        {
            var provider = MixedRealityToolkit.GetService<T>();
            if (!provider.IsEnabled)
            {
                provider.Enable();
            }
        }

        /// <inheritdoc />
        public void EnableLocomotionProvider(Type locomotionProviderType)
        {
            Debug.Assert(typeof(IMixedRealityLocomotionProvider).IsAssignableFrom(locomotionProviderType));
            var locomotionProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityLocomotionProvider>();

            for (var i = 0; i < locomotionProviders.Count; i++)
            {
                var provider = locomotionProviders[i];
                if (provider.GetType() == locomotionProviderType && !provider.IsEnabled)
                {
                    provider.Enable();
                }
            }
        }

        /// <inheritdoc />
        public void DisableLocomotionProvider<T>() where T : IMixedRealityLocomotionProvider
        {
            var provider = MixedRealityToolkit.GetService<T>();
            if (provider.IsEnabled)
            {
                provider.Disable();
            }
        }

        /// <inheritdoc />
        public void DisableLocomotionProvider(Type locomotionProviderType)
        {
            Debug.Assert(typeof(IMixedRealityLocomotionProvider).IsAssignableFrom(locomotionProviderType));
            var locomotionProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityLocomotionProvider>();

            for (var i = 0; i < locomotionProviders.Count; i++)
            {
                var provider = locomotionProviders[i];
                if (provider.GetType() == locomotionProviderType && provider.IsEnabled)
                {
                    provider.Disable();
                }
            }
        }

        /// <inheritdoc />
        public void OnLocomotionProviderEnabled(IMixedRealityLocomotionProvider locomotionProvider)
        {
            var enabledLocomotionProvidersSnapshot = new Dictionary<Type, List<IMixedRealityLocomotionProvider>>(enabledLocomotionProviders);

            if (locomotionProvider is IMixedRealityOnRailsLocomotionProvider)
            {
                // On rails locomotion providers are exclusive, meaning whenever an on rails
                // provider is enabled, any other providers must be disabled.
                foreach (var typeList in enabledLocomotionProvidersSnapshot)
                {
                    for (var i = 0; i < typeList.Value.Count; i++)
                    {
                        var provider = typeList.Value[i];

                        // Making sure to not disable the provider that just got enabled,
                        // in case it is already in the list.
                        if (provider != locomotionProvider)
                        {
                            provider.Disable();
                        }
                    }
                }

                // Ensure the now enabled provider gets added to the managed enabled
                // providers list.
                if (!enabledLocomotionProvidersSnapshot[typeof(IMixedRealityOnRailsLocomotionProvider)].Contains(locomotionProvider))
                {
                    enabledLocomotionProviders[typeof(IMixedRealityOnRailsLocomotionProvider)].Add(locomotionProvider);
                }
            }
            else if (locomotionProvider is IMixedRealityTeleportLocomotionProvider ||
                locomotionProvider is IMixedRealityFreeLocomotionProvider)
            {
                // Free / teleport locomotion excludes on rails locomotion,
                // disable any active on rails locomotion providers.
                var onRailsLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(IMixedRealityOnRailsLocomotionProvider)];
                for (var i = 0; i < onRailsLocomotionProviders.Count; i++)
                {
                    onRailsLocomotionProviders[i].Disable();
                }

                // Free / Teleport providers behave like a commong toggle group. There can only
                // ever be one active provider for free locomotion and one active for teleprot locomotion.
                // So all we have to do is disable all other providers of the respective type.
                if (locomotionProvider is IMixedRealityTeleportLocomotionProvider)
                {
                    var teleportLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(IMixedRealityTeleportLocomotionProvider)];
                    for (var i = 0; i < teleportLocomotionProviders.Count; i++)
                    {
                        var teleportLocomotionProvider = teleportLocomotionProviders[i];

                        // Making sure to not disable the provider that just got enabled,
                        // in case it is already in the list.
                        if (teleportLocomotionProvider != locomotionProvider)
                        {
                            teleportLocomotionProvider.Disable();
                        }
                    }

                    // Ensure the now enabled provider gets added to the managed enabled
                    // providers list.
                    if (!teleportLocomotionProviders.Contains(locomotionProvider))
                    {
                        enabledLocomotionProviders[typeof(IMixedRealityTeleportLocomotionProvider)].Add(locomotionProvider);
                    }
                }
                else
                {
                    var freeLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(IMixedRealityFreeLocomotionProvider)];
                    for (var i = 0; i < freeLocomotionProviders.Count; i++)
                    {
                        var freeLocomotionProvider = freeLocomotionProviders[i];

                        // Making sure to not disable the provider that just got enabled,
                        // in case it is already in the list.
                        if (freeLocomotionProvider != locomotionProvider)
                        {
                            freeLocomotionProvider.Disable();
                        }
                    }

                    // Ensure the now enabled provider gets added to the managed enabled
                    // providers list.
                    if (!freeLocomotionProviders.Contains(locomotionProvider))
                    {
                        enabledLocomotionProviders[typeof(IMixedRealityFreeLocomotionProvider)].Add(locomotionProvider);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnLocomotionProviderDisabled(IMixedRealityLocomotionProvider locomotionProvider)
        {
            var type = locomotionProvider.GetType();
            if (enabledLocomotionProviders.ContainsKey(type) &&
                enabledLocomotionProviders[type].Contains(locomotionProvider))
            {
                enabledLocomotionProviders[type].Remove(locomotionProvider);
            }
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
