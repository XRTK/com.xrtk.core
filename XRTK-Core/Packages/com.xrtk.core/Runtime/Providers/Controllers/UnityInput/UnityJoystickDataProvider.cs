// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// Manages joysticks using unity input system.
    /// </summary>
    [Obsolete]
    [System.Runtime.InteropServices.Guid("A4D8D13B-253C-469A-A3A2-ECFE16DD969F")]
    public class UnityJoystickDataProvider : BaseControllerDataProvider
    {
        /// <inheritdoc />
        public UnityJoystickDataProvider(string name, uint priority, UnityInputControllerDataProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        private const float DEVICE_REFRESH_INTERVAL = 3.0f;

        protected static readonly Dictionary<string, GenericJoystickController> ActiveGenericControllers = new Dictionary<string, GenericJoystickController>();

        private float deviceRefreshTimer;
        private string[] lastDeviceList;

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DEVICE_REFRESH_INTERVAL)
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

            foreach (var controller in ActiveGenericControllers)
            {
                RemoveController(controller.Key, false);
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

                    if (string.IsNullOrEmpty(joystickName)) { continue; }
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
                        InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
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

            var controllerType = GetCurrentControllerType(joystickName);

            if (controllerType == null)
            {
                return null;
            }

            GenericJoystickController detectedController;

            try
            {
                detectedController = Activator.CreateInstance(controllerType, this, TrackingState.NotTracked, Handedness.None, GetControllerProfile(controllerType, Handedness.None)) as GenericJoystickController;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller!\n{e}");
                return null;
            }

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller!");
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
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }

            if (clearFromRegistry)
            {
                RemoveController(controller);
                ActiveGenericControllers.Remove(joystickName);
            }
        }

        /// <summary>
        /// Gets the current controller type for the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of they joystick from Unity's <see cref="Input.GetJoystickNames"/></param>
        /// <returns>The supported controller type</returns>
        protected virtual Type GetCurrentControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName) ||
                joystickName.Contains("<0"))
            {
                Debug.LogError($"Joystick not found! {joystickName}");
                return null;
            }

            if (joystickName.Contains("Xbox Controller") ||
                joystickName.Contains("Xbox One For Windows") ||
                joystickName.Contains("Xbox Bluetooth Gamepad") ||
                joystickName.Contains("Xbox Wireless Controller"))
            {
                return typeof(XboxController);
            }

            return null;
        }
    }
}
