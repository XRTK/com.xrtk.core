// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Platform;
using XRTK.Interfaces.Platform;

namespace XRTK.Services.PlatformSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's default implementation of the <see cref="IMixedRealityPlatformSystem"/>
    /// </summary>
    public class MixedRealityPlatformSystem : BaseEventSystem, IMixedRealityPlatformSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityPlatformSystem(MixedRealityPlatformSystemProfile profile) : base(profile)
        {
        }
    }
}