// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Interfaces.Providers.Controllers.Hands
{
    /// <summary>
    /// <see cref="HandData"/> post processor definition.
    /// A post processor may recieve <see cref="HandData"/>
    /// from a <see cref="IMixedRealityHandControllerDataProvider"/> just before the actual
    /// <see cref="IMixedRealityHandController"/> is updated with it to perform last minute
    /// processing on it.
    /// </summary>
    public interface IHandDataPostProcessor
    {
        /// <summary>
        /// Performs post processing on the provided <see cref="HandData"/>.
        /// </summary>
        /// <param name="handedness">The <see cref="Handedness"/> of the <see cref="IMixedRealityHandController"/> the
        /// data is being prepared for.</param>
        /// <param name="handData">The <see cref="HandData"/> provided by the <see cref="IMixedRealityHandControllerDataProvider"/>.</param>
        /// <returns>Returns modified <see cref="HandData"/> after post processing was applied.</returns>
        HandData PostProcess(Handedness handedness, HandData handData);
    }
}