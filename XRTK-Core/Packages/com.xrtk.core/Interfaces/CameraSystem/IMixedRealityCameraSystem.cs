// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace XRTK.Interfaces.CameraSystem
{
    /// <summary>
    /// The base interface for implementing a mixed reality camera system.
    /// </summary>
    public interface IMixedRealityCameraSystem : IMixedRealityService
    {
        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Is the current camera displaying on a traditional 2d screen or a stereoscopic display?
        /// </summary>
        bool IsStereoscopic { get; }

        /// <summary>
        /// The <see cref="IMixedRealityCameraRig"/> component used in the current configuration.
        /// </summary>
        IMixedRealityCameraRig CameraRig { get; }

        /// <summary>
        /// The default head height when a platform doesn't automatically set it.
        /// </summary>
        float DefaultHeadHeight { get; }

        /// <summary>
        /// The current head height of the player
        /// </summary>
        float HeadHeight { get; set; }
    }
}