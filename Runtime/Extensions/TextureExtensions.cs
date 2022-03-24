﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using UnityEngine;
using XRTK.Utilities.Async;

namespace XRTK.Extensions
{
    /// <summary>
    /// <see cref="Texture"/> and <see cref="Texture2D"/> Extensions.
    /// </summary>
    public static class TextureExtensions
    {
        public static async Task<Texture2D> LoadTextureAsync(this Texture2D texture, string path)
        {
            // TODO load raw image data
            return await texture.LoadTextureAsync((byte[])null);
        }

        public static async Task<Texture2D> LoadTextureAsync(this Texture2D texture, byte[] rawImageData)
        {
            texture.GetNativeTexturePtr();
            await Awaiters.UnityMainThread;
            // TODO Call into native graphic plugin.
            return texture;
        }
    }
}