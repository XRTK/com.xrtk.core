// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Base platform class to derive all <see cref="IMixedRealityPlatform"/>s from.
    /// </summary>
    public abstract class BasePlatform : IMixedRealityPlatform
    {
        private string name = null;

        /// <inheritdoc />
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = GetType().Name.Replace("Platform", string.Empty);
                }

                return name;
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable => false;

        /// <inheritdoc />
        public virtual IMixedRealityPlatform[] PlatformOverrides { get; } = new IMixedRealityPlatform[0];

#if UNITY_EDITOR

        /// <inheritdoc />
        public virtual bool IsBuildTargetAvailable => ValidBuildTargets != null &&
                                                      ValidBuildTargets.Any(buildTarget => UnityEditor.EditorUserBuildSettings.activeBuildTarget == buildTarget);

        /// <inheritdoc />
        public virtual UnityEditor.BuildTarget[] ValidBuildTargets => null;

#endif // UNITY_EDITOR

        /// <inheritdoc />
        public override bool Equals(object other) => other?.GetType() == GetType();

        /// <inheritdoc />
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
