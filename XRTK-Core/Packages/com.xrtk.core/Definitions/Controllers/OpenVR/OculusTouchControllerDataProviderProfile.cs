// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.OpenVR
{
    public class OculusTouchControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Oculus Touch Controller Left", typeof(OculusTouchOpenVRController), Handedness.Left),
            // new MixedRealityControllerMapping("Oculus Touch Controller Right", typeof(OculusTouchOpenVRController), Handedness.Right),
            throw new System.NotImplementedException();
        }
    }
}