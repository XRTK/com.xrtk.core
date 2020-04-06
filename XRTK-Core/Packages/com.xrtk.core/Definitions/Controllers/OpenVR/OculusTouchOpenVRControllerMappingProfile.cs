// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.OpenVR;

namespace XRTK.Definitions.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Oculus Touch OpenVR Controller", fileName = "OculusTouchOpenVRControllerMappingProfile")]
    public class OculusTouchOpenVRControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.OculusTouch;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}OculusControllersTouch";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Oculus Touch OpenVR Controller Left", typeof(OculusTouchOpenVRController), Handedness.Left),
                    new MixedRealityControllerMapping("Oculus Touch OpenVR Controller Right", typeof(OculusTouchOpenVRController), Handedness.Right),
                };
            }

            base.Awake();
        }
    }
}