using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Utilities.Async;

namespace XRTK.Inspectors.Utilities
{
    [InitializeOnLoad]
    public static class MixedRealityPackageUtilities
    {
        static MixedRealityPackageUtilities()
        {
            EditorApplication.delayCall += CheckPackageManifest;
        }

        private static async void CheckPackageManifest()
        {
            if (Application.isPlaying) { return; }
            await CheckPackageManifestAsync();
        }

        private static async Task CheckPackageManifestAsync()
        {
            // TODO read this data from a scriptable object so it can be configured in the editor.
            var searchResult = Client.Search("XRTK UPM Git Extension");

            await new WaitUntil(() => searchResult.Status != StatusCode.InProgress);

            if (searchResult.Result == null)
            {
                Client.Add("com.xrtk.upm-git-extension@https://github.com/XRTK/UpmGitExtension.git#1.0.0");
            }
        }
    }
}
