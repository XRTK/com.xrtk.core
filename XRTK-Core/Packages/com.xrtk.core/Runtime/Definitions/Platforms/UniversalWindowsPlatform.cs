// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the Windows Universal Platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("7DC72B4E-34F6-4B26-AFD7-CDE0C51F83A3")]
    public class UniversalWindowsPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if WINDOWS_UWP
                return !UnityEngine.Application.isEditor;
#else
                return false;
#endif
            }
        }

        public override bool IsBuildTargetAvailable
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WSAPlayer;
#else
                return false;
#endif
            }
        }
    }
}
