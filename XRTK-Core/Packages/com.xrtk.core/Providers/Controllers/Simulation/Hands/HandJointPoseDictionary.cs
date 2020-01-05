// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Utility class to serialize hand pose as a dictionary with full joint names.
    /// </summary>
    [Serializable]
    internal class HandJointPoseDictionary
    {
        private static readonly string[] jointNames = Enum.GetNames(typeof(TrackedHandJoint));
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public HandJointItem[] items = null;

        public void FromJointPoses(MixedRealityPose[] jointPoses)
        {
            items = new HandJointItem[jointCount];
            for (int i = 0; i < jointCount; ++i)
            {
                items[i].JointIndex = (TrackedHandJoint)i;
                items[i].pose = jointPoses[i];
            }
        }

        public void ToJointPoses(MixedRealityPose[] jointPoses)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                jointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
            foreach (var item in items)
            {
                jointPoses[(int)item.JointIndex] = item.pose;
            }
        }

        [Serializable]
        internal struct HandJointItem
        {
            public string joint;
            public MixedRealityPose pose;

            public TrackedHandJoint JointIndex
            {
                get
                {
                    int nameIndex = Array.FindIndex(jointNames, IsJointName);
                    if (nameIndex < 0)
                    {
                        Debug.LogError($"Joint name {joint} not in TrackedHandJoint enum");
                        return TrackedHandJoint.None;
                    }
                    return (TrackedHandJoint)nameIndex;
                }
                set { joint = jointNames[(int)value]; }
            }

            private bool IsJointName(string s)
            {
                return string.Equals(s, joint);
            }

            public HandJointItem(TrackedHandJoint joint, MixedRealityPose pose)
            {
                this.joint = jointNames[(int)joint];
                this.pose = pose;
            }
        }
    }
}