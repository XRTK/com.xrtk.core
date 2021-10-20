// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.CameraSystem
{
    /// <summary>
    /// Base interface for implementing camera data providers to be registered with the <see cref="IMixedRealityCameraSystem"/>
    /// </summary>
    public interface IMixedRealityCameraDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Is the current camera displaying on a traditional 2d screen or a stereoscopic display?
        /// </summary>
        bool IsStereoscopic { get; }

#if XRTK_USE_LEGACYVR
        /// <summary>
        /// Is the head height, and thus the camera y-position, managed by the device itself?
        /// If true, the <see cref="DefaultHeadHeight"/> setting is ignored and has no effect
        /// on camera positioning.
        /// </summary>
        bool HeadHeightIsManagedByDevice { get; }
#endif

        /// <summary>
        /// The <see cref="IMixedRealityCameraRig"/> reference for this data provider.
        /// </summary>
        IMixedRealityCameraRig CameraRig { get; }

#if XRTK_USE_LEGACYVR
        /// <summary>
        /// The default head height when a platform doesn't automatically set it.
        /// </summary>
        float DefaultHeadHeight { get; }
#endif

#if XRTK_USE_LEGACYVR
        /// <summary>
        /// The current head height of the player
        /// </summary>
        float HeadHeight { get; set; }
#else
        /// <summary>
        /// The current head height of the player
        /// </summary>
        float HeadHeight { get; }
#endif
    }
}
