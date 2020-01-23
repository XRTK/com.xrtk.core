// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Definitions.Controllers.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Hand Controller Mapping Profile", fileName = "HandControllerMappingProfile")]
    public class MixedRealityHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.Hand;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}Hand";

        /// <inheritdoc />
        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Hand Controller Left", typeof(MixedRealityHandController), Handedness.Left),
                    new MixedRealityControllerMapping("Hand Controller Right", typeof(MixedRealityHandController), Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}