// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// The Editor platform definition for the Mixed Reality Toolkit.
    /// </summary>
    /// <remarks>
    /// Defines any editor platform for Win, OSX, and Linux.
    /// </remarks>
    public class EditorPlatform : BaseDataProvider, IMixedRealityPlatform
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public EditorPlatform(string name, uint priority)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public override string Name => "Editor";

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public bool IsActive => Application.isEditor;
    }
}