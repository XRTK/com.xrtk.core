// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation
{
    /// <summary>
    /// The simulated controller data provider is mainly responsible for managing
    /// active simulated controllers, such as hand controllers. All active controllers
    /// at a given time are accessible via <see cref="BaseControllerDataProvider.ActiveControllers"/>.
    /// </summary>
    public class SimulatedControllerDataProvider : BaseControllerDataProvider
    {
        private SimulationTimeStampStopWatch simulatedUpdateStopWatch;
        private long lastSimulatedUpdateTimeStamp = 0;

        private bool leftControllerIsAlwaysVisible = false;
        private bool rightControllerIsAlwaysVisible = false;
        private bool leftControllerIsTracked = false;
        private bool rightControllerIsTracked = false;

        /// <summary>
        /// Gets the active profile for the simulated controller data provider.
        /// </summary>
        public SimulatedControllerDataProviderProfile Profile { get; }

        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public SimulatedControllerDataProvider(string name, uint priority, SimulatedControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            Profile = profile;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            // Start the timestamp stopwatch
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
            DateTime currentTime = simulatedUpdateStopWatch.Current;
            double msSinceLastUpdate = currentTime.Subtract(new DateTime(lastSimulatedUpdateTimeStamp)).TotalMilliseconds;
            if (msSinceLastUpdate > Profile.SimulatedUpdateFrequency)
            {
                if (Input.GetKeyDown(Profile.ToggleLeftPersistentKey))
                {
                    leftControllerIsAlwaysVisible = !leftControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.LeftControllerTrackedKey))
                {
                    leftControllerIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.LeftControllerTrackedKey))
                {
                    leftControllerIsTracked = false;
                }

                if (leftControllerIsAlwaysVisible || leftControllerIsTracked)
                {
                    GetOrAddController(Handedness.Left);
                }
                else
                {
                    RemoveController(Handedness.Left);
                }

                if (Input.GetKeyDown(Profile.ToggleRightPersistentKey))
                {
                    rightControllerIsAlwaysVisible = !rightControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.RightControllerTrackedKey))
                {
                    rightControllerIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.RightControllerTrackedKey))
                {
                    rightControllerIsTracked = false;
                }

                if (rightControllerIsAlwaysVisible || rightControllerIsTracked)
                {
                    GetOrAddController(Handedness.Right);
                }
                else
                {
                    RemoveController(Handedness.Right);
                }

                lastSimulatedUpdateTimeStamp = currentTime.Ticks;
            }
        }

        private IMixedRealityController GetOrAddController(Handedness handedness)
        {
            if (TryGetController(handedness, out IMixedRealityController existingController))
            {
                return existingController;
            }

            IMixedRealityPointer[] pointers = RequestPointers(Profile.SimulatedControllerType, handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{Profile.SimulatedControllerType.Type.Name} {handedness}", pointers);
            IMixedRealityController controller = (IMixedRealityController)Activator.CreateInstance(Profile.SimulatedControllerType, TrackingState.Tracked, handedness, inputSource, null);

            if (controller == null || !controller.SetupConfiguration(Profile.SimulatedControllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            MixedRealityToolkit.InputSystem.RaiseSourceDetected(controller.InputSource, controller);
            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers)
            {
                controller.TryRenderControllerModel(Profile.SimulatedControllerType);
            }

            AddController(controller);
            return controller as IMixedRealityHandController;
        }

        private void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out IMixedRealityController controller))
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
                RemoveController(controller);
            }
        }

        private void RemoveAllControllers()
        {
            while (ActiveControllers.Count > 0)
            {
                RemoveController(ActiveControllers[0].ControllerHandedness);
            }
        }

        private bool TryGetController(Handedness handedness, out IMixedRealityController controller)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                IMixedRealityController existingController = ActiveControllers[i];
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