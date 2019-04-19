using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Inspectors.Utilities.Packages
{
    /// <summary>
    /// This scriptable object notifies the editor project that the package is installed and ready to use.
    /// </summary>
    [CreateAssetMenu(fileName = "PackageValidation", menuName = "Mixed Reality Toolkit/Settings/Package Validation", order = (int)CreateProfileMenuItemIndices.Settings)]
    public class MixedRealityPackageValidation : ScriptableObject
    {
        [SerializeField]
        private string packageName = string.Empty;

        public string PackageName => packageName;

        /// <summary>
        /// Is the package located in the project's Assets folder?
        /// </summary>
        public bool IsMainProjectAsset
        {
            get
            {
                var localPath = AssetDatabase.GetAssetPath(this);
                return localPath.StartsWith("Assets/");
            }
        }
    }
}
