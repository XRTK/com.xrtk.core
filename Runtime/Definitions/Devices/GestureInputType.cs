﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Devices
{
    /// <summary>
    /// The GestureInputType defines the types of gestures exposed by a controller.
    /// </summary>
    public enum GestureInputType
    {
        None = 0,
        Tap,
        DoubleTap,
        Hold,
        Navigation,
        Manipulation
    }
}