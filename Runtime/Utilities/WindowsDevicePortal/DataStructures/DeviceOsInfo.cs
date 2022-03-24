﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Utilities.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class DeviceOsInfo
    {
        public string ComputerName;
        public string OsEdition;
        public int OsEditionId;
        public string OsVersion;
        public string Platform;
    }
}