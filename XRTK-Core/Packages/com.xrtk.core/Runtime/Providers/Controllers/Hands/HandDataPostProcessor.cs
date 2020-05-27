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
    /// The hand data post processor updates <see cref="HandData"/> provided
    /// by a platform and enriches it with potentially missing information.
    /// </summary>
    public sealed class HandDataPostProcessor
    {
        /// <summary>
        /// Creates a new instance of the hand data post processor.
        /// </summary>
        /// <param name="trackedPoses">Pose recognizer instance to use for pose recognition.</param>
        public HandDataPostProcessor(IReadOnlyList<HandControllerPoseDefinition> trackedPoses)
        {
            Recognizer = new HandPoseRecognizer(trackedPoses);
        }

        private const float TWO_CENTIMETER_SQUARE_MAGNITUDE = 0.0004f;
        private const float FIVE_CENTIMETER_SQUARE_MAGNITUDE = 0.0025f;
        private const float TEN_CENTIMETER_SQUARE_MAGNITUDE = 0.01f;
        private const float FIFTEEN_CENTIMETER_SQUARE_MAGNITUDE = 0.0025f;
        private const float PINCH_STRENGTH_DISTANCE = FIVE_CENTIMETER_SQUARE_MAGNITUDE - TWO_CENTIMETER_SQUARE_MAGNITUDE;
        private const float GRIP_STRENGTH_DISTANCE = FIFTEEN_CENTIMETER_SQUARE_MAGNITUDE - TEN_CENTIMETER_SQUARE_MAGNITUDE;

        /// <summary>
        /// Recognizer instance used by the converter for pose recognition.
        /// </summary>
        private HandPoseRecognizer Recognizer { get; }

        /// <summary>
        /// Is the <see cref="HandData.PointerPose"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesPointerPose { get; set; }

        /// <summary>
        /// Is the <see cref="HandData.IsPinching"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesIsPinching { get; set; }

        /// <summary>
        /// Is the <see cref="HandData.PinchStrength"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesPinchStrength { get; set; }

        /// <summary>
        /// Is the <see cref="HandData.IsPointing"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesIsPointing { get; set; }

        /// <summary>
        /// Is the <see cref="HandData.IsGripping"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesIsGripping { get; set; }

        /// <summary>
        /// Is the <see cref="HandData.GripStrength"/> data provided by the
        /// implementing platform converter?
        /// </summary>
        public bool PlatformProvidesGripStrength { get; set; }

        /// <summary>
        /// Finalizes the hand data retrieved from platform APIs by adding
        /// any information the platform could not provide.
        /// </summary>
        /// <param name="handData">The hand data retrieved from platform conversion.</param>
        public void PostProcess(HandData handData)
        {
            UpdateIsPinchingAndStrength(handData);
            UpdateIsPointing(handData);
            UpdateIsGrippingAndStrength(handData);
            UpdatePointerPose(handData);
            UpdateTrackedPose(handData);
        }

        /// <summary>
        /// Updates <see cref="HandData.IsGripping"/> and <see cref="HandData.GripStrength"/>
        /// if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsGripping"/> and <see cref="HandData.GripStrength"/> for.</param>
        private void UpdateIsGrippingAndStrength(HandData handData)
        {
            if (handData.IsTracked)
            {
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                var littleTipPose = handData.Joints[(int)TrackedHandJoint.LittleTip];
                var ringTipPose = handData.Joints[(int)TrackedHandJoint.RingTip];
                var middleTipPose = handData.Joints[(int)TrackedHandJoint.MiddleTip];
                var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];

                if (!PlatformProvidesIsGripping)
                {
                    handData.IsGripping = (palmPose.Position - littleTipPose.Position).sqrMagnitude <= TEN_CENTIMETER_SQUARE_MAGNITUDE &&
                    (palmPose.Position - ringTipPose.Position).sqrMagnitude <= TEN_CENTIMETER_SQUARE_MAGNITUDE &&
                    (palmPose.Position - middleTipPose.Position).sqrMagnitude <= TEN_CENTIMETER_SQUARE_MAGNITUDE &&
                    (palmPose.Position - indexTipPose.Position).sqrMagnitude <= TEN_CENTIMETER_SQUARE_MAGNITUDE;

                    Debug.Log(handData.IsGripping);
                }

                if (!PlatformProvidesGripStrength)
                {
                    var averageDistanceSquareMagnitude = ((palmPose.Position - littleTipPose.Position).sqrMagnitude +
                        (palmPose.Position - ringTipPose.Position).sqrMagnitude +
                        (palmPose.Position - middleTipPose.Position).sqrMagnitude +
                        (palmPose.Position - indexTipPose.Position).sqrMagnitude) / 4f;

                    handData.GripStrength = 1 - Mathf.Clamp(averageDistanceSquareMagnitude / GRIP_STRENGTH_DISTANCE, 0f, 1f);

                    //if (handData.GripStrength == 0f)
                    //{
                    //    Debug.LogWarning(handData.GripStrength);
                    //}

                    //Debug.Log(handData.GripStrength);

                    //if (handData.GripStrength == 1f)
                    //{
                    //    Debug.LogWarning(handData.GripStrength);
                    //}
                }
            }
            else
            {
                handData.IsGripping = false;
                handData.GripStrength = 0f;
            }
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/>
        /// if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/> for.</param>
        private void UpdateIsPinchingAndStrength(HandData handData)
        {
            if (handData.IsTracked)
            {
                var thumbTipPose = handData.Joints[(int)TrackedHandJoint.ThumbTip];
                var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];

                if (!PlatformProvidesIsPinching)
                {
                    handData.IsPinching = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude < TWO_CENTIMETER_SQUARE_MAGNITUDE;
                }

                if (!PlatformProvidesPinchStrength)
                {
                    var distanceSquareMagnitude = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude - TWO_CENTIMETER_SQUARE_MAGNITUDE;
                    handData.PinchStrength = 1 - Mathf.Clamp(distanceSquareMagnitude / PINCH_STRENGTH_DISTANCE, 0f, 1f);
                }
            }
            else
            {
                handData.IsPinching = false;
                handData.PinchStrength = 0f;
            }
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPointing"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPointing"/> for.</param>
        private void UpdateIsPointing(HandData handData)
        {
            if (handData.IsTracked && !handData.IsPinching)
            {
                if (!PlatformProvidesIsPointing)
                {
                    var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                    var cameraTransform = MixedRealityToolkit.CameraSystem != null
                    ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform
                    : CameraCache.Main.transform;

                    // We check if the palm forward is roughly in line with the camera lookAt.
                    var projectedPalmUp = Vector3.ProjectOnPlane(-palmPose.Up, cameraTransform.up);
                    handData.IsPointing = Vector3.Dot(cameraTransform.forward, projectedPalmUp) > 0.3f;
                }
            }
            else
            {
                handData.IsPointing = false;
            }
        }

        /// <summary>
        /// Updates <see cref="HandData.TrackedPose"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.TrackedPose"/> for.</param>
        private void UpdateTrackedPose(HandData handData)
        {
            if (handData.IsTracked)
            {
                Recognizer.Recognize(handData);
            }
            else
            {
                handData.TrackedPose = null;
            }
        }

        /// <summary>
        /// Updates <see cref="HandData.PointerPose"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.PointerPose"/> for.</param>
        private void UpdatePointerPose(HandData handData)
        {
            if (handData.IsTracked && !PlatformProvidesPointerPose)
            {
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                palmPose.Rotation = Quaternion.Inverse(palmPose.Rotation) * palmPose.Rotation;

                var thumbProximalPose = handData.Joints[(int)TrackedHandJoint.ThumbProximal];
                var indexDistalPose = handData.Joints[(int)TrackedHandJoint.IndexDistal];
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