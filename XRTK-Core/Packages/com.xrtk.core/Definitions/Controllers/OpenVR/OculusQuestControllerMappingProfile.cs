// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Oculus Quest Controller Mapping Profile", fileName = "OculusQuestControllerMappingProfile")]
    public class OculusQuestControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.OculusQuest;

        /// <inheritdoc />
        public override string TexturePath => $"{base.TexturePath}OculusControllersTouch";

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Oculus Quest Controller Left", typeof(OculusQuestController), Handedness.Left),
                    new MixedRealityControllerMapping("Oculus Quest Controller Right", typeof(OculusQuestController), Handedness.Right),
                };
            }

            base.Awake();
        }
    }
}