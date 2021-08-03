// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.InputSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface definition for components and services providing teleportation targets for
    /// <see cref="ITeleportLocomotionProvider"/>s. Whenever an <see cref="ITeleportLocomotionProvider"/>
    /// requests a teleportation target, the target provider will answer the request, if both share
    /// the same <see cref="IMixedRealityInputSource"/> connectinon.
    /// </summary>
    public interface ITeleportTargetProvider : IMixedRealityLocomotionSystemHandler
    {
        /// <summary>
        /// Gets the <see cref="ITeleportLocomotionProvider"/> that is currently requesting a teleport target
        /// from this provider, if any.
        /// </summary>
        /// <remarks>Can be <see cref="null"/> if this provider has not received a target request at the time of evaluation.</remarks>
        ITeleportLocomotionProvider RequestingLocomotionProvider { get; }

        /// <summary>
        /// Gets the <see cref="IMixedRealityInputSource"/> this provider is
        /// connected with. Each <see cref="ITeleportTargetProvider"/> must be
        /// assigned an input source so targets can be mapped to the
        /// <see cref="RequestingLocomotionProvider"/>'s input source.
        /// </summary>
        IMixedRealityInputSource InputSource { get; }

        /// <summary>
        /// Gets the validation result for the current target location. The provider should only
        /// provide valid teleportation results to <see cref="ITeleportLocomotionProvider"/>s.
        /// </summary>
        TeleportValidationResult ValidationResult { get; }
    }
}
