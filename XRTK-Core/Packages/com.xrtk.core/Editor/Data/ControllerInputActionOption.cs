// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Editor.Data
{
    /// <summary>
    /// Used to aid in layout of Controller Input Actions.
    /// </summary>
    [Serializable]
    public struct ControllerInputActionOption
    {
        public string Controller;
        public Handedness Handedness;
        public Vector2[] InputLabelPositions;
        public bool[] IsLabelFlipped;
    }
}