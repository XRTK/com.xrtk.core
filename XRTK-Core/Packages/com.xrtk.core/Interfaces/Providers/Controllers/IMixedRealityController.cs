// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface IMixedRealityController
    {
        /// <summary>
        /// Is the controller enabled?
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Outputs the current state of the Input Source, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        TrackingState TrackingState { get; }

        /// <summary>
        /// The designated hand that the Input Source is managing, as defined by the SDK / Unity.
        /// </summary>
        Handedness ControllerHandedness { get; }

        /// <summary>
        /// The registered Input Source for this controller
        /// </summary>
        IMixedRealityInputSource InputSource { get; }

        /// <summary>
        /// The controller's "Visual" <see cref="UnityEngine.Component"/> in the scene.
        /// </summary>
        IMixedRealityControllerVisualizer Visualizer { get; }

        /// <summary>
        /// Indicates that this controller is currently providing position data.
        /// </summary>
        /// <remarks>
        /// This value may change during usage for some controllers. As a best practice,
        /// be sure to check this value before using position data.
        /// </remarks>
        bool IsPositionAvailable { get; }

        /// <summary>
        /// Indicates the accuracy of the position data being reported.
        /// </summary>
        bool IsPositionApproximate { get; }

        /// <summary>
        /// Indicates that this controller is currently providing rotation data.
        /// </summary>
        /// <remarks>
        /// This value may change during usage for some controllers. As a best practice,
        /// be sure to check this value before using rotation data.
        /// </remarks>
        bool IsRotationAvailable { get; }

        /// <summary>
        /// Mapping definition for this controller, linking the Physical inputs to logical Input System Actions
        /// </summary>
        MixedRealityInteractionMapping[] Interactions { get; }

        /// <summary>
        /// Start the controllers haptics feedback based on the intensity and duration provided.
        /// </summary>
        /// <param name="intensity">The intensity of the waveform.</param>
        /// <param name="duration">The time in seconds.</param>
        void StartHaptics(float intensity, float duration);

        /// <summary>
        /// Sends the controller a pre-defined haptic feedback based on the pattern and intensity.
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="intensity"></param>
        void SendHapticFeedback(HapticFeedbackType feedback, float intensity);

        /// <summary>
        /// Immediately stops any haptic feedback that's currently being executed.
        /// </summary>
        void StopHaptics();
    }
}