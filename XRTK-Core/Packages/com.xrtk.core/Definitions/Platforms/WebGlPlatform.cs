// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if PLATFORM_WEBGL
using UnityEngine;
#endif

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the WebGL platform.
    /// </summary>
    public class WebGlPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if PLATFORM_WEBGL
                return !Application.isEditor;
#else
                return false;
#endif
            }
        }
    }
}
