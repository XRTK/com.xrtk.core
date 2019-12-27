﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation for all hand controller data providers. Takes care of all the platform agnostic
    /// hand tracking logic.
    /// </summary>
    public abstract class BaseHandControllerDataProvider<T> : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider where T : IMixedRealityHandController
    {
        private InputEventData<HandData> handDataUpdateEventData;
        private readonly List<IMixedRealityHandDataHandler> handDataUpdateEventHandlers = new List<IMixedRealityHandDataHandler>();

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Hand controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public BaseHandControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                handDataUpdateEventData = new InputEventData<HandData>(EventSystem.current);
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            // Execute the platform specific implementation to get active hands.
            RefreshActiveControllers();

            // Update all tracked hands (controllers).
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

        /// <inheritdoc />
        public bool TryGetJointPose(Handedness handedness, TrackedHandJoint joint, out MixedRealityPose pose)
        {
            if (TryGetController(handedness, out T controller))
            {
                return controller.TryGetJointPose(joint, out pose);
            }

            pose = default;
            return false;
        }

        /// <inheritdoc />
        public bool IsHandTracked(Handedness handedness)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                if (ActiveControllers[i].ControllerHandedness == handedness)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a controller instance of the data providers type if it already exists
        /// or creates and registers a new one if not.
        /// </summary>
        /// <param name="handedness">The handedness to assign to the controller.</param>
        /// <returns>Controller instance.</returns>
        public IMixedRealityHandController GetOrAddController(Handedness handedness)
        {
            if (TryGetController(handedness, out T existingController))
            {
                return existingController;
            }

            IMixedRealityPointer[] pointers = RequestPointers(typeof(T), handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            T controller = (T)System.Activator.CreateInstance(typeof(T), TrackingState.Tracked, handedness, inputSource, null);

            if (controller == null || !controller.SetupConfiguration(typeof(T)))
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
                controller.TryRenderControllerModel(typeof(T));
            }

            AddController(controller);
            return controller as IMixedRealityHandController;
        }

        /// <summary>
        /// Removes the controller for a given handedness.
        /// </summary>
        /// <param name="handedness">The handness of the controller to remove.</param>
        protected void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out T controller))
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
                RemoveController(controller);
            }
        }

        private void RemoveAllControllers()
        {
            while (ActiveControllers.Count > 0)
            {
                RemoveController(ActiveControllers[0]);
            }
        }

        private bool TryGetController(Handedness handedness, out T controller)
        {
            for (int i = 0; i < ActiveControllers.Count; i++)
            {
                IMixedRealityController existingController = ActiveControllers[i];
                if (existingController.ControllerHandedness == handedness)
                {
                    controller = (T)existingController;
                    return true;
                }
            }

            controller = default;
            return false;
        }

        /// <inheritdoc />
        public void Register(IMixedRealityHandDataHandler handler)
        {
            if (handler != null)
            {
                handDataUpdateEventHandlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityHandDataHandler handler)
        {
            if (handDataUpdateEventHandlers.Contains(handler))
            {
                handDataUpdateEventHandlers.Remove(handler);
            }
        }

        /// <summary>
        /// Refreshes the active devices for this data provider.
        /// </summary>
        protected abstract void RefreshActiveControllers();

        /// <inheritdoc />
        public void UpdateHandData(Handedness handedness, HandData handData)
        {
            if (handData != null && handData.IsTracked)
            {
                IMixedRealityHandController controller = GetOrAddController(handedness);
                if (controller != null)
                {
                    handDataUpdateEventData.Initialize(controller.InputSource, handedness, MixedRealityInputAction.None, handData);
                    for (int i = 0; i < handDataUpdateEventHandlers.Count; i++)
                    {
                        IMixedRealityHandDataHandler handler = handDataUpdateEventHandlers[i];
                        handler.OnHandDataUpdated(handDataUpdateEventData);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to create {controller.GetType().Name} controller");
                }
            }
            else
            {
                RemoveController(handedness);
            }
        }
    }
}
