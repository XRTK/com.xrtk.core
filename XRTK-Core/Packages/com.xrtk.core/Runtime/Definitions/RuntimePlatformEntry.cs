// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    [Serializable]
    public class RuntimePlatformEntry
    {
        public RuntimePlatformEntry()
        {
            runtimePlatforms = new SystemType[0];
        }

        public RuntimePlatformEntry(IReadOnlyList<IMixedRealityPlatform> runtimePlatforms)
        {
            this.runtimePlatforms = new SystemType[runtimePlatforms.Count];

            for (int i = 0; i < runtimePlatforms.Count; i++)
            {
                this.runtimePlatforms[i] = new SystemType(runtimePlatforms[i].GetType());
            }
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityPlatform), TypeGrouping.ByNamespaceFlat)]
        private SystemType[] runtimePlatforms;

        public SystemType[] RuntimePlatforms => runtimePlatforms;
    }
}