// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services.LocomotionSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// The base interface to define locomotion providers for the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public interface IMixedRealityLocomotionProvider : IMixedRealityLocomotionDataProvider
    {
        /// <summary>
        /// Gets whether this locomotion provider is enabled and handling locomotion requests.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the <see cref="LocomotionType"/> implemented by this locomotion implementation.
        /// </summary>
        LocomotionType Type { get; }
    }
}
