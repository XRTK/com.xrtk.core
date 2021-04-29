// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XRTK.Seed
{
    internal class PackageManifest
    {
        internal static string ManifestFilePath => $"{Directory.GetParent(Application.dataPath)}\\Packages\\manifest.json";

        [JsonProperty("scopedRegistries")]
        public List<ScopedRegistry> ScopedRegistries { get; set; }

        [JsonProperty("dependencies")]
        public Dictionary<string, string> Dependencies { get; set; }
    }
}