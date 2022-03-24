// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming gesture based input actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Gestures Profile", fileName = "MixedRealityGesturesProfile", order = (int)CreateProfileMenuItemIndices.Gestures)]
    public class MixedRealityGesturesProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityGestureMapping[] gestures =
        {
            new MixedRealityGestureMapping("Hold", GestureInputType.Hold, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Navigation", GestureInputType.Navigation, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Manipulation", GestureInputType.Manipulation, MixedRealityInputAction.None),
        };

        /// <summary>
        /// The currently configured gestures for the application.
        /// </summary>
        public MixedRealityGestureMapping[] Gestures => gestures;
    }
}