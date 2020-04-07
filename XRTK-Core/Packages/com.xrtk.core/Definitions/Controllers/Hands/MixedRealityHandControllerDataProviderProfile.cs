// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Controllers.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Hand Controller Mapping Profile", fileName = "HandControllerMappingProfile")]
    public class MixedRealityHandControllerDataProviderProfile : BaseHandControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Hand Controller Left", typeof(MixedRealityHandController), Handedness.Left),
            // new MixedRealityControllerMapping("Hand Controller Right", typeof(MixedRealityHandController), Handedness.Right)
            throw new System.NotImplementedException();
        }
    }
}