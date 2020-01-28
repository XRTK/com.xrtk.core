// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extensions for the InteractionMapping class to refactor the generic methods used for raising events in InteractionMappings.
    /// </summary>
    public static class InteractionMappingsExtensions
    {
        /// <summary>
        /// Raise the actions to the input system.
        /// </summary>
        /// <param name="interactionMapping"></param>
        /// <param name="inputSource"></param>
        /// <param name="controllerHandedness"></param>
        public static void RaiseInputAction(this MixedRealityInteractionMapping interactionMapping, IMixedRealityInputSource inputSource, Handedness controllerHandedness)
        {
            var changed = interactionMapping.ControlActivated;
            var updated = interactionMapping.Updated;

            if (changed &&
                (interactionMapping.AxisType == AxisType.Digital ||
                 interactionMapping.AxisType == AxisType.SingleAxis))
            {
                if (interactionMapping.BoolData)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputDown(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputUp(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }

            if (updated)
            {
                switch (interactionMapping.AxisType)
                {
                    case AxisType.Digital:
                        MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.BoolData ? 1 : 0);
                        break;
                    case AxisType.SingleAxis:
                        MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.FloatData);
                        break;
                    case AxisType.DualAxis:
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.Vector2Data);
                        break;
                    case AxisType.ThreeDofPosition:
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PositionData);
                        break;
                    case AxisType.ThreeDofRotation:
                        MixedRealityToolkit.InputSystem?.RaiseRotationInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.RotationData);
                        break;
                    case AxisType.SixDof:
                        MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PoseData);
                        break;
                }
            }
        }

        /// <summary>
        /// Query the controller mapping library for any mappings for a specific controller
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="controllerType">Type of controller to search for</param>
        /// <param name="handedness">Specific controller hand to query</param>
        /// <param name="resolvedMapping">If found, the specific controller mapping is returned</param>
        /// <returns>Returns true if a mapping profile is found</returns>
        /// <remarks>Will also check for any controller mappings assigned to the handedness of Both (both hands) allowing a single configuration to be used for either hand</remarks>
        public static bool GetControllerInteractionMapping(this List<MixedRealityControllerMapping> mappings, Type controllerType, Handedness handedness, out MixedRealityControllerMapping resolvedMapping)
        {
            resolvedMapping = MixedRealityControllerMapping.None;

            for (int i = 0; i < mappings?.Count; i++)
            {
                // Assign any known interaction mappings.
                if (mappings[i].ControllerType?.Type == controllerType &&
                    (mappings[i].Handedness == handedness || mappings[i].Handedness == Handedness.Both) &&
                    mappings[i].Interactions.Length > 0)
                {
                    resolvedMapping = mappings[i];
                    return true;
                }
            }
                    return false;
        }
    }
}