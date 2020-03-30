// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.PlatformSystem;
using XRTK.Interfaces.PlatformSystem;

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

        private readonly List<IMixedRealityPlatform> activePlatforms = new List<IMixedRealityPlatform>();

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityPlatform> ActivePlatforms => activePlatforms;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            var registeredPlatforms = MixedRealityToolkit.GetActiveServices<IMixedRealityPlatform>();

            activePlatforms.Clear();

            foreach (var service in registeredPlatforms)
            {
                var platform = service as IMixedRealityPlatform;

                if (platform.IsAvailable)
                {
                    Debug.Log(platform.Name);
                    activePlatforms.Add(platform);
                }
            }
        }
    }
}