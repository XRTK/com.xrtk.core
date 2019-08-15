// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Services.InputSystem.Simulation
{
    /// <summary>
    /// Snapshot of simulated hand data.
    /// </summary>
    [Serializable]
    public class SimulatedHandData
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. DateTime.UtcNow.ToFileTime().
        /// </summary>
        public long Timestamp { get; private set; } = 0;

        [SerializeField]
        private bool isTracked = false;
        public bool IsTracked => isTracked;

        [SerializeField]
        private MixedRealityPose[] joints = new MixedRealityPose[jointCount];
        public MixedRealityPose[] Joints => joints;

        [SerializeField]
        private bool isPinching = false;
        public bool IsPinching => isPinching;

        public delegate void HandJointDataGenerator(MixedRealityPose[] jointPositions);

        public void Copy(SimulatedHandData other)
        {
            Timestamp = other.Timestamp;
            isTracked = other.isTracked;
            isPinching = other.isPinching;
            for (int i = 0; i < jointCount; ++i)
            {
                joints[i] = other.joints[i];
            }
        }

        public bool Update(bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            return UpdateWithTimestamp(DateTime.UtcNow.Ticks, isTrackedNew, isPinchingNew, generator);
        }

        public bool UpdateWithTimestamp(long timestampNew, bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            bool handDataChanged = false;

            if (isTracked != isTrackedNew || isPinching != isPinchingNew)
            {
                isTracked = isTrackedNew;
                isPinching = isPinchingNew;
                handDataChanged = true;
            }

            if (Timestamp != timestampNew)
            {
                Timestamp = timestampNew;
                if (isTracked)
                {
                    generator(Joints);
                    handDataChanged = true;
                }
            }

            return handDataChanged;
        }
    }
}