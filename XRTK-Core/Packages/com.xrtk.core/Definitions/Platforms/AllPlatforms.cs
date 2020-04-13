// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on every platform.
    /// </summary>
    public sealed class AllPlatforms : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsActive => true;

        /// <inheritdoc />
        public override bool IsBuildTargetAvailable => true;
    }
}