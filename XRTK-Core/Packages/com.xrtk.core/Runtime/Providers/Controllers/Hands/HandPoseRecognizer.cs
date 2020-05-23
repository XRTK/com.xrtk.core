// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Services;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// A recognizer manages a list of recorded hand poses and
    /// may be used to recognize any of those poses during runtime.
    /// </summary>
    public sealed class HandPoseRecognizer
    {
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
        public void Recognize(HandData handData, Handedness handedness)
        {
            var localJointPoses = ParseFromJointPoses(handData.Joints, handedness, handData.RootPose);
            var currentHighestProbability = 0f;
            HandControllerPoseDefinition recognizedPose = null;

            for (int i = 0; i < poseHandDatas.Length; i++)
            {
                var recordedData = poseHandDatas[i];
                var probability = Compare(recordedData.Joints, localJointPoses, .1f);

                if (probability > currentHighestProbability)
                {
                    currentHighestProbability = probability;
                    recognizedPose = poseDefinitions[i];
                }
            }

            handData.TrackedPose = recognizedPose;

            if (handData.TrackedPose != null)
            {
                Debug.Log(handData.TrackedPose.Id);
            }
        }

        private float Compare(MixedRealityPose[] recordedJointPoses, MixedRealityPose[] runtimeJointPoses, float threshold)
        {
            // Variable keeps count of how many joint poses have passed
            // the test for equality.
            var passed = 0;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var recordedJointPosition = recordedJointPoses[i].Position;
                var runtimeJointPosition = runtimeJointPoses[i].Position;
                var delta = Vector3.Distance(recordedJointPosition, runtimeJointPosition);

                // If the delta is below threshold we consider the joint
                // test passed.
                if (delta < threshold)
                {
                    passed++;
                }
            }

            // The more joints have passed the test, the more likely it is
            // the poses are the same.
            return passed / HandData.JointCount;
        }

        /// <summary>
        /// Takes world space joint poses from any hand and converts them into right-hand, camera-space poses.
        /// </summary>
        private MixedRealityPose[] ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, MixedRealityPose rootPose)
        {
            var localJointPoses = new MixedRealityPose[joints.Length];
            var invRotation = Quaternion.Inverse(rootPose.Rotation);
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.rotation
                : CameraCache.Main.transform.rotation);

            for (int i = 0; i < HandData.JointCount; i++)
            {
                Vector3 p = joints[i].Position;
                Quaternion r = joints[i].Rotation;

                // Apply inverse external transform
                p = invRotation * (p - rootPose.Position);
                r = invRotation * r;

                // To camera space
                p = invCameraRotation * p;
                r = invCameraRotation * r;

                // Pose offset are for right hand, mirror on X axis if left hand is given
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                localJointPoses[i] = new MixedRealityPose(p, r);
            }

            return localJointPoses;
        }
    }
}