// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.OpenVR.Profiles;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Providers.Controllers.UnityInput;

namespace XRTK.Providers.Controllers.OpenVR
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    [System.Runtime.InteropServices.Guid("9B116969-7F4B-4CF9-9D5A-4FBA7793F50E")]
    public class OpenVRControllerDataProvider : UnityJoystickDataProvider
    {
        /// <inheritdoc />
        public OpenVRControllerDataProvider(string name, uint priority, OpenVRControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        #region Controller Utilities

        /// <inheritdoc />
        protected override GenericJoystickController GetOrAddController(string joystickName)
        {
            Debug.Assert(!string.IsNullOrEmpty(joystickName), "Joystick name is invalid!");

            // If a device is already registered with the ID provided, just return it.
            if (ActiveGenericControllers.ContainsKey(joystickName))
            {
                var controller = ActiveGenericControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            Handedness handedness;

            if (joystickName.Contains("Left"))
            {
                handedness = Handedness.Left;
            }
            else if (joystickName.Contains("Right"))
            {
                handedness = Handedness.Right;
            }
            else
            {
                handedness = Handedness.None;
            }

            var controllerType = GetCurrentControllerType(joystickName);

            GenericOpenVRController detectedController;

            try
            {
                detectedController = Activator.CreateInstance(controllerType, this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(controllerType, handedness)) as GenericOpenVRController;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name}");
                return null;
            }

            detectedController.TryRenderControllerModel();

            ActiveGenericControllers.Add(joystickName, detectedController);
            AddController(detectedController);
            return detectedController;
        }

        /// <inheritdoc />
        protected override Type GetCurrentControllerType(string joystickName)
        {
            if (joystickName.Contains("Oculus Rift CV1") ||
                joystickName.Contains("Oculus Touch") ||
                joystickName.Contains("Oculus Quest"))
            {
                return typeof(OculusTouchOpenVRController);
            }

            if (joystickName.Contains("Oculus Tracked Remote"))
            {
                return typeof(OculusGoOpenVRController);
            }

            if (joystickName.Contains("Oculus remote"))
            {
                return typeof(OculusRemoteOpenVRController);
            }

            if (joystickName.Contains("Vive Wand"))
            {
                return typeof(ViveWandOpenVRController);
            }

            if (joystickName.Contains("Vive Knuckles"))
            {
                return typeof(ViveKnucklesOpenVRController);
            }

            if (joystickName.Contains("WindowsMR") ||
                joystickName.Contains("Spatial"))
            {
                return typeof(WindowsMixedRealityOpenVRMotionController);
            }

            if (joystickName.Contains("OpenVR"))
            {
                Debug.LogWarning($"{joystickName} does not have a defined controller type, falling back to {nameof(GenericOpenVRController)}.");
                return typeof(GenericOpenVRController);
            }

            return base.GetCurrentControllerType(joystickName);
        }

        #endregion Controller Utilities
    }
}