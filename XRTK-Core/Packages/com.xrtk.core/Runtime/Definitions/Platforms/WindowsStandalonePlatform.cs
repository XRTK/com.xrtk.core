// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the Windows Standalone platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("58DE2D2A-DB5F-4090-A949-4028B8EA3AFD")]
    public class WindowsStandalonePlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return !UnityEngine.Application.isEditor;
#else
                return false;
#endif
            }
        }

        /// <inheritdoc />
        public override bool IsBuildTargetAvailable
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows ||
                       UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows64;
#else
                return false;
#endif
            }
        }
    }
}