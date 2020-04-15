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
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public class SimulatedHandControllerDataProvider : BaseSimulatedControllerDataProvider, ISimulatedHandControllerDataProvider
    {
        /// <inheritdoc />
        public SimulatedHandControllerDataProvider(string name, uint priority, SimulatedHandControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            var globalSettingsProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            var poseDefinitions = profile.PoseDefinitions;
            handPoseDefinitions = new List<SimulatedHandControllerPoseData>(poseDefinitions.Count);

            foreach (var poseDefinition in poseDefinitions)
            {
                handPoseDefinitions.Add(poseDefinition);
            }

            HandPoseAnimationSpeed = profile.HandPoseAnimationSpeed;

            HandMeshingEnabled = profile.HandMeshingEnabled != globalSettingsProfile.HandMeshingEnabled
                ? profile.HandMeshingEnabled
                : globalSettingsProfile.HandMeshingEnabled;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != globalSettingsProfile.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : globalSettingsProfile.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != globalSettingsProfile.UseTriggers
                ? profile.UseTriggers
                : globalSettingsProfile.UseTriggers;

            BoundsMode = profile.BoundsMode != globalSettingsProfile.BoundsMode
                ? profile.BoundsMode
                : globalSettingsProfile.BoundsMode;

            if (profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != globalSettingsProfile.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : globalSettingsProfile.TrackedPoses;
            }
            else
            {
                TrackedPoses = globalSettingsProfile.TrackedPoses;
            }
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
        public IReadOnlyList<SimulatedHandControllerPoseData> TrackedPoses { get; }

        /// <inheritdoc />
        protected override IMixedRealitySimulatedController CreateAndRegisterSimulatedController(Handedness handedness)
        {
            SimulatedHandController controller;

            try
            {
                controller = new SimulatedHandController(this, TrackingState.Tracked, handedness, GetControllerMappingProfile(typeof(SimulatedHandController), handedness));

            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(SimulatedHandController)}!\n{e}");
                return null;
            }

            controller.TryRenderControllerModel();

            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            AddController(controller);
            return controller;
        }
    }
}