// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the Windows Universal Platform.
    /// </summary>
    public class UniversalWindowsPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if UNITY_WSA
                return true;
#else
                return false;
#endif
            }
        }
    }
}