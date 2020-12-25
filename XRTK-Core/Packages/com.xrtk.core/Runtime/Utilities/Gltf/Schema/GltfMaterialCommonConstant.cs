// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Utilities.Gltf.Schema
{
    [Serializable]
    public class GltfMaterialCommonConstant : GltfProperty
    {
        // Note: GltfMaterialCommonConstants aren't currently used but exists for deserializing the json.
        // These values would influence properties in Unity -> Window -> Rendering -> Light Settings if used.

        /// <summary>
        /// Used to scale the ambient light contributions to this material
        /// </summary>
        public float[] ambientFactor;

        /// <summary>
        /// Texture used to store pre-computed direct lighting
        /// </summary>
        public GltfNormalTextureInfo lightmapTexture;

        /// <summary>
        /// Scale factor for the lightmap texture
        /// </summary>
        public float[] lightmapFactor;
    }
}