// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="MixedRealityInteractionMapping"/> to refactor the generic methods used for raising events.
    /// </summary>
    public static class InteractionMappingsExtensions
    {
        private static IMixedRealityInputSystem inputSystem = null;

        private static IMixedRealityInputSystem InputSystem
            => inputSystem ?? (inputSystem = MixedRealityToolkit.GetSystem<IMixedRealityInputSystem>());

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
                    InputSystem?.RaiseOnInputDown(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }

            if (updated)
            {
                switch (interactionMapping.AxisType)
                {
                    case AxisType.Digital:
                        InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.BoolData ? 1 : 0);
                        break;
                    case AxisType.SingleAxis:
                        InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.FloatData);
                        break;
                    case AxisType.DualAxis:
                        InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.Vector2Data);
                        break;
                    case AxisType.ThreeDofPosition:
                        InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PositionData);
                        break;
                    case AxisType.ThreeDofRotation:
                        InputSystem?.RaiseRotationInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.RotationData);
                        break;
                    case AxisType.SixDof:
                        InputSystem?.RaisePoseInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PoseData);
                        break;
                }
            }
        }
    }
}