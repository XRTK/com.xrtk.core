// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Services;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation for platform converters mapping platform
    /// hand data to the toolkit's agnostic hand data.
    /// </summary>
    public abstract class BaseHandDataConverter
    {
        /// <summary>
        /// Creates a new instance of the hand data converter.
        /// </summary>
        /// <param name="handedness">Handedness this converter is converting to.</param>
        /// <param name="trackedPoses">Pose recognizer instance to use for pose recognition.</param>
        protected BaseHandDataConverter(Handedness handedness, IReadOnlyList<HandControllerPoseDefinition> trackedPoses)
        {
            Handedness = handedness;
            Recognizer = new HandPoseRecognizer(trackedPoses);
        }

        private const float TWO_CENTIMETER_SQUARE_MAGNITUDE = 0.0004f;
        private const float FIVE_CENTIMETER_SQUARE_MAGNITUDE = 0.0025f;
        private const float PINCH_STRENGTH_DISTANCE = FIVE_CENTIMETER_SQUARE_MAGNITUDE - TWO_CENTIMETER_SQUARE_MAGNITUDE;

        /// <summary>
        /// Recognizer instance used by the converter for pose recognition.
        /// </summary>
        private HandPoseRecognizer Recognizer { get; }

        /// <summary>
        /// The handedness this converter is converting to.
        /// </summary>
        protected Handedness Handedness { get; }

        /// <summary>
        /// Is the <see cref="HandData.PointerPose"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        protected abstract bool PlatformProvidesPointerPose { get; }

        /// <summary>
        /// Is the <see cref="HandData.IsPinching"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        protected abstract bool PlatformProvidesIsPinching { get; }

        /// <summary>
        /// Is the <see cref="HandData.PinchStrength"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        protected abstract bool PlatformProvidesPinchStrength { get; }

        /// <summary>
        /// Is the <see cref="HandData.IsPointing"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        protected abstract bool PlatformProvidesIsPointing { get; }

        /// <summary>
        /// Finalizes the hand data retrieved from platform APIs by adding
        /// any information the platform could not provide.
        /// </summary>
        /// <param name="handData">The hand data retrieved from platform conversion.</param>
        protected void Finalize(HandData handData)
        {
            if (!PlatformProvidesIsPinching)
            {
                UpdateIsPinching(handData);
            }

            if (!PlatformProvidesPinchStrength)
            {
                UpdatePinchStrength(handData);
            }

            if (!PlatformProvidesIsPointing)
            {
                UpdateIsPointing(handData);
            }

            if (!PlatformProvidesPointerPose)
            {
                UpdatePointerPose(handData);
            }

            UpdateTrackedPose(handData);
        }

        /// <summary>
        /// Fallback to compute whether the hand is pinching for the hand controller.
        /// </summary>
        /// <param name="handData">The hand data to update is pinching state for.</param>
        private void UpdateIsPinching(HandData handData)
        {
            if (handData.IsTracked)
            {
                var thumbTipPose = handData.Joints[(int)TrackedHandJoint.ThumbTip];
                var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];
                handData.IsPinching = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude < TWO_CENTIMETER_SQUARE_MAGNITUDE;
            }
            else
            {
                handData.IsPinching = false;
            }
        }

        /// <summary>
        /// Fallback to compute the current pinch strength for the hand controller.
        /// </summary>
        /// <param name="handData">The hand data to update pinch strength for.</param>
        private void UpdatePinchStrength(HandData handData)
        {
            if (handData.IsTracked)
            {
                var thumbTipPose = handData.Joints[(int)TrackedHandJoint.ThumbTip];
                var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];
                var distanceSquareMagnitude = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude - TWO_CENTIMETER_SQUARE_MAGNITUDE;
                handData.PinchStrength = 1 - Mathf.Clamp(distanceSquareMagnitude / PINCH_STRENGTH_DISTANCE, 0f, 1f);
            }
            else
            {
                handData.PinchStrength = 0f;
            }
        }

        /// <summary>
        /// Fallback to compute whether the hand is pointing for the hand controller.
        /// </summary>
        /// <param name="handData">The hand data to update is pointing state for.</param>
        private void UpdateIsPointing(HandData handData)
        {
            if (handData.IsTracked && !handData.IsPinching)
            {
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                var cameraTransform = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform
                : CameraCache.Main.transform;

                // We check if the palm forward is roughly in line with the camera lookAt.
                var projectedPalmUp = Vector3.ProjectOnPlane(-palmPose.Up, cameraTransform.up);
                handData.IsPointing = Vector3.Dot(cameraTransform.forward, projectedPalmUp) > 0.3f;
            }
            else
            {
                handData.IsPointing = false;
            }
        }

        /// <summary>
        /// Updates the tracked pose for the hand.
        /// </summary>
        /// <param name="handData">The hand data to update tracked pose for.</param>
        private void UpdateTrackedPose(HandData handData)
        {
            if (handData.IsTracked)
            {
                Recognizer.Recognize(handData, Handedness);
            }
            else
            {
                handData.TrackedPose = null;
            }
        }

        /// <summary>
        /// Fallback to compute a pointer pose for the hand controller.
        /// </summary>
        /// <param name="handData">The hand data to update pointer pose for.</param>
        private void UpdatePointerPose(HandData handData)
        {
            if (handData.IsTracked)
            {
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                palmPose.Rotation = Quaternion.Inverse(palmPose.Rotation) * palmPose.Rotation;

                var thumbProximalPose = handData.Joints[(int)TrackedHandJoint.ThumbProximalJoint];
                var indexDistalPose = handData.Joints[(int)TrackedHandJoint.IndexDistalJoint];
                var pointerPosition = Vector3.Lerp(thumbProximalPose.Position, indexDistalPose.Position, .5f);
                var pointerEndPosition = pointerPosition + palmPose.Forward * 10f;

                var camera = MixedRealityToolkit.CameraSystem != null
                    ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera
                    : CameraCache.Main;

                var pointerDirection = pointerEndPosition - pointerPosition;
                var pointerRotation = Quaternion.LookRotation(pointerDirection, camera.transform.up);

                handData.PointerPose = new MixedRealityPose(pointerPosition, pointerRotation);
            }
        }
    }
}