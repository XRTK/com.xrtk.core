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
            teleportCooldown = profile.TeleportCooldown;
        }

        private readonly float teleportCooldown;
        private float currentTeleportCooldown;
        private LocomotionEventData teleportEventData;
        private readonly Dictionary<Type, List<ILocomotionProvider>> enabledLocomotionProviders = new Dictionary<Type, List<ILocomotionProvider>>()
        {
            { typeof(IFreeLocomotionProvider), new List<ILocomotionProvider>() },
            { typeof(ITeleportLocomotionProvider), new List<ILocomotionProvider>() },
            { typeof(IOnRailsLocomotionProvider), new List<ILocomotionProvider>() }
        };

        /// <inheritdoc />
        public IReadOnlyList<ILocomotionProvider> EnabledLocomotionProviders => enabledLocomotionProviders.SelectMany(kv => kv.Value).ToList();

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

            if (currentTeleportCooldown > 0f)
            {
                currentTeleportCooldown -= Time.deltaTime;
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
        public void EnableLocomotionProvider<T>() where T : ILocomotionProvider
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
            Debug.Assert(typeof(ILocomotionProvider).IsAssignableFrom(locomotionProviderType));
            var locomotionProviders = MixedRealityToolkit.GetActiveServices<ILocomotionProvider>();

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
        public void DisableLocomotionProvider<T>() where T : ILocomotionProvider
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
            Debug.Assert(typeof(ILocomotionProvider).IsAssignableFrom(locomotionProviderType));
            var locomotionProviders = MixedRealityToolkit.GetActiveServices<ILocomotionProvider>();

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
        public void OnLocomotionProviderEnabled(ILocomotionProvider locomotionProvider)
        {
            var enabledLocomotionProvidersSnapshot = new Dictionary<Type, List<ILocomotionProvider>>(enabledLocomotionProviders);

            if (locomotionProvider is IOnRailsLocomotionProvider)
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
                if (!enabledLocomotionProvidersSnapshot[typeof(IOnRailsLocomotionProvider)].Contains(locomotionProvider))
                {
                    enabledLocomotionProviders[typeof(IOnRailsLocomotionProvider)].Add(locomotionProvider);
                }
            }
            else if (locomotionProvider is ITeleportLocomotionProvider ||
                locomotionProvider is IFreeLocomotionProvider)
            {
                // Free / teleport locomotion excludes on rails locomotion,
                // disable any active on rails locomotion providers.
                var onRailsLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(IOnRailsLocomotionProvider)];
                for (var i = 0; i < onRailsLocomotionProviders.Count; i++)
                {
                    onRailsLocomotionProviders[i].Disable();
                }

                // Free / Teleport providers behave like a commong toggle group. There can only
                // ever be one active provider for free locomotion and one active for teleprot locomotion.
                // So all we have to do is disable all other providers of the respective type.
                if (locomotionProvider is ITeleportLocomotionProvider)
                {
                    var teleportLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(ITeleportLocomotionProvider)];
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
                        enabledLocomotionProviders[typeof(ITeleportLocomotionProvider)].Add(locomotionProvider);
                    }
                }
                else
                {
                    var freeLocomotionProviders = enabledLocomotionProvidersSnapshot[typeof(IFreeLocomotionProvider)];
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
                        enabledLocomotionProviders[typeof(IFreeLocomotionProvider)].Add(locomotionProvider);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnLocomotionProviderDisabled(ILocomotionProvider locomotionProvider)
        {
            var type = locomotionProvider.GetType();
            if (enabledLocomotionProviders.ContainsKey(type) &&
                enabledLocomotionProviders[type].Contains(locomotionProvider))
            {
                enabledLocomotionProviders[type].Remove(locomotionProvider);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionSystemHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityLocomotionSystemHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnTeleportTargetRequested(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportTargetRequest(ITeleportLocomotionProvider teleportLocomotionProvider, IMixedRealityInputSource inputSource)
        {
            teleportEventData.Initialize(teleportLocomotionProvider, inputSource);
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionSystemHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityLocomotionSystemHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnTeleportStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportStarted(ILocomotionProvider locomotionProvider, IMixedRealityPointer pointer, ITeleportHotSpot hotSpot)
        {
            if (currentTeleportCooldown > 0f)
            {
                return;
            }

            teleportEventData.Initialize(locomotionProvider, pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportStartedHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionSystemHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityLocomotionSystemHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnTeleportCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCompleted(ILocomotionProvider locomotionProvider, IMixedRealityPointer pointer, ITeleportHotSpot hotSpot)
        {
            currentTeleportCooldown = teleportCooldown;
            teleportEventData.Initialize(locomotionProvider, pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCompletedHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionSystemHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityLocomotionSystemHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnTeleportCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCanceled(ILocomotionProvider locomotionProvider, IMixedRealityPointer pointer, ITeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(locomotionProvider, pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);
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
