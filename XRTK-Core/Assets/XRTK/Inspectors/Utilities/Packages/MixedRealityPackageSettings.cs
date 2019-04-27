// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Inspectors.Utilities.Packages
{
    /// <summary>
    /// Data container for holding the settings for the upm packages associated with the XRTK.
    /// </summary>
    public class MixedRealityPackageSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Known xrtk packages in upm.")]
        private MixedRealityPackageInfo[] mixedRealityPackages = new MixedRealityPackageInfo[0];

        /// <summary>
        /// Known XRTK packages in upm.
        /// </summary>
        public MixedRealityPackageInfo[] MixedRealityPackages
        {
            get => mixedRealityPackages;
            internal set => mixedRealityPackages = value;
        }
    }
}
