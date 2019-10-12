// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Utilities
{
    public enum TrackedObjectType
    {
        /// <summary>
        /// Calculates position and orientation from the main camera.
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
        /// Calculates position and orientation from the playspace.
        /// </summary>
        Body
    }
}