// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Interfaces;

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Base platform class to derive all <see cref="IMixedRealityPlatform"/>s from.
    /// </summary>
    [Serializable]
    public abstract class BasePlatform : IMixedRealityPlatform
    {
        /// <inheritdoc />
        public virtual bool IsAvailable => false;

        /// <inheritdoc />
        public virtual bool IsBuildTargetAvailable => false;

        /// <inheritdoc />
        public virtual IMixedRealityPlatform[] PlatformOverrides { get; } = new IMixedRealityPlatform[0];
    }
}