// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.LeapMotion
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Leap Motion Hand Controller Mapping Profile", fileName = "LeapMotionHandControllerMappingProfile")]
    public class LeapMotionHandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
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
                    new MixedRealityControllerMapping("Leap Motion Hand Controller Left", typeof(LeapMotionHandController), Handedness.Left),
                    new MixedRealityControllerMapping("Leap Motion Hand Controller Right", typeof(LeapMotionHandController), Handedness.Right)
                };
            }

            base.Awake();
        }
    }
}