// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.OpenVR;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.WindowsMixedReality.Controllers;

namespace XRTK.WindowsMixedReality.Profiles
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mappings/Windows Mixed Reality Controller Mapping Profile", fileName = "WindowsMixedRealityControllerMappingProfile")]
    public class WindowsMixedRealityMotionControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.WindowsMixedReality;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}MotionController";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Windows Mixed Reality HoloLens Hand Input", typeof(WindowsMixedRealityController)),
                    new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Left", typeof(WindowsMixedRealityController), Handedness.Left),
                    new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Right", typeof(WindowsMixedRealityController), Handedness.Right),
                    new MixedRealityControllerMapping("Open VR Motion Controller Left", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
                    new MixedRealityControllerMapping("Open VR Motion Controller Right", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
                };
            }

            base.Awake();
        }
    }
}