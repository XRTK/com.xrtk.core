// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;

namespace XRTK.Providers.Controllers.Hands.LeapMotion
{
    public class LeapMotionHandControllerDataProvider : BaseHandControllerDataProvider<LeapMotionHandController>
    {
        public LeapMotionHandControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile) { }

        protected override void RefreshActiveControllers()
        {

        }
    }
}