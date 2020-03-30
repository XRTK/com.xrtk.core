// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation
{
    /// <summary>
    /// The simulated controller data provider is mainly responsible for managing
    /// active simulated controllers, such as hand controllers.
    /// </summary>
    public class SimulatedControllerDataProvider : BaseControllerDataProvider, ISimulatedControllerDataProvider
    {
        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public SimulatedControllerDataProvider(string name, uint priority, SimulatedControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            if (profile == null)
            {
                throw new NullReferenceException($"A {nameof(SimulatedControllerDataProviderProfile)} is required for {name}");
            }

            simulatedControllerType = profile.SimulatedControllerType?.Type ?? throw new NullReferenceException($"{nameof(SimulatedControllerDataProviderProfile)}.{nameof(SimulatedControllerDataProviderProfile.SimulatedControllerType)} cannot be null.");
            SimulatedUpdateFrequency = profile.SimulatedUpdateFrequency;
            ControllerHideTimeout = profile.ControllerHideTimeout;
            DefaultDistance = profile.DefaultDistance;
            DepthMultiplier = profile.DepthMultiplier;
            JitterAmount = profile.JitterAmount;
            ToggleLeftPersistentKey = profile.ToggleLeftPersistentKey;
            LeftControllerTrackedKey = profile.LeftControllerTrackedKey;
            ToggleRightPersistentKey = profile.ToggleRightPersistentKey;
            RightControllerTrackedKey = profile.RightControllerTrackedKey;
            RotationSpeed = profile.RotationSpeed;
        }

        private readonly Type simulatedControllerType;
        private readonly List<BaseController> activeControllers = new List<BaseController>();

        private SimulationTimeStampStopWatch simulatedUpdateStopWatch;
        private long lastSimulatedUpdateTimeStamp = 0;

        private bool leftControllerIsAlwaysVisible = false;
        private bool rightControllerIsAlwaysVisible = false;
        private bool leftControllerIsTracked = false;
        private bool rightControllerIsTracked = false;

        /// <inheritdoc />
        public double SimulatedUpdateFrequency { get; }

        /// <inheritdoc />
        public float ControllerHideTimeout { get; }

        /// <inheritdoc />
        public float DefaultDistance { get; }

        /// <inheritdoc />
        public float DepthMultiplier { get; }

        /// <inheritdoc />
        public float JitterAmount { get; }

        /// <inheritdoc />
        public KeyCode ToggleLeftPersistentKey { get; }

        /// <inheritdoc />
        public KeyCode LeftControllerTrackedKey { get; }

        /// <inheritdoc />
        public KeyCode ToggleRightPersistentKey { get; }

        /// <inheritdoc />
        public KeyCode RightControllerTrackedKey { get; }

        /// <inheritdoc />
        public float RotationSpeed { get; }

        /// <summary>
        /// Gets a read only list of active controllers. This property hides the inherited
        /// active controllers property.
        /// </summary>
        /// <remarks>Subject to change, once the new controller refactorings are in place.</remarks>
        public new IReadOnlyList<BaseController> ActiveControllers => activeControllers;

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            simulatedUpdateStopWatch = new SimulationTimeStampStopWatch();
            simulatedUpdateStopWatch.Reset();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            RefreshSimulatedDevices();

            // Update all active simulated controllers.
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                ActiveControllers[i].UpdateController();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            RemoveAllControllers();
            base.Disable();
        }

        private void RefreshSimulatedDevices()
        {
            var currentTime = simulatedUpdateStopWatch.Current;
            var msSinceLastUpdate = currentTime.Subtract(new DateTime(lastSimulatedUpdateTimeStamp)).TotalMilliseconds;

            if (msSinceLastUpdate > SimulatedUpdateFrequency)
            {
                if (Input.GetKeyDown(ToggleLeftPersistentKey))
                {
                    leftControllerIsAlwaysVisible = !leftControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(LeftControllerTrackedKey))
                {
                    leftControllerIsTracked = true;
                }

                if (Input.GetKeyUp(LeftControllerTrackedKey))
                {
                    leftControllerIsTracked = false;
                }

                if (leftControllerIsAlwaysVisible || leftControllerIsTracked)
                {
                    CreateControllerIfNotExists(Handedness.Left);
                }
                else
                {
                    RemoveController(Handedness.Left);
                }

                if (Input.GetKeyDown(ToggleRightPersistentKey))
                {
                    rightControllerIsAlwaysVisible = !rightControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(RightControllerTrackedKey))
                {
                    rightControllerIsTracked = true;
                }

                if (Input.GetKeyUp(RightControllerTrackedKey))
                {
                    rightControllerIsTracked = false;
                }

                if (rightControllerIsAlwaysVisible || rightControllerIsTracked)
                {
                    CreateControllerIfNotExists(Handedness.Right);
                }
                else
                {
                    RemoveController(Handedness.Right);
                }

                lastSimulatedUpdateTimeStamp = currentTime.Ticks;
            }
        }

        private void CreateControllerIfNotExists(Handedness handedness)
        {
            if (TryGetController(handedness, out _))
            {
                return;
            }

            var pointers = RequestPointers(simulatedControllerType, handedness, true);
            var inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{simulatedControllerType.Name} {handedness}", pointers);
            var controller = (BaseController)Activator.CreateInstance(simulatedControllerType, TrackingState.Tracked, handedness, inputSource, null);

            if (controller == null || !controller.SetupConfiguration(simulatedControllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers)
            {
                controller.TryRenderControllerModel(simulatedControllerType);
            }

            MixedRealityToolkit.InputSystem.RaiseSourceDetected(controller.InputSource, controller);
            activeControllers.Add(controller);
        }

        private void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out BaseController controller))
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
                activeControllers.Remove(controller);
            }
        }

        private void RemoveAllControllers()
        {
            while (ActiveControllers.Count > 0)
            {
                // It's important here to pass the handedness. Passing the controller
                // will execute the base RemoveController implementation.
                RemoveController(ActiveControllers[0].ControllerHandedness);
            }
        }

        private bool TryGetController(Handedness handedness, out BaseController controller)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                BaseController existingController = ActiveControllers[i];
                if (existingController.ControllerHandedness == handedness)
                {
                    controller = existingController;
                    return true;
                }
            }

            controller = default;
            return false;
        }
    }
}