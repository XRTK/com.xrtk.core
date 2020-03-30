// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The OSX platform definition for the Mixed Reality Toolkit.
    /// </summary>
    public class OSXPlatform : BasePlatform
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public OSXPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE_OSX
                return true;
#else
                return false;
#endif
            }
        }
    }
}