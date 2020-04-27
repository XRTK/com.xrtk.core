// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public class SimulatedMixedRealityHandController : MixedRealityHandController, IMixedRealitySimulatedController
    {
        public SimulatedMixedRealityHandController() : base() { }

        /// <inheritdoc />
        public SimulatedMixedRealityHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping("Yaw Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.E),
            new MixedRealityInteractionMapping("Yaw Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Q),
            new MixedRealityInteractionMapping("Pitch Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.F),
            new MixedRealityInteractionMapping("Pitch Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.R),
            new MixedRealityInteractionMapping("Roll Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.X),
            new MixedRealityInteractionMapping("Roll Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Z),
            new MixedRealityInteractionMapping("Move Away (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.PageUp),
            new MixedRealityInteractionMapping("Move Closer (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.PageDown)
        };

        /// <inheritdoc />
        public Vector3 GetDeltaRotation(float rotationSpeed)
        {
            UpdateSimulationMappings();

            float rotationDelta = rotationSpeed * Time.deltaTime;
            Vector3 rotationDeltaEulerAngles = Vector3.zero;

            if (Interactions[1].BoolData)
            {
                rotationDeltaEulerAngles.y = rotationDelta;
            }

            if (Interactions[2].BoolData)
            {
                rotationDeltaEulerAngles.y = -rotationDelta;
            }

            if (Interactions[3].BoolData)
            {
                rotationDeltaEulerAngles.x = -rotationDelta;
            }

            if (Interactions[4].BoolData)
            {
                rotationDeltaEulerAngles.x = rotationDelta;
            }

            if (Interactions[5].BoolData)
            {
                rotationDeltaEulerAngles.z = -rotationDelta;
            }

            if (Interactions[6].BoolData)
            {
                rotationDeltaEulerAngles.z = rotationDelta;
            }

            return rotationDeltaEulerAngles;
        }

        /// <inheritdoc />
        public Vector3 GetPosition(float depthMultiplier)
        {
            UpdateSimulationMappings();

            Vector3 mousePosition = Input.mousePosition;

            if (Interactions[7].BoolData)
            {
                mousePosition.z += Time.deltaTime * depthMultiplier;
            }

            if (Interactions[8].BoolData)
            {
                mousePosition.z -= Time.deltaTime * depthMultiplier;
            }

            return mousePosition;
        }

        private void UpdateSimulationMappings()
        {
            for (int i = 0; i < Interactions?.Length; i++)
            {
                var interactionMapping = Interactions[i];

                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ButtonPress:
                        interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                        interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
                        break;
                }
            }
        }
    }
}