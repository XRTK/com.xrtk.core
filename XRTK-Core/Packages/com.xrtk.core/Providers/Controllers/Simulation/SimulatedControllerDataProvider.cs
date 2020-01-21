// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation
{
    /// <summary>
    /// The simulated controller data provider is mainly responsible for managing
    /// active simulated controllers, such as hand controllers.
    /// </summary>
    public class SimulatedControllerDataProvider : BaseControllerDataProvider
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
            this.profile = profile;
        }

        private readonly SimulatedControllerDataProviderProfile profile;
        private readonly List<BaseController> activeControllers = new List<BaseController>();
        private SimulationTimeStampStopWatch simulatedUpdateStopWatch;
        private long lastSimulatedUpdateTimeStamp = 0;

        private bool leftControllerIsAlwaysVisible = false;
        private bool rightControllerIsAlwaysVisible = false;
        private bool leftControllerIsTracked = false;
        private bool rightControllerIsTracked = false;

        /// <summary>
        /// Gets configured simulated hand controller pose definitions used to simulate
        /// different hand poses.
        /// </summary>
        public IReadOnlyList<SimulatedHandControllerPoseData> HandPoseDefinitions => profile.PoseDefinitions;

        /// <summary>
        /// Gets the simulated hand controller pose animation speed controlling
        /// how fast the hand will translate from one pose to another.
        /// </summary>
        public float HandPoseAnimationSpeed => profile.HandPoseAnimationSpeed;

        /// <summary>
        /// Gets the hand depth change multiplier used to simulate controller depth movement.
        /// </summary>
        public float HandDepthMultiplier => profile.HandDepthMultiplier;

        /// <summary>
        /// Gets the amount of simulated jitter offset for simulated controllers.
        /// </summary>
        public float JitterAmount => profile.JitterAmount;

        /// <summary>
        /// Gets the rotation speed for simulated hand controller pitch/yaw/roll animation.
        /// </summary>
        public float RotationSpeed => profile.RotationSpeed;

        /// <summary>
        /// Gets the default distance fom the camera to spawn simulated controllers at.
        /// </summary>
        public float DefaultDistance => profile.DefaultDistance;

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
            if (msSinceLastUpdate > profile.SimulatedUpdateFrequency)
            {
                if (Input.GetKeyDown(profile.ToggleLeftPersistentKey))
                {
                    leftControllerIsAlwaysVisible = !leftControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(profile.LeftControllerTrackedKey))
                {
                    leftControllerIsTracked = true;
                }

                if (Input.GetKeyUp(profile.LeftControllerTrackedKey))
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

                if (Input.GetKeyDown(profile.ToggleRightPersistentKey))
                {
                    rightControllerIsAlwaysVisible = !rightControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(profile.RightControllerTrackedKey))
                {
                    rightControllerIsTracked = true;
                }

                if (Input.GetKeyUp(profile.RightControllerTrackedKey))
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

            IMixedRealityPointer[] pointers = RequestPointers(profile.SimulatedControllerType, handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{profile.SimulatedControllerType.Type.Name} {handedness}", pointers);
            IMixedRealityController controller = (IMixedRealityController)Activator.CreateInstance(profile.SimulatedControllerType, TrackingState.Tracked, handedness, inputSource, null);

            if (controller == null || !controller.SetupConfiguration(profile.SimulatedControllerType))
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
                controller.TryRenderControllerModel(profile.SimulatedControllerType);
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
                // It's important here to pass the handedness. Passing the controller
                // will execute the base RmoveController implementation, which will remove
                // the controller but not RaiseSourceLost.
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