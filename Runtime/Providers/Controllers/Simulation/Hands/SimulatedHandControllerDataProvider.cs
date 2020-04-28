// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    [System.Runtime.InteropServices.Guid("07512FFC-5128-434C-B7BF-5CD7CA8EF853")]
    public class SimulatedHandControllerDataProvider : BaseSimulatedControllerDataProvider, ISimulatedHandControllerDataProvider
    {
        /// <inheritdoc />
        public SimulatedHandControllerDataProvider(string name, uint priority, SimulatedHandControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            var poseDefinitions = profile.PoseDefinitions;
            handPoseDefinitions = new List<SimulatedHandControllerPoseData>(poseDefinitions.Count);

            foreach (var poseDefinition in poseDefinitions)
            {
                handPoseDefinitions.Add(poseDefinition);
            }

            HandPoseAnimationSpeed = profile.HandPoseAnimationSpeed;
            HandPhysicsEnabled = profile.HandPhysicsEnabled;
            UseTriggers = profile.UseTriggers;
            BoundsMode = profile.BoundsMode;
            HandMeshingEnabled = profile.HandMeshingEnabled;
        }

        private readonly List<SimulatedHandControllerPoseData> handPoseDefinitions;

        /// <inheritdoc />
        public IReadOnlyList<SimulatedHandControllerPoseData> HandPoseDefinitions => handPoseDefinitions;

        /// <inheritdoc />
        public float HandPoseAnimationSpeed { get; }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; }

        /// <inheritdoc />
        public bool UseTriggers { get; }

        /// <inheritdoc />
        public HandBoundsMode BoundsMode { get; }

        /// <inheritdoc />
        public bool HandMeshingEnabled { get; }

        /// <inheritdoc />
        protected override IMixedRealitySimulatedController CreateAndRegisterSimulatedController(Handedness handedness)
        {
            SimulatedMixedRealityHandController controller;

            try
            {
                controller = new SimulatedMixedRealityHandController(this, TrackingState.Tracked, handedness, GetControllerMappingProfile(typeof(SimulatedMixedRealityHandController), handedness));

            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(SimulatedMixedRealityHandController)}!\n{e}");
                return null;
            }

            controller.TryRenderControllerModel();

            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            AddController(controller);
            return controller;
        }
    }
}