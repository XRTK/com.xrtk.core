// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.InputSystem.Simulation;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class EyeTrackingSimulationDataProvider : BaseSimulationDataProvider //, IMixedRealityEyeGazeDataProvider
    {
        private MixedRealityEyeTrackingInputSimulationDataProviderProfile profile;

        ///// <inheritdoc/>
        //IMixedRealityEyeSaccadeProvider IMixedRealityEyeGazeDataProvider.SaccadeProvider => null;

        ///// <inheritdoc/>
        //bool IMixedRealityEyeGazeDataProvider.SmoothEyeTracking { get; set; }

        public EyeTrackingSimulationDataProvider(string name, uint priority, MixedRealityEyeTrackingInputSimulationDataProviderProfile profile) : base(name, priority)
        {
            this.profile = profile;
        }

        public override void Update()
        {
            if (profile.SimulateEyePosition)
            {
                //    MixedRealityToolkit.InputSystem.EyeGazeProvider?.UpdateEyeGaze(this, new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward), DateTime.UtcNow);
            }
        }
    }
}