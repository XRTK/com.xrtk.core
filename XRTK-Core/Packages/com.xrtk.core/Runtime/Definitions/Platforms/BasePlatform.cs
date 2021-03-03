// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Interfaces;

#if UNITY_EDITOR
using System.Linq;
#endif

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
        public virtual IMixedRealityPlatform[] PlatformOverrides { get; } = new IMixedRealityPlatform[0];

#if UNITY_EDITOR

        /// <inheritdoc />
        public virtual bool IsBuildTargetAvailable => ValidBuildTargets != null &&
                                                      ValidBuildTargets.Any(buildTarget => UnityEditor.EditorUserBuildSettings.activeBuildTarget == buildTarget);

        /// <inheritdoc />
        public virtual UnityEditor.BuildTarget[] ValidBuildTargets { get; } = null;

#endif // UNITY_EDITOR
    }
}