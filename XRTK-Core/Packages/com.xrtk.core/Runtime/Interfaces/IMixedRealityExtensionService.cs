// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces
{
    /// <summary>
    /// Generic interface for all optional <see cref="IMixedRealityService"/>s that can be added to the <see cref="Definitions.MixedRealityRegisteredServiceProvidersProfile"/>
    /// </summary>
    public interface IMixedRealityExtensionService : IMixedRealityService
    {
        // Empty for now, but it is used to filter out the valid class types in the inspector dropdown.
    }
}