// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to define on rails locomotion providers for the <see cref="IMixedRealityLocomotionSystem"/>.
    /// With on rails locomotion the user has very limited to none control over their movement. Instead the player rig
    /// moves on predefined pathways (rails). Either automatically or by input control. On rails locomotion is exclusive,
    /// that means when any on rails locomotion provider is active and enabled, all other types of locomotion are not available.
    /// There can always be only one active provider for on rails locomotion at a time.
    /// </summary>
    public interface IMixedRealityOnRailsLocomotionProvider : IMixedRealityLocomotionProvider { }
}
