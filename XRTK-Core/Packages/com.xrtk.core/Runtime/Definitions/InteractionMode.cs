// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;

namespace XRTK.Definitions
{
    [Flags]
    public enum InteractionMode
    {
        Both = ~0,
        Far = 1,
        Near = 2,
    }
}