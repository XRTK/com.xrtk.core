// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.UnityInput.Profiles
{
    public class GenericUnityControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Generic Unity Controller", typeof(GenericJoystickController), Handedness.None, true),
            throw new System.NotImplementedException();
        }
    }
}