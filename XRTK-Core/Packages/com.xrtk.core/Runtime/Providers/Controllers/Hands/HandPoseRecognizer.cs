// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// The hand pose recongizer uses the recorded hand pose definitions
    /// configured in <see cref="Definitions.InputSystem.MixedRealityInputSystemProfile.TrackedPoses"/>
    /// or the platform's <see cref="BaseHandControllerDataProviderProfile.TrackedPoses"/>
    /// and attempts to recognize them during runtime.
    /// </summary>
    public sealed class HandPoseRecognizer
    {
        private const float POSITION_DELTA_SQR_MAGNITUDE_THRESHOLD = .01f;
        private const float ROTATION_DELTA_ANGLE_THRESHOLD = 2f;

        /// <summary>
        /// Creates a new recognizer instance to work on the provided list of poses.
        /// </summary>
        /// <param name="recognizablePoses">Recognizable poses by this recognizer.</param>
        public HandPoseRecognizer(IReadOnlyList<HandControllerPoseDefinition> recognizablePoses)
        {
            poseHandDatas = new HandData[recognizablePoses.Count];
            poseDefinitions = new Dictionary<int, HandControllerPoseDefinition>();

            var i = 0;
            foreach (var item in recognizablePoses)
            {
                poseHandDatas[i] = item.ToHandData();
                poseDefinitions.Add(i, item);

                i++;
            }
        }

        private readonly HandData[] poseHandDatas;
        private readonly Dictionary<int, HandControllerPoseDefinition> poseDefinitions;

        /// <summary>
        /// Attempts to recognize a hand pose.
        /// </summary>
        /// <param name="handData">The hand data to use for recognition.</param>
        public void Recognize(HandData handData)
        {
            var currentHighestProbability = 0f;
            HandControllerPoseDefinition recognizedPose = null;

            for (int i = 0; i < poseHandDatas.Length; i++)
            {
                var recordedData = poseHandDatas[i];
                var probability = Compare(handData.Handedness, recordedData.Joints, handData.Joints);

                //Debug.Log($"{poseDefinitions[i].Id} | {probability}");
                if (probability > currentHighestProbability)
                {
                    currentHighestProbability = probability;
                    recognizedPose = poseDefinitions[i];
                }
            }

            handData.TrackedPose = recognizedPose;

            if (handData.TrackedPose != null)
            {
                //Debug.Log(handData.TrackedPose.Id);
            }
        }

        private float Compare(Handedness handedness, MixedRealityPose[] recordedJointPoses, MixedRealityPose[] runtimeJointPoses)
        {
            // Variable keeps count of how many joint poses have passed
            // the test for equality.
            var passed = 0;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var recordedJointPose = recordedJointPoses[i];
                var recordedJointPosition = recordedJointPose.Position;
                var recordedJointRotation = recordedJointPose.Rotation;

                // Recorded poses are for right hand, mirror on X axis if left hand is given.
                if (handedness == Handedness.Left)
                {
                    recordedJointPosition.x = -recordedJointPosition.x;
                    recordedJointRotation.y = -recordedJointRotation.y;
                    recordedJointRotation.z = -recordedJointRotation.z;
                }

                var runtimeJointPose = runtimeJointPoses[i];
                var runtimeJointPosition = runtimeJointPose.Position;
                var runtimeJointRotation = runtimeJointPose.Rotation;
                var deltaPosition = (runtimeJointPosition - recordedJointPosition).sqrMagnitude;

                // If the delta is below threshold we consider the joint
                // test passed.
                if (deltaPosition < POSITION_DELTA_SQR_MAGNITUDE_THRESHOLD &&
                    runtimeJointRotation.Approximately(recordedJointRotation, ROTATION_DELTA_ANGLE_THRESHOLD))
                {
                    passed++;
                }
            }

            // The more joints have passed the test, the more likely it is
            // the poses are the same.
            return passed / HandData.JointCount;
        }
    }
}