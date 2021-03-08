// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRTK.Editor.BuildPipeline
{
    /// <summary>
    /// Build profile for saving 3d app icon's path in the build settings.
    /// </summary>
    [Serializable]
    public class MixedRealityAppIcon
    {
        [SerializeField]
        [FormerlySerializedAs("MixedRealityAppIconPath")]
        private string appIconPath = "";

        public string MixedRealityAppIconPath
        {
            get => appIconPath;
            set => appIconPath = value;
        }
    }
}