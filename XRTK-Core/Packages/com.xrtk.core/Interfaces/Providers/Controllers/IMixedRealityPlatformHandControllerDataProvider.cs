// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Interfaces.Providers.Controllers
{
    public delegate void HandDataUpdate(Handedness handedness, HandData handData);

    public interface IMixedRealityPlatformHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Fired whenever updated hand data is available.
        /// </summary>
        event HandDataUpdate OnHandDataUpdate;
    }
}
