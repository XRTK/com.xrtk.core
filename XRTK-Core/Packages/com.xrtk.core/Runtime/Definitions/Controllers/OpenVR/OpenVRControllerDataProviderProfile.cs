// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.OpenVR;

namespace XRTK.Definitions.Controllers.OpenVR.Profiles
{
    public class OpenVRControllerDataProviderProfile : UnityInputControllerDataProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                //new ControllerDefinition(typeof(GenericOpenVRController), Handedness.None, true),
                //new ControllerDefinition(typeof(GenericOpenVRController), Handedness.Left, true),
                //new ControllerDefinition(typeof(GenericOpenVRController), Handedness.Right, true),
                new ControllerDefinition(typeof(OculusGoOpenVRController)),
                //new ControllerDefinition(typeof(OculusRemoteOpenVRController)),
                new ControllerDefinition(typeof(OculusTouchOpenVRController), Handedness.Left),
                new ControllerDefinition(typeof(OculusTouchOpenVRController), Handedness.Right),
                new ControllerDefinition(typeof(ViveWandOpenVRController), Handedness.Left),
                new ControllerDefinition(typeof(ViveWandOpenVRController), Handedness.Right),
                new ControllerDefinition(typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
                new ControllerDefinition(typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
            };
        }
    }
}
