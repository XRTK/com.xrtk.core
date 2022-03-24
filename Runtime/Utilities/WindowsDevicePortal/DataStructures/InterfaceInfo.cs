﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Utilities.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class InterfaceInfo
    {
        public string Description;
        public string GUID;
        public int Index;
        public NetworkProfileInfo[] ProfilesList;
    }
}