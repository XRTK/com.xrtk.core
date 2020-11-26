// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions
{
    /// <summary>
    /// Base Profile for all data containers to use in the Mixed Reality Toolkit.
    /// </summary>
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        /// <summary>
        /// The profile's parent in the service graph hierarchy.
        /// </summary>
        public BaseMixedRealityProfile ParentProfile { get; internal set; } = null;
    }
}