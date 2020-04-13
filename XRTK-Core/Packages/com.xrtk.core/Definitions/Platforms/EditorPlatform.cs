// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available in the Unity Editor.
    /// </summary>
    /// <remarks>
    /// Defines any editor platform for Win, OSX, and Linux.
    /// </remarks>
    public sealed class EditorPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable => Application.isEditor;

        /// <inheritdoc />
        public override bool IsBuildTargetAvailable => false;
    }
}