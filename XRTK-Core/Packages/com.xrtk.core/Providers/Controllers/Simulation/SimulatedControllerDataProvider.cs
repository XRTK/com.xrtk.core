// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers.Simulation.Hands;
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
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
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

        #region Controller Management

        private void RefreshSimulatedDevices()
        {
            // For now the only supported simulated controller type
            // are hand conttrollers. This code needs to be revisted,
            // once more simulated controller types, such as motion controllers are added.
            RefreshHandControllerDevices();
        }

        private IMixedRealityController GetOrAddController(Type controllerType, Handedness handedness)
        {
            if (TryGetController(handedness, out IMixedRealityController existingController))
            {
                return existingController;
            }

            IMixedRealityPointer[] pointers = RequestPointers(controllerType, handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{controllerType.Name} {handedness}", pointers);
            IMixedRealityController controller = (IMixedRealityController)Activator.CreateInstance(controllerType, TrackingState.Tracked, handedness, inputSource, null);

            if (controller == null || !controller.SetupConfiguration(controllerType))
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
                controller.TryRenderControllerModel(controllerType);
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

        #endregion

        #region Hand Controller Simulation

        private SimulationTimeStampStopWatch handUpdateStopWatch;
        private long lastHandUpdateTimeStamp = 0;

        private bool leftHandIsAlwaysVisible = false;
        private bool rightHandIsAlwaysVisible = false;
        private bool leftHandIsTracked = false;
        private bool rightHandIsTracked = false;

        private void RefreshHandControllerDevices()
        {
            DateTime currentTime = handUpdateStopWatch.Current;
            double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandUpdateTimeStamp)).TotalMilliseconds;
            if (msSinceLastHandUpdate > Profile.SimulatedUpdateFrequency)
            {
                if (Input.GetKeyDown(Profile.ToggleLeftHandKey))
                {
                    leftHandIsAlwaysVisible = !leftHandIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.LeftHandTrackedKey))
                {
                    leftHandIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.LeftHandTrackedKey))
                {
                    leftHandIsTracked = false;
                }

                if (leftHandIsAlwaysVisible || leftHandIsTracked)
                {
                    GetOrAddController(typeof(SimulatedHandController), Handedness.Left);
                }
                else
                {
                    RemoveController(Handedness.Left);
                }

                if (Input.GetKeyDown(Profile.ToggleRightHandKey))
                {
                    rightHandIsAlwaysVisible = !rightHandIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.RightHandTrackedKey))
                {
                    rightHandIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.RightHandTrackedKey))
                {
                    rightHandIsTracked = false;
                }

                if (rightHandIsAlwaysVisible || rightHandIsTracked)
                {
                    GetOrAddController(typeof(SimulatedHandController), Handedness.Right);
                }
                else
                {
                    RemoveController(Handedness.Right);
                }

                lastHandUpdateTimeStamp = currentTime.Ticks;
            }
        }

        #endregion
    }
}