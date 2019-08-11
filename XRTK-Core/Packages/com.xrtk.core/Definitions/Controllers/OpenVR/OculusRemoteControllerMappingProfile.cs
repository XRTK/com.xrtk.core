// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;

namespace XRTK.Definitions.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Oculus Remote Controller Mapping Profile", fileName = "OculusRemoteControllerMappingProfile")]
    public class OculusRemoteControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.OculusRemote;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}OculusRemoteController";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Oculus Remote Controller", typeof(OculusRemoteOpenVRController)),
                };
            }

            base.Awake();
        }
    }
}