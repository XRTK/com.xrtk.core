// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    /// <summary>
    /// Snapshot of simulated hand data.
    /// </summary>
    [Serializable]
    public class UnityEditorHandData : HandData
    {
        public bool IsPinching { get; private set; } = false;

        public delegate void HandJointDataGenerator(MixedRealityPose[] jointPositions);

        public bool Update(bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            return UpdateWithTimeStamp(DateTime.UtcNow.Ticks, isTrackedNew, isPinchingNew, generator);
        }

        public bool UpdateWithTimeStamp(long timestampNew, bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            bool handDataChanged = false;

            if (IsTracked != isTrackedNew || IsPinching != isPinchingNew)
            {
                IsTracked = isTrackedNew;
                IsPinching = isPinchingNew;
                handDataChanged = true;
            }

            if (TimeStamp != timestampNew)
            {
                TimeStamp = timestampNew;
                if (IsTracked)
                {
                    generator(Joints);
                    handDataChanged = true;
                }
            }

            return handDataChanged;
        }
    }
}