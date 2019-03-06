// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;
using XRTK.Utilities.Async;
using XRTK.Utilities.Gltf.Schema;
using XRTK.Utilities.Gltf.Serialization;

namespace XRTK.Examples.Demos.Gltf
{
    public class TestGlbLoading : MonoBehaviour
    {
        [SerializeField]
        private string uri = string.Empty;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uri))
            {
                uri = "\\XRTK.Examples\\Demos\\Gltf\\Models\\Lantern\\glTF-Binary\\Lantern.glb";
                var path = $"{Application.dataPath}{uri}";
                path = path.Replace("/", "\\");
                Debug.Assert(File.Exists(path));
            }
        }

        private async void Start()
        {
            await new WaitForSeconds(5f);
            GltfObject gltfObject = null;

            try
            {
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync($"{Application.dataPath}{uri}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}