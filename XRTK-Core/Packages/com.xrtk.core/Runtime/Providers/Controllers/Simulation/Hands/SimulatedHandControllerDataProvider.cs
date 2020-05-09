﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;
using XRTK.Providers.Controllers.Hands;

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
            var globalSettingsProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;

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

            leftHandConverter = new SimulatedHandDataConverter(
                Handedness.Left,
                TrackedPoses,
                HandPoseAnimationSpeed,
                JitterAmount,
                DefaultDistance);

            rightHandConverter = new SimulatedHandDataConverter(
                Handedness.Right,
                TrackedPoses,
                HandPoseAnimationSpeed,
                JitterAmount,
                DefaultDistance);
        }

        private readonly SimulatedHandDataConverter leftHandConverter;
        private readonly SimulatedHandDataConverter rightHandConverter;

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
        public IReadOnlyList<HandControllerPoseDefinition> TrackedPoses { get; }

        /// <inheritdoc />
        protected override void UpdateSimulatedController(IMixedRealitySimulatedController simulatedController)
        {
            // Ignore updates if the simulated controllers are not tracked, but only visible.
            if (simulatedController.ControllerHandedness == Handedness.Left && !LeftControllerIsTracked)
            {
                return;
            }
            else if (simulatedController.ControllerHandedness == Handedness.Right && !RightControllerIsTracked)
            {
                return;
            }

            var simulatedHandController = (MixedRealityHandController)simulatedController;
            var converter = simulatedHandController.ControllerHandedness == Handedness.Left
                ? leftHandConverter
                : rightHandConverter;

            simulatedHandController.UpdateController(converter.GetSimulatedHandData(
                simulatedController.GetPosition(DepthMultiplier),
                simulatedController.GetDeltaRotation(RotationSpeed)));
        }

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