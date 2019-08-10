// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Hand Controller Mapping Profile", fileName = "MixedRealityHandControllerMappingProfile")]
    public class MixedRealityHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.Hand;

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Articulated", typeof(BaseHandController), Definitions.Utilities.Handedness.Left),
                    new MixedRealityControllerMapping("Articulated", typeof(BaseHandController), Definitions.Utilities.Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}