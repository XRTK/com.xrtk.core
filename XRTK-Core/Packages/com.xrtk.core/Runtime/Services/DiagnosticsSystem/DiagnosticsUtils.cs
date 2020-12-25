// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Diagnostics utilities.
    /// </summary>
    public static class DiagnosticsUtils
    {
        /// <summary>
        /// Converts megabytes to bytes.
        /// </summary>
        /// <param name="megabytes">Amount of megabytes.</param>
        /// <returns>Amount of bytes.</returns>
        public static ulong ConvertMegabytesToBytes(int megabytes)
        {
            return (ulong)megabytes * 1024UL * 1024UL;
        }

        /// <summary>
        /// Converts bytes to megabytes.
        /// </summary>
        /// <param name="bytes">Amount of bytes.</param>
        /// <returns>Amount of megabytes.</returns>
        public static float ConvertBytesToMegabytes(ulong bytes)
        {
            return bytes / 1024.0f / 1024.0f;
        }
    }
}