// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces
{
    /// <summary>
    /// Defines the platform to be registered
    /// </summary>
    public interface IMixedRealityPlatform
    {
        /// <summary>
        /// Is this platform currently available?
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// The this platform build target available?
        /// </summary>
        /// <remarks>
        /// Only returns true in editor.
        /// </remarks>
        bool IsBuildTargetAvailable { get; }

        /// <summary>
        /// The list of platforms that this specific platform will override and make the others return not available.
        /// </summary>
        IMixedRealityPlatform[] PlatformOverrides { get; }
    }
}