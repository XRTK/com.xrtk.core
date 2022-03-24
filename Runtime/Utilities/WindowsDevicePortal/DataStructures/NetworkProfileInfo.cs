﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Utilities.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class NetworkProfileInfo
    {
        public bool GroupPolicyProfile;
        public string Name;
        public bool PerUserProfile;
    }
}