// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.InputSystem.Simulation;
using XRTK.Providers.Controllers;
using XRTK.Services;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class HandTrackingSimulationDataProvider : BaseControllerDataProvider, IMixedRealitySimulationDataProvider
    {
        private HandTrackingSimulationDataProviderProfile profile;
        private SimulatedHandDataProvider handDataProvider = null;
        private long lastHandDeviceUpdateTimeStamp = 0;
        private readonly Dictionary<Handedness, SimulatedHand> trackedHands = new Dictionary<Handedness, SimulatedHand>();

        /// <summary>
        /// Gets left hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();

        /// <summary>
        /// Gets right hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; private set; } = true;

        /// <summary>
        /// Dictionary to capture all active hands detected.
        /// </summary>
        public IReadOnlyDictionary<Handedness, SimulatedHand> TrackedHands => trackedHands;

        public HandTrackingSimulationDataProvider(string name, uint priority, HandTrackingSimulationDataProviderProfile profile)
            : base(name, priority, profile)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            ArticulatedHandPose.LoadGesturePoses(profile.GestureDefinitions);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (handDataProvider == null)
            {
                handDataProvider = new SimulatedHandDataProvider(profile);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            RemoveAllHandDevices();

            if (handDataProvider != null)
            {
                handDataProvider = null;
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (profile.SimulateHandTracking && UserInputEnabled)
            {
                handDataProvider.UpdateHandData(HandDataLeft, HandDataRight);
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (profile.SimulateHandTracking)
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandDeviceUpdateTimeStamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    if (HandDataLeft.Timestamp > lastHandDeviceUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Left, HandDataLeft);
                    }
                    if (HandDataRight.Timestamp > lastHandDeviceUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Right, HandDataRight);
                    }

                    lastHandDeviceUpdateTimeStamp = currentTime.Ticks;
                }
            }
        }

        // Register input sources for hands based on changes of the data provider
        private void UpdateHandInputSource(Handedness handedness, SimulatedHandData handData)
        {
            if (!profile.SimulateHandTracking)
            {
                RemoveAllHandDevices();
            }
            else
            {
                if (handData != null && handData.IsTracked)
                {
                    SimulatedHand controller = GetOrAddHandDevice(handedness);
                    controller.UpdateState(handData);
                }
                else
                {
                    RemoveHandDevice(handedness);
                }
            }
        }

        private SimulatedHand GetHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out SimulatedHand controller))
            {
                return controller;
            }
            return null;
        }

        private SimulatedHand GetOrAddHandDevice(Handedness handedness)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                return controller;
            }

            IMixedRealityPointer[] pointers = RequestPointers(typeof(SimulatedArticulatedHand), handedness);

            var inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);

            Type controllerType = typeof(SimulatedArticulatedHand);
            if (controller == null)
            {
                Debug.LogError($"Failed to create {controllerType} controller");
                return null;
            }

            if (!controller.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to Setup {controllerType} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            MixedRealityToolkit.InputSystem.RaiseSourceDetected(controller.InputSource, controller);

            trackedHands.Add(handedness, controller);
            UpdateActiveControllers();

            return controller;
        }

        private void RemoveHandDevice(Handedness handedness)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);

                trackedHands.Remove(handedness);
                UpdateActiveControllers();
            }
        }

        private void RemoveAllHandDevices()
        {
            foreach (var controller in trackedHands.Values)
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
            }

            trackedHands.Clear();
            UpdateActiveControllers();
        }

        private void UpdateActiveControllers()
        {
            //activeControllers = trackedHands.Values.ToArray<IMixedRealityController>();
        }
    }
}