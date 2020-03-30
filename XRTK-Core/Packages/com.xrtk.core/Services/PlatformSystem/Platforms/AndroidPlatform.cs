// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The Android platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class AndroidPlatform : BasePlatform
    {
        public AndroidPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public override bool IsAvailable
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