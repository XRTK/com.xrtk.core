// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
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
            if (interactionMapping.Changed &&
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

            if (!interactionMapping.Updated) { return; }

            switch (interactionMapping.AxisType)
            {
                case AxisType.Digital:
                    MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.BoolData ? 1 : 0);
                    break;
                case AxisType.SingleAxis:
                    MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                    break;
                case AxisType.DualAxis:
                    MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
                    break;
                case AxisType.ThreeDofPosition:
                    MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PositionData);
                    break;
                case AxisType.ThreeDofRotation:
                    MixedRealityToolkit.InputSystem?.RaiseRotationInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.RotationData);
                    break;
                case AxisType.SixDof:
                    MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
                    break;
                default:
                    break;
            }
        }
    }
}