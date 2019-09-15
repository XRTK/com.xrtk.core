// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Hands
{
    public class LeapMotionHandControllerDataProvider : BaseControllerDataProvider, IMixedRealityPlatformHandControllerDataProvider
    {
        /// <inheritdoc />
        public event HandDataUpdate OnHandDataUpdate;

        public LeapMotionHandControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }
    }
}