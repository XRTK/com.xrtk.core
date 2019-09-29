// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// Manages joysticks using unity input system.
    /// </summary>
    public class UnityJoystickDataProvider : BaseControllerDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public UnityJoystickDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        private const float DeviceRefreshInterval = 3.0f;

        protected static readonly Dictionary<string, GenericJoystickController> ActiveGenericControllers = new Dictionary<string, GenericJoystickController>();

        private float deviceRefreshTimer;
        private string[] lastDeviceList;

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {
                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }

            foreach (var controller in ActiveGenericControllers)
            {
                controller.Value?.UpdateController();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            foreach (var genericOpenVRController in ActiveGenericControllers)
            {
                RemoveController(genericOpenVRController.Key, false);
            }

            ActiveGenericControllers.Clear();
        }

        private void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            if (lastDeviceList != null && joystickNames.Length == lastDeviceList.Length)
            {
                for (int i = 0; i < lastDeviceList.Length; i++)
                {
                    var joystickName = lastDeviceList[i];

                    if (joystickNames[i].Equals(joystickName)) { continue; }

                    RemoveController(joystickName);
                }
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                var name = joystickNames[i];

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (!ActiveGenericControllers.ContainsKey(name))
                {
                    var controller = GetOrAddController(name);

                    if (controller != null)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                    }
                }
            }

            lastDeviceList = joystickNames;
        }

        /// <summary>
        /// Gets or adds a controller using the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of they joystick from Unity's <see cref="Input.GetJoystickNames"/></param>
        /// <returns>A new controller reference.</returns>
        protected virtual GenericJoystickController GetOrAddController(string joystickName)
        {
            if (ActiveGenericControllers.ContainsKey(joystickName))
            {
                var controller = ActiveGenericControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            Type controllerType;

            switch (GetCurrentControllerType(joystickName))
            {
                default:
                    return null;
                case SupportedControllerType.GenericUnity:
                    controllerType = typeof(GenericJoystickController);
                    break;
                case SupportedControllerType.Xbox:
                    controllerType = typeof(XboxController);
                    break;
            }

            var inputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource($"{controllerType.Name} Controller");
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, Handedness.None, inputSource, null) as GenericJoystickController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                Debug.LogError($"Failed to configure {controllerType.Name} controller!");
                return null;
            }

            ActiveGenericControllers.Add(joystickName, detectedController);
            AddController(detectedController);
            return detectedController;
        }

        protected virtual void RemoveController(string joystickName, bool clearFromRegistry = true)
        {
            var controller = GetOrAddController(joystickName);

            if (controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                RemoveController(controller);
            }

            if (clearFromRegistry)
            {
                ActiveGenericControllers.Remove(joystickName);
            }
        }

        /// <summary>
        /// Gets the current controller type for the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of they joystick from Unity's <see cref="Input.GetJoystickNames"/></param>
        /// <returns>The supported controller type</returns>
        protected virtual SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName) ||
                joystickName.Contains("OpenVR") ||
                joystickName.Contains("Spatial"))
            {
                return SupportedControllerType.None;
            }

            if (joystickName.Contains("Xbox Controller") ||
                joystickName.Contains("Xbox One For Windows") ||
                joystickName.Contains("Xbox Bluetooth Gamepad") ||
                joystickName.Contains("Xbox Wireless Controller"))
            {
                return SupportedControllerType.Xbox;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");
            return SupportedControllerType.GenericUnity;
        }
    }
}