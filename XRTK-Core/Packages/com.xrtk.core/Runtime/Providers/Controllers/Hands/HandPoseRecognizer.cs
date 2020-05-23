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
    public class HandPoseRecognizer
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
        public void Recognize(HandData handData)
        {
            //var wristPose = handData.Joints[(int)TrackedHandJoint.Wrist];
            //var localJointPoses = ParseFromJointPoses(handData.Joints, Handedness, wristPose.Rotation, wristPose.Position);

            var lowestSumOfJointPairDistances = Mathf.Infinity;
            HandControllerPoseDefinition recognizedPose = null;

            for (int i = 0; i < poseHandDatas.Length; i++)
            {
                var recordedData = poseHandDatas[i];
                var sumOfJointPairDistances = Compare(recordedData, handData, 1f);

                if (sumOfJointPairDistances < lowestSumOfJointPairDistances)
                {
                    recognizedPose = poseDefinitions[i];
                }
            }

            handData.TrackedPose = recognizedPose;
            Debug.Log(handData.TrackedPose?.Id);
        }

        private float Compare(HandData recordedData, HandData runtimeData, float tolerance)
        {
            var sumOfJointPairDistances = Mathf.Infinity;

            Debug.Log(runtimeData.Joints[0].Position);

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var recordedJointPosition = recordedData.Joints[i].Position;
                var runtimeJointPosition = runtimeData.Joints[i].Position;
                var delta = Vector3.Distance(recordedJointPosition, runtimeJointPosition);

                // If the distance for any pair of joints exceeds the tolerance
                // we consider the poses to definitely not be equal.
                if (delta > tolerance)
                {
                    sumOfJointPairDistances = Mathf.Infinity;
                    break;
                }

                // Otherwise we add the delta to the sum of distances.
                sumOfJointPairDistances += delta;
            }

            // The higher the sum of distances the more unlikely
            // it is that a and b are the same pose.
            return sumOfJointPairDistances;
        }

        /// <summary>
        /// Takes world space joint poses from any hand and converts them into right-hand, camera-space poses.
        /// </summary>
        private MixedRealityPose[] ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var localJointPoses = new MixedRealityPose[joints.Length];
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.rotation
                : CameraCache.Main.transform.rotation);

            for (int i = 0; i < HandData.JointCount; i++)
            {
                Vector3 p = joints[i].Position;
                Quaternion r = joints[i].Rotation;

                // Apply inverse external transform
                p = invRotation * (p - position);
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