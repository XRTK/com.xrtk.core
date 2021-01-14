// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.Events;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// System interface for a locomorion system in the Mixed Reality Toolkit.
    /// All replacement systems for providing locomorion functionality should derive from this interface.
    /// </summary>
    public interface IMixedRealityLocomotionSystem : IMixedRealityEventSystem
    {
    }
}