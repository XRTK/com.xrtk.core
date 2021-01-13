// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// Dummy scriptable object used to find the relative path to com.xrtk.core.
    /// </summary>
    /// <inheritdoc cref="IPathFinder" />
    public class CorePathFinder : ScriptableObject, IPathFinder
    {
        /// <inheritdoc />
        public string Location => $"/Editor/Utilities/{nameof(CorePathFinder)}.cs";
    }
}