// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Controllers.Hands;
using XRTK.Services;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Simulation
{
    /// <summary>
    /// Base data provider implementation for controller simulation data providers.
    /// </summary>
    public abstract class BaseSimulatedControllerDataProvider : BaseControllerDataProvider, ISimulatedControllerDataProvider
    {
        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public BaseSimulatedControllerDataProvider(string name, uint priority, SimulatedControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            if (profile == null)
            {
                throw new NullReferenceException($"A {nameof(SimulatedControllerDataProviderProfile)} is required for {name}");
            }

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

        private StopWatch simulatedUpdateStopWatch;
        private long lastSimulatedUpdateTimeStamp = 0;

        /// <summary>
        /// Internal read/write list of currently simulated controllers.
        /// </summary>
        protected List<BaseController> SimulatedControllers { get; } = new List<BaseController>();

        /// <summary>
        /// Gets or sets whether the left controller is currently set to be always visible
        /// no matter the simulated tracking state.
        /// </summary>
        protected bool LeftControllerIsAlwaysVisible { get; set; }

        /// <summary>
        /// Gets or sets whether the right controller is currently set to be always visible
        /// no matter the simulated tracking state.
        /// </summary>
        protected bool RightControllerIsAlwaysVisible { get; set; }

        /// <summary>
        /// Gets or sets whether the left controller is currently tracked by simulation.
        /// </summary>
        protected bool LeftControllerIsTracked { get; set; }

        /// <summary>
        /// Gets or sets whether the right controller is currently tracked by simulation.
        /// </summary>
        protected bool RightControllerIsTracked { get; set; }

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
        public new IReadOnlyList<BaseController> ActiveControllers => SimulatedControllers;

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            simulatedUpdateStopWatch = new StopWatch();
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
                    LeftControllerIsAlwaysVisible = !LeftControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(LeftControllerTrackedKey))
                {
                    LeftControllerIsTracked = true;
                }

                if (Input.GetKeyUp(LeftControllerTrackedKey))
                {
                    LeftControllerIsTracked = false;
                }

                if (LeftControllerIsAlwaysVisible || LeftControllerIsTracked)
                {
                    if (!TryGetController(Handedness.Left, out _))
                    {
                        CreateAndRegisterSimulatedController(Handedness.Left);
                    }
                }
                else
                {
                    RemoveController(Handedness.Left);
                }

                if (Input.GetKeyDown(ToggleRightPersistentKey))
                {
                    RightControllerIsAlwaysVisible = !RightControllerIsAlwaysVisible;
                }

                if (Input.GetKeyDown(RightControllerTrackedKey))
                {
                    RightControllerIsTracked = true;
                }

                if (Input.GetKeyUp(RightControllerTrackedKey))
                {
                    RightControllerIsTracked = false;
                }

                if (RightControllerIsAlwaysVisible || RightControllerIsTracked)
                {
                    if (!TryGetController(Handedness.Right, out _))
                    {
                        CreateAndRegisterSimulatedController(Handedness.Right);
                    }
                }
                else
                {
                    RemoveController(Handedness.Right);
                }

                lastSimulatedUpdateTimeStamp = currentTime.Ticks;
            }
        }

        /// <summary>
        /// Removes the simulated controller and unregisters it for a given hand, if it exists.
        /// </summary>
        /// <param name="handedness">Handedness of the controller to remove.</param>
        protected void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out var controller))
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                SimulatedControllers.Remove(controller);
            }
        }

        /// <summary>
        /// Removes and unregisters all currently active simulated controllers.
        /// </summary>
        protected void RemoveAllControllers()
        {
            while (ActiveControllers.Count > 0)
            {
                // It's important here to pass the handedness. Passing the controller
                // will execute the base RemoveController implementation.
                RemoveController(ActiveControllers[0].ControllerHandedness);
            }
        }

        /// <summary>
        /// Gets a simulated controller instance for a hand if it exists.
        /// </summary>
        /// <param name="handedness">Handedness to lookup.</param>
        /// <param name="controller">Controller instance if found.</param>
        /// <returns>True, if instance exists for given handedness.</returns>
        protected bool TryGetController(Handedness handedness, out BaseController controller)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                var existingController = ActiveControllers[i];

                if (existingController.ControllerHandedness == handedness)
                {
                    controller = existingController;
                    return true;
                }
            }

            controller = default;
            return false;
        }

        /// <summary>
        /// Asks the concrete simulation data create and register a new simulated controller.
        /// </summary>
        /// <param name="handedness">The handedness of the controller to create.</param>
        protected abstract void CreateAndRegisterSimulatedController(Handedness handedness);
    }
}