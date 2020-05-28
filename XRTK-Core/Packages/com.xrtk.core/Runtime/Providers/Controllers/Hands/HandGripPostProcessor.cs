// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// The <see cref="HandGripPostProcessor"/> processes <see cref="HandData"/>
    /// retrieved from platform APIs and calculates needed information for <see cref="HandData.GripStrength"/>
    /// and <see cref="HandData.IsGripping"/> as well as <see cref="HandData.FingerCurlStrengths"/>.
    /// </summary>
    public sealed class HandGripPostProcessor
    {
        private const float CURL_THUMB_LOW_END_ANGLE = 70f;
        private const float CURL_THUMB_HIGH_END_ANGLE = 100f;
        private const float CURL_THUMB_DISTANCE = CURL_THUMB_HIGH_END_ANGLE - CURL_THUMB_LOW_END_ANGLE;

        private const float CURL_INDEX_LOW_END_ANGLE = 18f;
        private const float CURL_INDEX_HIGH_END_ANGLE = 120f;
        private const float CURL_INDEX_DISTANCE = CURL_INDEX_HIGH_END_ANGLE - CURL_INDEX_LOW_END_ANGLE;

        private const float CURL_MIDDLE_LOW_END_ANGLE = 16f;
        private const float CURL_MIDDLE_HIGH_END_ANGLE = 150f;
        private const float CURL_MIDDLE_DISTANCE = CURL_MIDDLE_HIGH_END_ANGLE - CURL_MIDDLE_LOW_END_ANGLE;

        private const float CURL_RING_LOW_END_ANGLE = 16f;
        private const float CURL_RING_HIGH_END_ANGLE = 160f;
        private const float CURL_RING_DISTANCE = CURL_RING_HIGH_END_ANGLE - CURL_RING_LOW_END_ANGLE;

        private const float CURL_LITTLE_LOW_END_ANGLE = 28f;
        private const float CURL_LITTLE_HIGH_END_ANGLE = 150f;
        private const float CURL_LITTLE_DISTANCE = CURL_LITTLE_HIGH_END_ANGLE - CURL_LITTLE_LOW_END_ANGLE;

        private const float CURL_TOTAL_DISTANCE = CURL_LITTLE_DISTANCE + CURL_RING_DISTANCE + CURL_MIDDLE_DISTANCE + CURL_INDEX_DISTANCE;
        private const float IS_GRIPPING_CURL_THRESHOLD = .9f;
        private const bool DEBUG_LOG_VALUES_TO_CONSOLE = true;

        /// <summary>
        /// Processes the hand data and updates its <see cref="HandData.IsGripping"/>,
        /// <see cref="HandData.GripStrength"/> and <see cref="HandData.FingerCurlStrengths"/> values.
        /// </summary>
        /// <param name="handData">The input hand data retrieved from platform conversion.</param>
        public void Process(HandData handData)
        {
            if (handData.IsTracked)
            {
                // Gather needed data for calculations.
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                var palmLookRotation = Quaternion.LookRotation(palmPose.Forward, palmPose.Up);
                var thumbMetacarpalPose = handData.Joints[(int)TrackedHandJoint.ThumbMetacarpal];
                var indexIntermediatePose = handData.Joints[(int)TrackedHandJoint.IndexIntermediate];
                var middleIntermediatePose = handData.Joints[(int)TrackedHandJoint.MiddleIntermediate];
                var ringIntermediatePose = handData.Joints[(int)TrackedHandJoint.RingIntermediate];
                var littleIntermediatePose = handData.Joints[(int)TrackedHandJoint.LittleIntermediate];

                // Calculate per finger curl angle.
                var thumbCurl = Quaternion.Angle(palmLookRotation, thumbMetacarpalPose.Rotation);
                var indexCurl = Quaternion.Angle(palmLookRotation, indexIntermediatePose.Rotation);
                var middleCurl = Quaternion.Angle(palmLookRotation, middleIntermediatePose.Rotation);
                var ringCurl = Quaternion.Angle(palmLookRotation, ringIntermediatePose.Rotation);
                var littleCurl = Quaternion.Angle(palmLookRotation, littleIntermediatePose.Rotation);
                // 70 - 100
                // Grip strength is defined as the total traveled curl distance
                // compared to the total travel curl distance for the whole hand.
                var totalCurlDistance =
                    littleCurl - CURL_LITTLE_LOW_END_ANGLE +
                    (ringCurl - CURL_RING_LOW_END_ANGLE) +
                    (middleCurl - CURL_MIDDLE_LOW_END_ANGLE) +
                    (indexCurl - CURL_INDEX_LOW_END_ANGLE);

                handData.GripStrength = Mathf.Clamp(totalCurlDistance / CURL_TOTAL_DISTANCE, 0f, 1f);
                handData.IsGripping = handData.GripStrength >= IS_GRIPPING_CURL_THRESHOLD;
                handData.FingerCurlStrengths = new float[]
                {
                    Mathf.Clamp((thumbCurl - CURL_THUMB_LOW_END_ANGLE) / CURL_THUMB_DISTANCE, 0f, 1f),
                    Mathf.Clamp((indexCurl - CURL_INDEX_LOW_END_ANGLE) / CURL_INDEX_DISTANCE, 0f, 1f),
                    Mathf.Clamp((middleCurl - CURL_MIDDLE_LOW_END_ANGLE) / CURL_MIDDLE_DISTANCE, 0f, 1f),
                    Mathf.Clamp((ringCurl - CURL_RING_LOW_END_ANGLE) / CURL_RING_DISTANCE, 0f, 1f),
                    Mathf.Clamp((littleCurl - CURL_LITTLE_LOW_END_ANGLE) / CURL_LITTLE_DISTANCE, 0f, 1f),
                };

                if (DEBUG_LOG_VALUES_TO_CONSOLE)
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
        }
    }
}