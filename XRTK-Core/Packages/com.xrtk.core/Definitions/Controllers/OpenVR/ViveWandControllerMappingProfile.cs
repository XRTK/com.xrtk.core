// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.OpenVR;

namespace XRTK.Definitions.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Vive Wand Controller Mapping Profile", fileName = "ViveWandControllerMappingProfile")]
    public class ViveWandControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.ViveWand;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}ViveWandController";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Vive Wand Controller Left", typeof(ViveWandOpenVRController), Handedness.Left),
                    new MixedRealityControllerMapping("Vive Wand Controller Right", typeof(ViveWandOpenVRController), Handedness.Right),
                };
            }

            base.Awake();
        }
    }
}