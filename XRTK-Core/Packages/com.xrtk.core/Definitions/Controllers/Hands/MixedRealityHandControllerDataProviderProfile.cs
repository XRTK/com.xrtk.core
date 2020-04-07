// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Definitions.Controllers.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mappings/Hand Controller Mapping Profile", fileName = "HandControllerMappingProfile")]
    public class MixedRealityHandControllerDataProviderProfile : BaseHandControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MixedRealityHandController), Handedness.Left),
                new ControllerDefinition(typeof(MixedRealityHandController), Handedness.Right),
            };
        }
    }
}