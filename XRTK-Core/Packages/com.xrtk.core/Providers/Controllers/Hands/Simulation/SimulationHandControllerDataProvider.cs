// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.Simulation;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    public class SimulationHandControllerDataProvider : BaseHandControllerDataProvider<SimulationHandController>
    {
        private SimulationTimeStampStopWatch handUpdateStopWatch;
        private long lastHandUpdateTimeStamp = 0;

        private bool leftHandIsAlwaysVisible = false;
        private bool rightHandIsAlwaysVisible = false;
        private bool leftHandIsTracked = false;
        private bool rightHandIsTracked = false;

        /// <summary>
        /// Gets the active profile for the simulation data provider.
        /// </summary>
        public SimulationHandControllerDataProviderProfile Profile { get; }

        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Hand controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public SimulationHandControllerDataProvider(string name, uint priority, SimulationHandControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            Profile = profile;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
        }

        /// <inheritdoc />
        protected override void RefreshActiveControllers()
        {
            DateTime currentTime = handUpdateStopWatch.Current;
            double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandUpdateTimeStamp)).TotalMilliseconds;
            if (msSinceLastHandUpdate > Profile.SimulatedUpdateFrequency)
            {
                if (Input.GetKeyDown(Profile.ToggleLeftHandKey))
                {
                    leftHandIsAlwaysVisible = !leftHandIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.LeftHandTrackedKey))
                {
                    leftHandIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.LeftHandTrackedKey))
                {
                    leftHandIsTracked = false;
                }

                if (leftHandIsAlwaysVisible || leftHandIsTracked)
                {
                    GetOrAddController(Handedness.Left);
                }
                else
                {
                    RemoveController(Handedness.Left);
                }

                if (Input.GetKeyDown(Profile.ToggleRightHandKey))
                {
                    rightHandIsAlwaysVisible = !rightHandIsAlwaysVisible;
                }

                if (Input.GetKeyDown(Profile.RightHandTrackedKey))
                {
                    rightHandIsTracked = true;
                }

                if (Input.GetKeyUp(Profile.RightHandTrackedKey))
                {
                    rightHandIsTracked = false;
                }

                if (rightHandIsAlwaysVisible || rightHandIsTracked)
                {
                    GetOrAddController(Handedness.Right);
                }
                else
                {
                    RemoveController(Handedness.Right);
                }

                lastHandUpdateTimeStamp = currentTime.Ticks;
            }
        }
    }
}