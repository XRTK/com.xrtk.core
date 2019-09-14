// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.OpenVR;

namespace XRTK.Definitions.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Generic OpenVR Controller Mapping Profile", fileName = "GenericOpenVRControllerMappingProfile")]
    public class GenericOpenVRControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.GenericOpenVR;

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
                    new MixedRealityControllerMapping("Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
                };
            }

            base.Awake();
        }
    }
}