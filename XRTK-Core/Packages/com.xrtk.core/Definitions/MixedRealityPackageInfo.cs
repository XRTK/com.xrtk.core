// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions
{
    /// <summary>
    /// The container for defining a xrtk upm package.
    /// </summary>
    [Serializable]
    public struct MixedRealityPackageInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the package: com.company.package</param>
        /// <param name="displayName">Friendly display name of the package.</param>
        /// <param name="uri">Remote repository uri.</param>
        /// <param name="isRequiredPackage">Is this package required?</param>
        internal MixedRealityPackageInfo(string name, string displayName, string uri, bool isRequiredPackage = false)
        {
            this.name = name;
            this.displayName = displayName;
            this.uri = uri;
            this.isRequiredPackage = isRequiredPackage;
            isDefaultPackage = true;
            isEnabled = false;
        }

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
        [Tooltip("Is this package currently enabled in the project?")]
        private bool isEnabled;

        /// <summary>
        /// Is this package currently enabled in the project?
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            internal set => isEnabled = value;
        }
    }
}