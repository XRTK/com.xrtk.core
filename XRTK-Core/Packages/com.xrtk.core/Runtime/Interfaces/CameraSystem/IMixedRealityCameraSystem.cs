// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace XRTK.Interfaces.CameraSystem
{
    /// <summary>
    /// The base interface for implementing a mixed reality camera system.
    /// </summary>
    public interface IMixedRealityCameraSystem : IMixedRealitySystem
    {
        /// <summary>
        /// The list of <see cref="IMixedRealityCameraDataProvider"/>s registered and running with the system.
        /// </summary>
        IReadOnlyCollection<IMixedRealityCameraDataProvider> CameraDataProviders { get; }

        /// <summary>
        /// The reference to the <see cref="IMixedRealityCameraRig"/> attached to the Main Camera (typically this is the player's camera).
        /// </summary>
        IMixedRealityCameraRig MainCameraRig { get; }

        /// <summary>
        /// Sets the <see cref="IMixedRealityCameraDataProvider.HeadHeight"/>. If <see cref="setForAllCameraProviders"/> is true, then it's set
        /// for all <see cref="CameraDataProviders"/>, otherwise it only sets the <see cref="MainCameraRig"/>.
        /// </summary>
        /// <param name="value">The height value to set.</param>
        /// <param name="setForAllCameraProviders">If <see cref="setForAllCameraProviders"/> is true, then it's set
        /// for all <see cref="CameraDataProviders"/>, otherwise it only sets the <see cref="MainCameraRig"/>.</param>
        void SetHeadHeight(float value, bool setForAllCameraProviders = false);

        /// <summary>
        /// Registers the <see cref="IMixedRealityCameraDataProvider"/> with the <see cref="IMixedRealityCameraSystem"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        void RegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider);

        /// <summary>
        /// UnRegisters the <see cref="IMixedRealityCameraDataProvider"/> with the <see cref="IMixedRealityCameraSystem"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        void UnRegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider);
    }
}