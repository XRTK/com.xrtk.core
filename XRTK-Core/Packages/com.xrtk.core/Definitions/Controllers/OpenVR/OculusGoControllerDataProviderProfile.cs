// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.OpenVR
{
    public class OculusGoControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Oculus Go Controller", typeof(OculusGoOpenVRController), Handedness.Both),
            throw new System.NotImplementedException();
        }
    }
}