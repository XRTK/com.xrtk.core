// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Simulation
{
    /// <summary>
    /// Base data provider implementation for controller simulation data providers.
    /// </summary>
    public abstract class BaseSimulatedControllerDataProvider : BaseControllerDataProvider, ISimulatedControllerDataProvider
    {
        /// <inheritdoc />
        protected BaseSimulatedControllerDataProvider(string name, uint priority, SimulatedControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
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
                UpdateSimulatedController((IMixedRealitySimulatedController)ActiveControllers[i]);
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
        protected virtual void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out var controller))
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                RemoveController(controller);
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
        protected bool TryGetController(Handedness handedness, out IMixedRealityController controller)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                if (ActiveControllers[i].ControllerHandedness == handedness)
                {
                    controller = ActiveControllers[i];
                    return true;
                }
            }

            controller = default;
            return false;
        }

        /// <summary>
        /// Updates the provided simulated controller instance.
        /// </summary>
        /// <param name="simulatedController">Controller to update.</param>
        protected virtual void UpdateSimulatedController(IMixedRealitySimulatedController simulatedController)
        {
            simulatedController.UpdateController();
        }

        /// <summary>
        /// Asks the concrete simulation data create and register a new simulated controller.
        /// </summary>
        /// <param name="handedness">The handedness of the controller to create.</param>
        protected abstract IMixedRealitySimulatedController CreateAndRegisterSimulatedController(Handedness handedness);
    }
}
