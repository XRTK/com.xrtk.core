// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Services.InputSystem.Simulation
{
    /// <summary>
    /// Snapshot of simulated hand data.
    /// </summary>
    [Serializable]
    public class SimulatedHandData : HandData
    {
        [SerializeField]
        private bool isPinching = false;
        public bool IsPinching => isPinching;

        public delegate void HandJointDataGenerator(MixedRealityPose[] jointPositions);

        public bool Update(bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            return UpdateWithTimestamp(DateTime.UtcNow.Ticks, isTrackedNew, isPinchingNew, generator);
        }

        public bool UpdateWithTimestamp(long timestampNew, bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            bool handDataChanged = false;

            if (IsTracked != isTrackedNew || isPinching != isPinchingNew)
            {
                IsTracked = isTrackedNew;
                isPinching = isPinchingNew;
                handDataChanged = true;
            }

            if (Timestamp != timestampNew)
            {
                Timestamp = timestampNew;
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