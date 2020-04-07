// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.Simulation.Hands;

namespace XRTK.Definitions.Controllers.Simulation.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Simulated Hand Controller Mapping Profile", fileName = "SimulatedHandControllerMappingProfile")]
    public class SimulatedHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
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
                    new MixedRealityControllerMapping("Simulated Hand Controller Left", typeof(SimulatedHandController), Handedness.Left),
                    new MixedRealityControllerMapping("Simulated Hand Controller Right", typeof(SimulatedHandController), Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}