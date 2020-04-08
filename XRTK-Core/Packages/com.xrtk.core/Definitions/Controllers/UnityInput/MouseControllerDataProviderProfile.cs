// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.UnityInput.Profiles
{
    public class MouseControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MouseController), Handedness.Any, true),
            };
        }
    }
}