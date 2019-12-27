// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Simulation Hand Controller Mapping Profile", fileName = "SimulationHandControllerMappingProfile")]
    public class SimulationHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.Hand;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}Hand";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Simulation Hand Controller Left", typeof(SimulationHandController), Handedness.Left),
                    new MixedRealityControllerMapping("Simulation Hand Controller Right", typeof(SimulationHandController), Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}