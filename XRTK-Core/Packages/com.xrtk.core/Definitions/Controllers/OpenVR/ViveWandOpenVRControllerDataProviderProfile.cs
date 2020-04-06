// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.OpenVR
{
    public class ViveWandOpenVRControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetControllerDefinitions()
        {
            // new MixedRealityControllerMapping("Vive Wand OpenVR Controller Left", typeof(ViveWandOpenVRController), Handedness.Left),
            // new MixedRealityControllerMapping("Vive Wand OpenVR Controller Right", typeof(ViveWandOpenVRController), Handedness.Right),
            throw new System.NotImplementedException();
        }
    }
}