// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.UnityInput;
using XRTK.Services;

namespace XRTK.Providers.Controllers.OpenVR
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    public class OpenVRDataProvider : UnityJoystickDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public OpenVRDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
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

            Handedness controllingHand;

            if (joystickName.Contains("Left"))
            {
                controllingHand = Handedness.Left;
            }
            else if (joystickName.Contains("Right"))
            {
                controllingHand = Handedness.Right;
            }
            else
            {
                controllingHand = Handedness.None;
            }

            var currentControllerType = GetCurrentControllerType(joystickName);
            Type controllerType;

            switch (currentControllerType)
            {
                case SupportedControllerType.GenericOpenVR:
                    controllerType = typeof(GenericOpenVRController);
                    break;
                case SupportedControllerType.ViveWand:
                    controllerType = typeof(ViveWandOpenVRController);
                    break;
                case SupportedControllerType.ViveKnuckles:
                    controllerType = typeof(ViveKnucklesOpenVRController);
                    break;
                case SupportedControllerType.OculusTouch:
                    controllerType = typeof(OculusTouchOpenVRController);
                    break;
                case SupportedControllerType.OculusRemote:
                    controllerType = typeof(OculusRemoteOpenVRController);
                    break;
                case SupportedControllerType.OculusGo:
                    controllerType = typeof(OculusGoOpenVRController);
                    break;
                case SupportedControllerType.WindowsMixedReality:
                    controllerType = typeof(WindowsMixedRealityOpenVRMotionController);
                    break;
                default:
                    return null;
            }

            var pointers = RequestPointers(controllerType, controllingHand);
            var inputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers);
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericOpenVRController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                Debug.LogError($"Failed to Setup {controllerType.Name} controller");
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            detectedController.TryRenderControllerModel(controllerType);

            ActiveGenericControllers.Add(joystickName, detectedController);
            AddController(detectedController);
            return detectedController;
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            if (joystickName.Contains("Oculus Rift CV1") || joystickName.Contains("Oculus Touch"))
            {
                return SupportedControllerType.OculusTouch;
            }

            if (joystickName.Contains("Oculus Tracked Remote"))
            {
                return SupportedControllerType.OculusGo;
            }

            if (joystickName.Contains("Oculus remote"))
            {
                return SupportedControllerType.OculusRemote;
            }

            if (joystickName.Contains("Vive Wand"))
            {
                return SupportedControllerType.ViveWand;
            }

            if (joystickName.Contains("Vive Knuckles"))
            {
                return SupportedControllerType.ViveKnuckles;
            }

            if (joystickName.Contains("WindowsMR"))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            if (joystickName.Contains("OpenVR") ||
                joystickName.Contains("Spatial"))
            {
                Debug.LogWarning($"{joystickName} does not have a defined controller type, falling back to {SupportedControllerType.GenericOpenVR} controller type.");
                return SupportedControllerType.GenericOpenVR;
            }

            return base.GetCurrentControllerType(joystickName);
        }

        #endregion Controller Utilities
    }
}