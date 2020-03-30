﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The IOS platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class WebGlPlatform : BasePlatform
    {
        public WebGlPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if PLATFORM_WEBGL
                return true;
#else
                return false;
#endif
            }
        }
    }
}