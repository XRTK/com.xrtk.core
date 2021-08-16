// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    [System.Runtime.InteropServices.Guid("435C4F16-8E23-4228-B2B0-5FCE09A97043")]
    public class SimulatedMixedRealityHandController : MixedRealityHandController, IMixedRealitySimulatedController
    {
        /// <inheritdoc />
        public SimulatedMixedRealityHandController() : base() { }

        /// <inheritdoc />
        public SimulatedMixedRealityHandController(IMixedRealityHandControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            // 6 DoF pose of the spatial pointer ("far interaction pointer").
            new MixedRealityInteractionMapping("Spatial Pointer Pose", AxisType.SixDof, DeviceInputType.SpatialPointer),
            // Select / pinch button press / release.
            new MixedRealityInteractionMapping("Select", AxisType.Digital, DeviceInputType.Select),
            // Hand in pointing pose yes/no?
            new MixedRealityInteractionMapping("Point", AxisType.Digital, DeviceInputType.ButtonPress),
            // Grip / grab button press / release.
            new MixedRealityInteractionMapping("Grip", AxisType.Digital, DeviceInputType.TriggerPress),
            // 6 DoF grip pose ("Where to put things when grabbing something?")
            new MixedRealityInteractionMapping("Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip),
            // 6 DoF index finger tip pose (mainly for "near interaction pointer").
            new MixedRealityInteractionMapping("Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger),
            
            // Simulation specifics...
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

            if (Interactions[6].BoolData)
            {
                rotationDeltaEulerAngles.y = rotationDelta;
            }

            if (Interactions[7].BoolData)
            {
                rotationDeltaEulerAngles.y = -rotationDelta;
            }

            if (Interactions[8].BoolData)
            {
                rotationDeltaEulerAngles.x = -rotationDelta;
            }

            if (Interactions[9].BoolData)
            {
                rotationDeltaEulerAngles.x = rotationDelta;
            }

            if (Interactions[10].BoolData)
            {
                rotationDeltaEulerAngles.z = -rotationDelta;
            }

            if (Interactions[11].BoolData)
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

            if (Interactions[12].BoolData)
            {
                mousePosition.z += Time.deltaTime * depthMultiplier;
            }

            if (Interactions[13].BoolData)
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
