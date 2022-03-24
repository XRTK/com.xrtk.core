﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Utilities.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class ProcessInfo
    {
        public float CPUUsage;
        public string ImageName;
        public float PageFileUsage;
        public int PrivateWorkingSet;
        public int ProcessId;
        public int SessionId;
        public string UserName;
        public int VirtualSize;
        public int WorkingSetSize;
    }
}