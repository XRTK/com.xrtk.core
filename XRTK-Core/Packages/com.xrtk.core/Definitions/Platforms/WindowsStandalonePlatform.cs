// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the Windows Standalone platform.
    /// </summary>
    public class WindowsStandalonePlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return true;
#else
                return false;
#endif
            }
        }
    }
}