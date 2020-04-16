// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Services;

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
        /// <param name="poseRecognizer">Pose recognizer instance to use for pose recognition.</param>
        protected BaseHandDataConverter(Handedness handedness, IReadOnlyList<HandControllerPoseDefinition> trackedPoses)
        {
            Handedness = handedness;
            poseCompareFrames = new HandPoseFrame[trackedPoses.Count];
            this.trackedPoses = new Dictionary<string, HandControllerPoseDefinition>();

            int i = 0;
            foreach (var item in trackedPoses)
            {
                var recordedHandData = JsonUtility.FromJson<RecordedHandJoints>(item.Data.text);
                var recordedLocalJointPoses = new MixedRealityPose[HandData.JointCount];
                for (int j = 0; j < recordedHandData.items.Length; j++)
                {
                    var jointRecord = recordedHandData.items[j];
                    recordedLocalJointPoses[(int)jointRecord.JointIndex] = jointRecord.pose;
                }

                poseCompareFrames[i] = new HandPoseFrame(item.Id, recordedLocalJointPoses);
                this.trackedPoses.Add(item.Id, item);

                i++;
            }
        }

        private readonly Dictionary<string, HandControllerPoseDefinition> trackedPoses;
        private readonly HandPoseFrame[] poseCompareFrames;

        /// <summary>
        /// The handedness this converter is converting to.
        /// </summary>
        protected Handedness Handedness { get; }

        /// <summary>
        /// Applies post conversion steps to the generated hand data.
        /// </summary>
        /// <param name="handData">The hand data retrieved from conversion.</param>
        protected void PostProcess(HandData handData)
        {
            if (handData.IsTracked)
            {
                if (TryRecognizePose(handData.Joints, out var recognizedPoseId))
                {
                    handData.PoseDefinition = recognizedPoseId;
                }
                else
                {
                    handData.PoseDefinition = null;
                }
            }
        }

        /// <summary>
        /// Tries to reconize the hands pose using the the provided <see cref="HandPoseRecognizer"/>.
        /// </summary>
        /// <param name="jointPoses">Local joint poses retrieved from initial conversion.</param>
        /// <param name="recognizedPose">The recognized pose ID, if any.</param>
        /// <returns>True, if a pose was recognized.</returns>
        protected bool TryRecognizePose(MixedRealityPose[] jointPoses, out HandControllerPoseDefinition recognizedPose)
        {
            var wristPose = jointPoses[(int)TrackedHandJoint.Wrist];
            var localJointPoses = ParseFromJointPoses(jointPoses, Handedness, wristPose.Rotation, wristPose.Position);
            var currentFrame = new HandPoseFrame(localJointPoses);

            for (int i = 0; i < poseCompareFrames.Length; i++)
            {
                var compareFrame = poseCompareFrames[i];

                if (compareFrame.Compare(currentFrame, .01f))
                {
                    recognizedPose = trackedPoses[compareFrame.Id];
                    return true;
                }
            }

            recognizedPose = null;
            return false;
        }

        /// <summary>
        /// Takes world space joint poses from any hand and converts them into right-hand, camera-space poses.
        /// </summary>
        protected MixedRealityPose[] ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var localJointPoses = new MixedRealityPose[joints.Length];
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.rotation);

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