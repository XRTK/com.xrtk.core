using System;
using UnityEngine;

namespace XRTK.Definitions
{
    /// <summary>
    /// The container for a upm package.
    /// </summary>
    [Serializable]
    public struct MixedRealityPackageInfo
    {
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
        [Tooltip("The package url\n\"https://githubcom/username/project.git#0.1.0\"")]
        private string uri;

        /// <summary>
        /// The package url.
        /// </summary>
        /// <example>
        /// https://githubcom/username/project.git#0.1.0
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