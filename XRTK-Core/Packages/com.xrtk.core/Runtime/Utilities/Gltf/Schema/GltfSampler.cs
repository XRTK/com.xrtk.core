// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Utilities.Gltf.Schema
{
    /// <summary>
    /// Texture sampler properties for filtering and wrapping modes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/sampler.schema.json
    /// </summary>
    [Serializable]
    public class GltfSampler : GltfChildOfRootProperty, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Magnification filter.
        /// Valid values correspond to WebGL enums: `9728` (NEAREST) and `9729` (LINEAR).
        /// </summary>
        public GltfMagnificationFilterMode MagFilter { get; set; } = GltfMagnificationFilterMode.Linear;

        [SerializeField]
        private string magFilter = GltfMagnificationFilterMode.Linear.ToString();

        /// <summary>
        /// Minification filter. All valid values correspond to WebGL enums.
        /// </summary>
        public GltfMinFilterMode MinFilter { get; set; } = GltfMinFilterMode.NearestMipmapLinear;

        [SerializeField]
        private string minFilter = GltfMinFilterMode.NearestMipmapLinear.ToString();

        /// <summary>
        /// s wrapping mode.  All valid values correspond to WebGL enums.
        /// </summary>
        public GltfWrapMode WrapS { get; set; } = GltfWrapMode.Repeat;

        [SerializeField]
        private string wrapS = null;

        /// <summary>
        /// t wrapping mode.  All valid values correspond to WebGL enums.
        /// </summary>
        public GltfWrapMode WrapT { get; set; } = GltfWrapMode.Repeat;

        [SerializeField]
        private string wrapT = GltfWrapMode.Repeat.ToString();

#region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (Enum.TryParse(magFilter, out GltfMagnificationFilterMode result))
            {
                MagFilter = result;
            }
            else
            {
                MagFilter = GltfMagnificationFilterMode.Linear;
            }
            if (Enum.TryParse(minFilter, out GltfMinFilterMode result2))
            {
                MinFilter = result2;
            }
            else
            {
                MinFilter = GltfMinFilterMode.NearestMipmapLinear;
            }
            if (Enum.TryParse(wrapT, out GltfWrapMode result3))
            {
                WrapT = result3;
            }
            else
            {
                WrapT = GltfWrapMode.Repeat;
            }
            if (Enum.TryParse(wrapS, out GltfWrapMode result4))
            {
                WrapS = result4;
            }
            else
            {
                WrapS = GltfWrapMode.Repeat;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            magFilter = MagFilter.ToString();
            minFilter = MinFilter.ToString();
            wrapT = WrapT.ToString();
            wrapS = WrapS.ToString();
        }

#endregion ISerializationCallbackReceiver
    }
}