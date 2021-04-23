// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// Influences how the <see cref="MixedRealityInteractionMapping"/> determines state changes that will raise the <see cref="MixedRealityInputAction"/>.
    /// </summary>
    public enum StateChangeType
    {
        /// <summary>
        /// Use this for any <see cref="MixedRealityInteractionMapping"/> which should trigger continuous <see cref="MixedRealityInputAction"/>s from the Control.
        /// </summary>
        Continuous = 0,
        /// <summary>
        ///  Use this for <see cref="MixedRealityInteractionMapping"/> that only trigger an <see cref="MixedRealityInputAction"/> once until the Control is reset to its default state.
        /// </summary>
        Trigger,
    }
}