// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.OpenVR
{
    public class GenericOpenVRControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
            // new MixedRealityControllerMapping("Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
            throw new System.NotImplementedException();
        }
    }
}