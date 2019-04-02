using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Definitions
{
    /// <summary>
    /// Data container for holding the settings for the upm packages associated with the XRTK.
    /// </summary>
    public class MixedRealityPackageSettings : ScriptableObject
    {
        [SerializeField]
        private string[] packageNames = new string[0];

        [SerializeField]
        private string[] packageUrls = new string[0];

        public Dictionary<string, string> PackageList
        {
            get
            {
                packageList.Clear();

                for (int i = 0; i < packageNames.Length; i++)
                {
                    packageList.Add(packageNames[i], packageUrls[i]);
                }

                return packageList;
            }
        }

        private Dictionary<string, string> packageList = new Dictionary<string, string>(0);
    }
}
