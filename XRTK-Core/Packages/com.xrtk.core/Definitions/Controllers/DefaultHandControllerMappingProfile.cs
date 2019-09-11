// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Providers.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Hand Controller Mapping Profile", fileName = "MixedRealityHandControllerMappingProfile")]
    public class DefaultHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
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
                    new MixedRealityControllerMapping("Generic", typeof(DefaultHandController), Handedness.Left),
                    new MixedRealityControllerMapping("Generic", typeof(DefaultHandController), Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}