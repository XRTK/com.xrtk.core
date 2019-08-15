// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    public class LeapMotionHandControllerDataProvider : BaseDataProvider, IMixedRealityPlatformHandControllerDataProvider
    {
        public LeapMotionHandControllerDataProvider(string name, uint priority) : base(name, priority)
        {
        }

        public IMixedRealityController[] GetActiveControllers()
        {
            throw new System.NotImplementedException();
        }
    }
}