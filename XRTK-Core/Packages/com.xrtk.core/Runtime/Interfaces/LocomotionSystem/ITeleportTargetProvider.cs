// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services.LocomotionSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface definition for components and services providing teleportation targets for
    /// <see cref="ITeleportLocomotionProvider"/>s.
    /// </summary>
    public interface ITeleportTargetProvider
    {
        /// <summary>
        /// Gets the <see cref="ITeleportLocomotionProvider"/> that is currently requesting a teleport target
        /// from this target provider, if any.
        /// </summary>
        ITeleportLocomotionProvider RequestingLocomotionProvider { get; }

        /// <summary>
        /// The result from the last raycast.
        /// </summary>
        TeleportValidationResult ValidationResult { get; }
    }
}

