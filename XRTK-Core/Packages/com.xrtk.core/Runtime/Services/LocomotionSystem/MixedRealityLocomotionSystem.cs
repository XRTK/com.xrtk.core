// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityLocomotionSystem"/>
    /// </summary>
    [System.Runtime.InteropServices.Guid("9453c088-285e-47aa-bfbb-dafd9109fdd5")]
    public class MixedRealityLocomotionSystem : BaseEventSystem, IMixedRealityLocomotionSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The active <see cref="MixedRealityLocomotionSystemProfile"/>.</param>
        public MixedRealityLocomotionSystem(MixedRealityLocomotionSystemProfile profile)
            : base(profile)
        {

        }
    }
}