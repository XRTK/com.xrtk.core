// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// The <see cref="HandGripPostProcessor"/> processes <see cref="HandData"/>
    /// retrieved from platform APIs and calculates needed information for <see cref="HandData.GripStrength"/>
    /// and <see cref="HandData.IsGripping"/> as well as <see cref="HandData.FingerCurlStrengths"/>.
    /// </summary>
    public sealed class HandGripPostProcessor : IHandDataPostProcessor
    {
        private const float CURL_THUMB_METACARPAL_LOW_END_ANGLE = 70f;
        private const float CURL_THUMB_METACARPAL_HIGH_END_ANGLE = 100f;
        private const float CURL_THUMB_METACARPAL_DISTANCE = CURL_THUMB_METACARPAL_HIGH_END_ANGLE - CURL_THUMB_METACARPAL_LOW_END_ANGLE;
        private const float CURL_THUMB_PROXIMAL_LOW_END_ANGLE = 60f;
        private const float CURL_THUMB_PROXIMAL_HIGH_END_ANGLE = 110f;
        private const float CURL_THUMB_PROXIMAL_DISTANCE = CURL_THUMB_PROXIMAL_HIGH_END_ANGLE - CURL_THUMB_PROXIMAL_LOW_END_ANGLE;

        private const float CURL_INDEX_PROXIMAL_LOW_END_ANGLE = 9f;
        private const float CURL_INDEX_PROXIMAL_HIGH_END_ANGLE = 58f;
        private const float CURL_INDEX_PROXIMAL_DISTANCE = CURL_INDEX_PROXIMAL_HIGH_END_ANGLE - CURL_INDEX_PROXIMAL_LOW_END_ANGLE;
        private const float CURL_INDEX_INTERMEDIATE_LOW_END_ANGLE = 18f;
        private const float CURL_INDEX_INTERMEDIATE_HIGH_END_ANGLE = 120f;
        private const float CURL_INDEX_INTERMEDIATE_DISTANCE = CURL_INDEX_INTERMEDIATE_HIGH_END_ANGLE - CURL_INDEX_INTERMEDIATE_LOW_END_ANGLE;

        private const float CURL_MIDDLE_PROXIMAL_LOW_END_ANGLE = 6f;
        private const float CURL_MIDDLE_PROXIMAL_HIGH_END_ANGLE = 65f;
        private const float CURL_MIDDLE_PROXIMAL_DISTANCE = CURL_MIDDLE_PROXIMAL_HIGH_END_ANGLE - CURL_MIDDLE_PROXIMAL_LOW_END_ANGLE;
        private const float CURL_MIDDLE_INTERMEDIATE_LOW_END_ANGLE = 16f;
        private const float CURL_MIDDLE_INTERMEDIATE_HIGH_END_ANGLE = 150f;
        private const float CURL_MIDDLE_INTERMEDIATE_DISTANCE = CURL_MIDDLE_INTERMEDIATE_HIGH_END_ANGLE - CURL_MIDDLE_INTERMEDIATE_LOW_END_ANGLE;

        private const float CURL_RING_PROXIMAL_LOW_END_ANGLE = 16f;
        private const float CURL_RING_PROXIMAL_HIGH_END_ANGLE = 73f;
        private const float CURL_RING_PROXIMAL_DISTANCE = CURL_RING_PROXIMAL_HIGH_END_ANGLE - CURL_RING_PROXIMAL_LOW_END_ANGLE;
        private const float CURL_RING_INTERMEDIATE_LOW_END_ANGLE = 16f;
        private const float CURL_RING_INTERMEDIATE_HIGH_END_ANGLE = 160f;
        private const float CURL_RING_INTERMEDIATE_DISTANCE = CURL_RING_INTERMEDIATE_HIGH_END_ANGLE - CURL_RING_INTERMEDIATE_LOW_END_ANGLE;

        private const float CURL_LITTLE_PROXIMAL_LOW_END_ANGLE = 25f;
        private const float CURL_LITTLE_PROXIMAL_HIGH_END_ANGLE = 79f;
        private const float CURL_LITTLE_PROXIMAL_DISTANCE = CURL_LITTLE_PROXIMAL_HIGH_END_ANGLE - CURL_LITTLE_PROXIMAL_LOW_END_ANGLE;
        private const float CURL_LITTLE_INTERMEDIATE_LOW_END_ANGLE = 28f;
        private const float CURL_LITTLE_INTERMEDIATE_HIGH_END_ANGLE = 150f;
        private const float CURL_LITTLE_INTERMEDIATE_DISTANCE = CURL_LITTLE_INTERMEDIATE_HIGH_END_ANGLE - CURL_LITTLE_INTERMEDIATE_LOW_END_ANGLE;

        private const float CURL_TOTAL_INTERMEDIATE_DISTANCE = CURL_LITTLE_INTERMEDIATE_DISTANCE + CURL_RING_INTERMEDIATE_DISTANCE + CURL_MIDDLE_INTERMEDIATE_DISTANCE + CURL_INDEX_INTERMEDIATE_DISTANCE;

        private const float IS_GRIPPING_CURL_THRESHOLD = .9f;
        private const float IS_GRIPPING_INDEX_CURL_THRESHOLD = .8f;

        private const bool DEBUG_LOG_VALUES_TO_CONSOLE = false;

        /// <inheritdoc />
        public HandData PostProcess(Handedness handedness, HandData handData)
        {
            if (handData.TrackingState == TrackingState.Tracked)
            {
                // Gather needed data for calculations.
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                var palmLookRotation = Quaternion.LookRotation(palmPose.Forward, palmPose.Up);
                var thumbMetacarpalPose = handData.Joints[(int)TrackedHandJoint.ThumbMetacarpal];
                var thumbProximalPose = handData.Joints[(int)TrackedHandJoint.ThumbProximal];
                var indexProximalPose = handData.Joints[(int)TrackedHandJoint.IndexProximal];
                var indexIntermediatePose = handData.Joints[(int)TrackedHandJoint.IndexIntermediate];
                var middleProximalPose = handData.Joints[(int)TrackedHandJoint.MiddleProximal];
                var middleIntermediatePose = handData.Joints[(int)TrackedHandJoint.MiddleIntermediate];
                var ringProximalPose = handData.Joints[(int)TrackedHandJoint.RingProximal];
                var ringIntermediatePose = handData.Joints[(int)TrackedHandJoint.RingIntermediate];
                var littleProximalPose = handData.Joints[(int)TrackedHandJoint.LittleProximal];
                var littleIntermediatePose = handData.Joints[(int)TrackedHandJoint.LittleIntermediate];

                // Calculate per finger curl angles.
                var thumbMetacarpalCurl = Quaternion.Angle(palmLookRotation, thumbMetacarpalPose.Rotation);
                var thumbProximalCurl = Quaternion.Angle(palmLookRotation, thumbProximalPose.Rotation);
                var indexProximalCurl = Quaternion.Angle(palmLookRotation, indexProximalPose.Rotation);
                var indexIntermediateCurl = Quaternion.Angle(palmLookRotation, indexIntermediatePose.Rotation);
                var middleProximalCurl = Quaternion.Angle(palmLookRotation, middleProximalPose.Rotation);
                var middleIntermediateCurl = Quaternion.Angle(palmLookRotation, middleIntermediatePose.Rotation);
                var ringProximalCurl = Quaternion.Angle(palmLookRotation, ringProximalPose.Rotation);
                var ringIntermediateCurl = Quaternion.Angle(palmLookRotation, ringIntermediatePose.Rotation);
                var littleProximalCurl = Quaternion.Angle(palmLookRotation, littleProximalPose.Rotation);
                var littleIntermediateCurl = Quaternion.Angle(palmLookRotation, littleIntermediatePose.Rotation);

                // Grip strength is defined as the total traveled curl distance for the intermediate joints
                // compared to the total travel curl distance for the whole hand.
                var totalIntermediateCurlDistance =
                    littleIntermediateCurl - CURL_LITTLE_INTERMEDIATE_LOW_END_ANGLE +
                    (ringIntermediateCurl - CURL_RING_INTERMEDIATE_LOW_END_ANGLE) +
                    (middleIntermediateCurl - CURL_MIDDLE_INTERMEDIATE_LOW_END_ANGLE) +
                    (indexIntermediateCurl - CURL_INDEX_INTERMEDIATE_LOW_END_ANGLE);

                // For grip strength we just use the intermediate curls.
                handData.GripStrength = Mathf.Clamp(totalIntermediateCurlDistance / CURL_TOTAL_INTERMEDIATE_DISTANCE, 0f, 1f);

                // For finger curl strengths we add the proximal curl to the equation.
                var thumbCurlStrength = Mathf.Clamp(
                    (((thumbMetacarpalCurl - CURL_THUMB_METACARPAL_LOW_END_ANGLE) / CURL_THUMB_METACARPAL_DISTANCE) +
                    (thumbProximalCurl - CURL_THUMB_PROXIMAL_LOW_END_ANGLE) / CURL_THUMB_PROXIMAL_DISTANCE) / 2, 0f, 1f);

                var indexCurlStrength = Mathf.Clamp(
                    (((indexProximalCurl - CURL_INDEX_PROXIMAL_LOW_END_ANGLE) / CURL_INDEX_PROXIMAL_DISTANCE) +
                    (indexIntermediateCurl - CURL_INDEX_INTERMEDIATE_LOW_END_ANGLE) / CURL_INDEX_INTERMEDIATE_DISTANCE) / 2, 0f, 1f);

                var middleCurlStrength = Mathf.Clamp(
                    (((middleProximalCurl - CURL_MIDDLE_PROXIMAL_LOW_END_ANGLE) / CURL_MIDDLE_PROXIMAL_DISTANCE) +
                    (middleIntermediateCurl - CURL_MIDDLE_INTERMEDIATE_LOW_END_ANGLE) / CURL_MIDDLE_INTERMEDIATE_DISTANCE) / 2, 0f, 1f);

                var ringCurlStrength = Mathf.Clamp(
                    (((ringProximalCurl - CURL_RING_PROXIMAL_LOW_END_ANGLE) / CURL_RING_PROXIMAL_DISTANCE) +
                    (ringIntermediateCurl - CURL_RING_INTERMEDIATE_LOW_END_ANGLE) / CURL_RING_INTERMEDIATE_DISTANCE) / 2, 0f, 1f);

                var littleCurlStrength = Mathf.Clamp(
                    (((littleProximalCurl - CURL_LITTLE_PROXIMAL_LOW_END_ANGLE) / CURL_LITTLE_PROXIMAL_DISTANCE) +
                    (littleIntermediateCurl - CURL_LITTLE_INTERMEDIATE_LOW_END_ANGLE) / CURL_LITTLE_INTERMEDIATE_DISTANCE) / 2, 0f, 1f);

                handData.FingerCurlStrengths = new float[]
                {
                    thumbCurlStrength,
                    indexCurlStrength,
                    middleCurlStrength,
                    ringCurlStrength,
                    littleCurlStrength,
                };

                // Hand is gripping if the grip strength passed the threshold. But we are also taking
                // the index curl into account explicitly, this helps avoiding the pinch gesture being
                // considered gripping as well.
                handData.IsGripping = handData.GripStrength >= IS_GRIPPING_CURL_THRESHOLD && indexCurlStrength >= IS_GRIPPING_INDEX_CURL_THRESHOLD;

                if (Debug.isDebugBuild && DEBUG_LOG_VALUES_TO_CONSOLE)
                {
                    Debug.Log($"Grip Strength: {handData.GripStrength} " +
                        $"| Thumb: {handData.FingerCurlStrengths[(int)HandFinger.Thumb]} " +
                        $"| Index: {handData.FingerCurlStrengths[(int)HandFinger.Index]} " +
                        $"| Middle: {handData.FingerCurlStrengths[(int)HandFinger.Middle]} " +
                        $"| Ring: {handData.FingerCurlStrengths[(int)HandFinger.Ring]} " +
                        $"| Little: {handData.FingerCurlStrengths[(int)HandFinger.Little]}");
                }
            }
            else
            {
                // When hand is not tracked, reset all values.
                handData.IsGripping = false;
                handData.GripStrength = 0f;
                handData.FingerCurlStrengths = new float[]
                 {
                    0f,
                    0f,
                    0f,
                    0f,
                    0f
                 };
            }

            return handData;
        }
    }
}