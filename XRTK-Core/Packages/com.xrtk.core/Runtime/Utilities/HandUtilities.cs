// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Utilities
{
    /// <summary>
    /// Hand controller utilities.
    /// </summary>
    public static class HandUtilities
    {
        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedIndexMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 indexMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .2f);
            Quaternion indexMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].Rotation;

            return new MixedRealityPose(indexMetacarpalPosition, indexMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedRingMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 ringMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .8f);
            Quaternion ringMetacarpalRotation = jointPoses[(int)TrackedHandJoint.LittleMetacarpal].Rotation;

            return new MixedRealityPose(ringMetacarpalPosition, ringMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedMiddleMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 middleMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .5f);
            Quaternion middleMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].Rotation;

            return new MixedRealityPose(middleMetacarpalPosition, middleMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.Palm"/> pose.
        /// Requires known <see cref="TrackedHandJoint.MiddleMetacarpal"/> and
        /// <see cref="TrackedHandJoint.MiddleProximal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.Palm"/> pose.</returns>
        public static MixedRealityPose GetEstimatedPalmPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose middleMetacarpalPose = GetEstimatedMiddleMetacarpalPose(jointPoses);
            MixedRealityPose middleProximalPose = jointPoses[(int)TrackedHandJoint.MiddleProximal];

            Vector3 palmPosition = Vector3.Lerp(middleMetacarpalPose.Position, middleProximalPose.Position, .5f);
            Quaternion palmRotation = middleMetacarpalPose.Rotation;

            return new MixedRealityPose(palmPosition, palmRotation);
        }
    }
}