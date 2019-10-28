// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;

namespace XRTK.Definitions.Utilities
{
    public enum TrackedObjectType
    {
        /// <summary>
        /// Calculates position and orientation based on the <see cref="IMixedRealityCameraRig.CameraTransform"/>.
        /// </summary>
        Head = 0,
        /// <summary>
        /// Calculates position and orientation from the left hand or tracked controller.
        /// </summary>
        LeftHandOrController,
        /// <summary>
        /// Calculates position and orientation from the right hand or tracked controller.
        /// </summary>
        RightHandOrController,
        /// <summary>
        /// Calculates position and orientation based on the <see cref="IMixedRealityCameraRig.BodyTransform"/>.
        /// </summary>
        Body,
        /// <summary>
        /// Calculates position and orientation based on the <see cref="IMixedRealityCameraRig.PlayspaceTransform"/>
        /// </summary>
        Playspace,
    }
}