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
        public static void UpdateInteractionMappingBool(this MixedRealityInteractionMapping interactionMapping, IMixedRealityInputSource inputSource, Handedness controllerHandedness)
        {
            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                if (interactionMapping.BoolData)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputDown(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputUp(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }

            if (interactionMapping.Updated)
            {
                MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
            }
        }

        public static void UpdateInteractionMappingFloat(this MixedRealityInteractionMapping interactionMapping, IMixedRealityInputSource inputSource, Handedness controllerHandedness)
        {
            // If our value changed raise it.
            if (interactionMapping.Updated)
            {
                // Raise input system Event if it enabled
                MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
            }
        }

        public static void UpdateInteractionMappingVector2(this MixedRealityInteractionMapping interactionMapping, IMixedRealityInputSource inputSource, Handedness controllerHandedness)
        {
            // If our value changed raise it.
            if (interactionMapping.Updated)
            {
                // Raise input system Event if it enabled
                MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
            }
        }

        public static void UpdateInteractionMappingPose(this MixedRealityInteractionMapping interactionMapping, IMixedRealityInputSource inputSource, Handedness controllerHandedness)
        {
            // If our value changed raise it.
            if (interactionMapping.Updated)
            {
                // Raise input system Event if it enabled 
                MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }
    }
}