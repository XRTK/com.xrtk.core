// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    public static class HandUtils
    {
        /// <summary>
        /// Converts an input array of joint poses to a dictionary of join keys and poses.
        /// </summary>
        /// <param name="poses">Poses array.</param>
        /// <returns>Dictionary of joint keys and their poses.</returns>
        public static IReadOnlyDictionary<TrackedHandJoint, MixedRealityPose> ToJointPoseDictionary(MixedRealityPose[] poses)
        {
            Dictionary<TrackedHandJoint, MixedRealityPose> dict = new Dictionary<TrackedHandJoint, MixedRealityPose>();
            for (int i = 0; i < poses.Length; i++)
            {
                dict.Add((TrackedHandJoint)i, poses[i]);
            }

            return dict;
        }
    }
}