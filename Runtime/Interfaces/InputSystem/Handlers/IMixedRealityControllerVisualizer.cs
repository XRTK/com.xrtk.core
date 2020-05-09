// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface for configuring controller visualization.
    /// </summary>
    public interface IMixedRealityControllerVisualizer : IMixedRealityControllerPoseSynchronizer
    {
        /// <summary>
        /// The <see cref="UnityEngine.GameObject"/> reference for this controller.
        /// </summary>
        /// <remarks>
        /// This reference may not always be available when called.
        /// </remarks>
        GameObject GameObject { get; }

        // TODO add defined elements or transforms?
    }
}