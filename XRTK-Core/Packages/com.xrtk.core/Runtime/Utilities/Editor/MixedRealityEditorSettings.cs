// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace XRTK.Utilities.Editor
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Mixed Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged
    {
        private static readonly string IgnoreKey = $"{Application.productName}_XRTK_Editor_IgnoreSettingsPrompts";
        private static readonly string SessionKey = $"{Application.productName}_XRTK_Editor_ShownSettingsPrompts";

        /// <summary>
        /// Constructor.
        /// </summary>
        static MixedRealityEditorSettings()
        {
            if (!Application.isBatchMode)
            {
                EditorApplication.delayCall += CheckSettings;
            }
        }

        /// <summary>
        /// Check the Mixed Reality Toolkit's settings.
        /// </summary>
        public static void CheckSettings()
        {
            EditorPrefs.SetBool($"{Application.productName}_XRTK", true);

            if (Application.isPlaying ||
                EditorPrefs.GetBool(IgnoreKey, false) ||
                !SessionState.GetBool(SessionKey, true))
            {
                return;
            }

            SessionState.SetBool(SessionKey, false);

            var message = "The Mixed Reality Toolkit needs to apply the following settings to your project:\n\n";

            var forceTextSerialization = EditorSettings.serializationMode == SerializationMode.ForceText;

            if (!forceTextSerialization)
            {
                message += "- Force Text Serialization\n";
            }

            var visibleMetaFiles = EditorSettings.externalVersionControl.Equals("Visible Meta Files");

            if (!visibleMetaFiles)
            {
                message += "- Visible meta files\n";
            }

            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
            {
                message += "- Enable Shared Depth Buffer in the XR SDK Settings\n";
            }

            message += "\nWould you like to make these changes?\n\n";

            if (!forceTextSerialization || !visibleMetaFiles)
            {
                var choice = EditorUtility.DisplayDialogComplex("Apply Mixed Reality Toolkit Default Settings?", message, "Apply", "Ignore", "Later");

                switch (choice)
                {
                    case 0:
                        EditorSettings.serializationMode = SerializationMode.ForceText;
                        EditorSettings.externalVersionControl = "Visible Meta Files";
                        AssetDatabase.SaveAssets();

                        if (!EditorApplication.isUpdating)
                        {
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                        }
                        break;
                    case 1:
                        EditorPrefs.SetBool(IgnoreKey, true);
                        break;
                    case 2:
                        break;
                }
            }
        }

        #region IActiveBuildTargetChanged Implementation

        /// <inheritdoc />
        public int callbackOrder => 0;

        /// <inheritdoc />
        void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            SessionState.SetBool(SessionKey, true);
            CheckSettings();
        }

        #endregion IActiveBuildTargetChanged Implementation
    }
}