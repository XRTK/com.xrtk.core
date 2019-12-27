// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// Simulatd pose of an hand defined by joint poses. Used by <see cref="SimulationHandControllerDataProvider"/> to simulate hand tracking.
    /// </summary>
    public class SimulationHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
        private static readonly Dictionary<string, SimulationHandPose> handPoses = new Dictionary<string, SimulationHandPose>();
        private static bool isInitialized = false;

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        private MixedRealityPose[] localJointPoses;

        /// <summary>
        /// Gets the unique identifier for the simulated pose.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the configured default pose for simulation hands.
        /// </summary>
        public static SimulationHandPoseData DefaultHandPose { get; private set; }

        /// <summary>
        /// Creates a new hand pose with all joints in their
        /// local space origin.
        /// </summary>
        /// <param name="id">Unique pose identfier.</param>
        public SimulationHandPose(string id)
        {
            Id = id;
            localJointPoses = new MixedRealityPose[jointCount];
            SetZero();
        }

        /// <summary>
        /// Creates a new hand pose using local pose data for the hand's joints.
        /// </summary>
        /// <param name="id">Unique pose identfier.</param>
        /// <param name="localJointPoses">Joint poses in local space.</param>
        public SimulationHandPose(string id, MixedRealityPose[] localJointPoses) : this(id)
        {
            Array.Copy(localJointPoses, this.localJointPoses, jointCount);
        }

        /// <summary>
        /// Initialize pose data for use in editor from files.
        /// </summary>
        /// <param name="poses">List of pose data assets with pose information.</param>
        public static void Initialize(IEnumerable<SimulationHandPoseData> poses)
        {
            if (isInitialized)
            {
                return;
            }

            // To stabilize the simulated hand poses, we look
            // for the hand "open" pose, which we will use as a reference to offset
            // all other poses, so the "Palm" joint position stays the same for all simulated
            // poses. If no open pose is defined, we can't do anything and keep everythign as it is.
            SimulationHandPose openPose = null;
            foreach (SimulationHandPoseData poseData in poses)
            {
                if (poseData.Id.Equals("open"))
                {
                    openPose = new SimulationHandPose(poseData.Id);
                    openPose.FromJson(poseData.Data.text);
                    handPoses.Add(openPose.Id, openPose);
                }
            }

            foreach (var poseData in poses)
            {
                if (poseData.Data != null)
                {
                    if (openPose != null)
                    {
                        // If we already found the open pose, we don't want it initialized again, since we took
                        // care of that above.
                        if (!poseData.Id.Equals("open"))
                        {
                            // We have open pose data, offset this pose using it's palm position.
                            SimulationHandPose pose = new SimulationHandPose(poseData.Id);
                            pose.FromJson(poseData.Data.text);
                            OffsetJointsRelativeToOpenPosePalmPosition(openPose, pose);
                            handPoses.Add(pose.Id, pose);
                        }
                    }
                    else
                    {
                        SimulationHandPose pose = new SimulationHandPose(poseData.Id);
                        pose.FromJson(poseData.Data.text);
                        handPoses.Add(pose.Id, pose);
                    }
                }

                if (poseData.IsDefault)
                {
                    DefaultHandPose = poseData;
                }
            }

            isInitialized = true;
        }

        private static void OffsetJointsRelativeToOpenPosePalmPosition(SimulationHandPose openPose, SimulationHandPose pose)
        {
            Vector3 openHandPalmPosition = openPose.localJointPoses[(int)TrackedHandJoint.Palm].Position;
            Vector3 posePalmPosition = pose.localJointPoses[(int)TrackedHandJoint.Palm].Position;
            Vector3 offset = posePalmPosition - openHandPalmPosition;

            for (int i = 0; i < pose.localJointPoses.Length; i++)
            {
                pose.localJointPoses[i].Position -= offset;
            }
        }

        /// <summary>
        /// Computes world space poses from camera-space joint data.
        /// </summary>
        public void ComputeJointPoses(Handedness handedness, Quaternion rotation, Vector3 position, MixedRealityPose[] jointsOut)
        {
            Quaternion cameraRotation = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation;

            for (int i = 0; i < jointCount; i++)
            {
                // Initialize from local offsets
                Vector3 p = localJointPoses[i].Position;
                Quaternion r = localJointPoses[i].Rotation;

                // Pose offset are for right hand, mirror on X axis if left hand is needed
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                // Apply camera transform
                p = cameraRotation * p;
                r = cameraRotation * r;

                // Apply external transform
                p = position + rotation * p;
                r = rotation * r;

                jointsOut[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Takes world space joint poses from any hand and convert into right-hand, camera-space poses.
        /// </summary>
        public void ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation);

            for (int i = 0; i < jointCount; i++)
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
        }

        /// <summary>
        /// Set all poses to zero.
        /// </summary>
        public void SetZero()
        {
            for (int i = 0; i < jointCount; i++)
            {
                localJointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
        }

        /// <summary>
        /// Copy data from another simulated hand pose.
        /// </summary>
        public void Copy(SimulationHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, jointCount);
        }

        /// <summary>
        /// Interpolates between the poses a and b by the interpolant t.
        /// </summary>
        /// <param name="a">Pose at t = 0.</param>
        /// <param name="b">Pose at t = 1.</param>
        /// <param name="t">The parameter t is clamped to the range [0,1].</param>
        public static SimulationHandPose Lerp(SimulationHandPose a, SimulationHandPose b, float t)
        {
            MixedRealityPose[] updatedJointPoses = new MixedRealityPose[jointCount];

            for (int i = 0; i < jointCount; i++)
            {
                MixedRealityPose jointPoseA = a.localJointPoses[i];
                MixedRealityPose jointPoseB = b.localJointPoses[i];

                Vector3 position = Vector3.Lerp(jointPoseA.Position, jointPoseB.Position, t);
                Quaternion rotation = Quaternion.Slerp(jointPoseA.Rotation, jointPoseB.Rotation, t);

                updatedJointPoses[i] = new MixedRealityPose(position, rotation);
            }

            return new SimulationHandPose(t >= 1 ? b.Id : a.Id, updatedJointPoses);
        }

        /// <summary>
        /// Gets the pose for a given pose name, if it's registered.
        /// </summary>
        /// <param name="name">The name of the pose.</param>
        /// <param name="pose">Hand pose reference.</param>
        /// <returns>True, if found.</returns>
        public static bool TryGetPoseByName(string name, out SimulationHandPose pose)
        {
            if (handPoses.TryGetValue(name, out SimulationHandPose p))
            {
                pose = p;
                return true;
            }

            pose = null;
            return false;
        }

        /// <summary>
        /// Gets the pose for a given pose name. Will throw it pose not registered.
        /// </summary>
        /// <param name="name">The name of the pose to lookup.</param>
        /// <returns>Simulated hand pose.</returns>
        public static SimulationHandPose GetPoseByName(string name)
        {
            return handPoses[name];
        }

        /// <summary>
        /// Serialize pose data to JSON format.
        /// </summary>
        public string ToJson()
        {
            var dict = new HandJointPoseDictionary();
            dict.FromJointPoses(localJointPoses);
            return JsonUtility.ToJson(dict);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            HandJointPoseDictionary dict = JsonUtility.FromJson<HandJointPoseDictionary>(json);
            dict.ToJointPoses(localJointPoses);
        }
    }
}