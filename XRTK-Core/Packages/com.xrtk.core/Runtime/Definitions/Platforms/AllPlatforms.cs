// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on every platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("C6BF6315-2E9C-4602-827D-2D4871D29422")]
    public sealed class AllPlatforms : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable => true;

        /// <inheritdoc />
        public override bool IsBuildTargetAvailable => true;
    }
}