// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// Simulation input at a given time. Best described as the "sensor data" for
    /// hand simulation updates.
    /// </summary>
    public class SimulationInput
    {
        /// <summary>
        /// If true then the hand is always visible,, even if it's currently
        /// not being tracked. This feature is unique to simulation.
        /// </summary>
        public bool IsAlwaysVisible { get; set; }

        /// <summary>
        /// If true, the hand is currently being tracked.
        /// </summary>
        public bool IsTracking { get; set; }

        /// <summary>
        /// The computed input based hand position delta since the last update.
        /// </summary>
        public Vector3 HandPositionDelta { get; set; }

        /// <summary>
        /// The computed input based hand rotation delta since the last update.
        /// </summary>
        public Vector3 HandRotationDelta { get; set; }

        /// <summary>
        /// The requested hand pose as determined by user input.
        /// </summary>
        public SimulatedHandPose HandPose { get; set; }
    }
}