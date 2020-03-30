// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The IOS platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class IOSPlatform : BaseDataProvider, IMixedRealityPlatform
    {
        public IOSPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public bool IsAvailable
        {
            get
            {
#if PLATFORM_IOS
                return true;
#else
                return false;
#endif
            }
        }
    }
}