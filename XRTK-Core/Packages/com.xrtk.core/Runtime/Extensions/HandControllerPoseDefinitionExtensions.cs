// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HandControllerPoseDefinition"/>
    /// </summary>
    public static class HandControllerPoseDefinitionExtensions
    {
        /// <summary>
        /// Converts a pose definition into <see cref="HandData"/> representing
        /// the pose.
        /// </summary>
        /// <param name="pose">The pose to convert.</param>
        /// <returns><see cref="HandData"/> object for the pose.</returns>
        public static HandData ToHandData(this HandControllerPoseDefinition pose)
        {
            var handData = new HandData();
            var recordedHandData = JsonUtility.FromJson<RecordedHandJoints>(pose.Data.text);

            for (int j = 0; j < recordedHandData.Joints.Length; j++)
            {
                var jointRecord = recordedHandData.Joints[j];
                handData.Joints[(int)jointRecord.Joint] = jointRecord.Pose;
            }

            return handData;
        }
    }
}