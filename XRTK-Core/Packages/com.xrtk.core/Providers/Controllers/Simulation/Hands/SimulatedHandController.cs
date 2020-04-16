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
    public class SimulatedHandController : MixedRealityHandController, IMixedRealitySimulatedController
    {
        public SimulatedHandController() : base() { }

        /// <inheritdoc />
        public SimulatedHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            simulatedHandControllerDataProvider = (ISimulatedHandControllerDataProvider)controllerDataProvider;

            converter = new SimulatedHandDataConverter(ControllerHandedness,
                simulatedHandControllerDataProvider.TrackedPoses,
                simulatedHandControllerDataProvider.HandPoseAnimationSpeed,
                simulatedHandControllerDataProvider.HandPoseDefinitions,
                simulatedHandControllerDataProvider.JitterAmount,
                simulatedHandControllerDataProvider.DefaultDistance);
        }

        private readonly SimulatedHandDataConverter converter;
        private readonly ISimulatedHandControllerDataProvider simulatedHandControllerDataProvider;

        private Vector3? lastMousePosition = null;

        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <returns>Updated hand rotation angles.</returns>
        private Vector3 DeltaRotation
        {
            get
            {
                UpdateSimulationMappings();

                float rotationDelta = simulatedHandControllerDataProvider.RotationSpeed * Time.deltaTime;
                Vector3 rotationDeltaEulerAngles = Vector3.zero;

                if (Interactions[0].BoolData)
                {
                    rotationDeltaEulerAngles.y = rotationDelta;
                }

                if (Interactions[1].BoolData)
                {
                    rotationDeltaEulerAngles.y = -rotationDelta;
                }

                if (Interactions[2].BoolData)
                {
                    rotationDeltaEulerAngles.x = -rotationDelta;
                }

                if (Interactions[3].BoolData)
                {
                    rotationDeltaEulerAngles.x = rotationDelta;
                }

                if (Interactions[4].BoolData)
                {
                    rotationDeltaEulerAngles.z = -rotationDelta;
                }

                if (Interactions[5].BoolData)
                {
                    rotationDeltaEulerAngles.z = rotationDelta;
                }

                return rotationDeltaEulerAngles;
            }
        }

        /// <summary>
        /// Gets a simulated depth tracking (hands closer / further from tracking device) update, as well
        /// as the hands simulated (x,y) position.
        /// </summary>
        /// <returns>Hand movement delta.</returns>
        private Vector3 DeltaPosition
        {
            get
            {
                UpdateSimulationMappings();

                Vector3 mouseDelta = lastMousePosition.HasValue ? Input.mousePosition - lastMousePosition.Value : Vector3.zero;

                if (Interactions[6].BoolData)
                {
                    mouseDelta.z += Time.deltaTime * simulatedHandControllerDataProvider.DepthMultiplier;
                }

                if (Interactions[7].BoolData)
                {
                    mouseDelta.z -= Time.deltaTime * simulatedHandControllerDataProvider.DepthMultiplier;
                }

                return mouseDelta;
            }
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
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
        public override void UpdateController()
        {
            UpdateController(converter.GetSimulatedHandData(DeltaPosition, DeltaRotation));
            lastMousePosition = Input.mousePosition;
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