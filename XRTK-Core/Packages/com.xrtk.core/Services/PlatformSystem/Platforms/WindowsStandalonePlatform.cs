// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The Windows Standalone platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class WindowsStandalonePlatform : BaseDataProvider, IMixedRealityPlatform
    {
        /// <inheritdoc />
        public WindowsStandalonePlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return true;
#else
                return false;
#endif
            }
        }
    }
}