// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor.PackageManager;
using UnityEngine;

namespace XRTK.Inspectors.Utilities.Packages
{
    /// <summary>
    /// The container for a upm package.
    /// </summary>
    [Serializable]
    public struct MixedRealityPackageInfo
    {
        /// <summary>
        /// The Unity <see cref="PackageInfo"/>
        /// </summary>
        public PackageInfo PackageInfo { get; internal set; }

        [SerializeField]
        [Tooltip("The package name\n\"com.company.product\"")]
        private string name;

        /// <summary>
        /// The package name
        /// </summary>
        /// <example>
        /// com.company.product
        /// </example>
        public string Name
        {
            get => name;
            internal set => name = value;
        }

        [SerializeField]
        [Tooltip("The package display name.")]
        private string displayName;

        /// <summary>
        /// "The package display name
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            internal set => displayName = value;
        }

        [SerializeField]
        [Tooltip("The package url\n\"https://githubcom/username/project.git\"")]
        private string uri;

        /// <summary>
        /// The package url.
        /// </summary>
        /// <example>
        /// https://githubcom/username/project.git
        /// </example>
        public string Uri
        {
            get => uri;
            internal set => uri = value;
        }

        [SerializeField]
        [Tooltip("This package is installed by the XRTK by default?")]
        private bool isDefaultPackage;

        /// <summary>
        /// This package is installed by the XRTK by default?
        /// </summary>
        public bool IsDefaultPackage
        {
            get => isDefaultPackage;
            internal set => isDefaultPackage = value;
        }

        [SerializeField]
        [Tooltip("This package is required by the XRTK.")]
        private bool isRequiredPackage;

        /// <summary>
        /// This package is required by the xrtk.
        /// </summary>
        public bool IsRequiredPackage
        {
            get => isRequiredPackage;
            internal set => isRequiredPackage = value;
        }

        [SerializeField]
        [Tooltip("The list of dependencies for this package.")]
        private string[] dependencies;

        /// <summary>
        /// The list of dependencies for this package.
        /// </summary>
        public string[] Dependencies
        {
            get => dependencies;
            internal set => dependencies = value;
        }
    }
}