// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions
{
    /// <summary>
    /// Data container for holding the settings for the upm packages associated with the XRTK.
    /// </summary>
    public class MixedRealityPackageSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Known xrtk packages in upm.")]
        private MixedRealityPackageInfo[] mixedRealityPackages =
        {
            new MixedRealityPackageInfo("com.xrtk.upm-git-extension", "XRTK.UpmGitExtension", "https://github.com/XRTK/UpmGitExtension.git", isRequiredPackage: true),
            new MixedRealityPackageInfo("com.xrtk.wmr", "XRTK.WindowsMixedReality", "https://github.com/XRTK/WindowsMixedReality.git"),
            new MixedRealityPackageInfo("com.xrtk.lumin", "XRTK.Lumin", "https://github.com/XRTK/Lumin.git"),
            new MixedRealityPackageInfo("com.xrtk.sdk", "XRTK.SDK", "https://github.com/XRTK/SDK.git"),
        };

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
