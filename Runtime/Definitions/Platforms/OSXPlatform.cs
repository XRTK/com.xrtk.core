// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the OSX platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("97344133-03B4-4314-9581-E42D1AAC91BA")]
    public class OSXPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE_OSX
                return !UnityEngine.Application.isEditor;
#else
                return false;
#endif
            }
        }

#if UNITY_EDITOR

        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } = { UnityEditor.BuildTarget.StandaloneOSX };

#endif // UNITY_EDITOR
    }
}
