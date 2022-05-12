// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to define free locomotion providers for the <see cref="ILocomotionSystem"/>.
    /// Free locomotion is defined as movement in a specific direction in 3D space, which the user can freely choose.
    /// This type of locomotion can coexist with <see cref="ITeleportLocomotionProvider"/>s. However there can always be only one
    /// active provider for free locomotion at a time.
    /// </summary>
    public interface IFreeLocomotionProvider : ILocomotionProvider { }
}
