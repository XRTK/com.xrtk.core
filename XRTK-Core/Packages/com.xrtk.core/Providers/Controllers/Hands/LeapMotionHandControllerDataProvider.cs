// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Providers.Controllers.Hands
{
    public class LeapMotionHandControllerDataProvider : BaseHandControllerDataProvider
    {
        public LeapMotionHandControllerDataProvider(string name, uint priority, HandControllerDataProviderProfile profile)
            : base(name, priority, profile) { }
    }
}