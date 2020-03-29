// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces
{
    /// <summary>
    /// Generic interface for all optional <see cref="IMixedRealityDataProvider"/>s that can be added to the <see cref="Definitions.BaseMixedRealityExtensionServiceProfile"/>
    /// </summary>
    public interface IMixedRealityExtensionDataProvider : IMixedRealityDataProvider
    {
        // Empty for now, but it is used to filter out the valid class types in the inspector dropdown.
    }
}