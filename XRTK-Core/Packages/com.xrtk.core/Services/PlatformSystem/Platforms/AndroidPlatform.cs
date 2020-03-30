// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The Android platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class AndroidPlatform : BaseDataProvider, IMixedRealityPlatform
    {
        public AndroidPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        public bool IsAvailable
        {
            get
            {
#if PLATFORM_ANDROID
                return true;
#else
                return false;
#endif
            }
        }
    }
}