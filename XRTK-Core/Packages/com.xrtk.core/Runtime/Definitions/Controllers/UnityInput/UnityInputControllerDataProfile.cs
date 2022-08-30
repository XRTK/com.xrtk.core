// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Providers.Controllers.UnityInput;

namespace XRTK.Definitions.Controllers.UnityInput.Profiles
{
    public class UnityInputControllerDataProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(GenericJoystickController)),
                new ControllerDefinition(typeof(XboxController))
            };
        }
    }
}
