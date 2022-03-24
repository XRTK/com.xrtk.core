// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    ///  Used by the XRTK to signal that the feature is available on the iOS platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("BCAC2CEF-E793-47B8-9DB7-116AF668CB66")]
    public class IOSPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if PLATFORM_IOS
                return !UnityEngine.Application.isEditor;
#else
                return false;
#endif
            }
        }

#if UNITY_EDITOR

        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } = { UnityEditor.BuildTarget.iOS };

#endif // UNITY_EDITOR
    }
}
