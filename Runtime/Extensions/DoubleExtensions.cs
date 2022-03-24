﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for the .Net Double struct
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Checks if two numbers are approximately equal. Similar to <see cref="UnityEngine.Mathf.Approximately(float, float)"/>, but the tolerance
        /// can be specified.
        /// </summary>
        /// <param name="number">One of the numbers to compare.</param>
        /// <param name="other">The other number to compare.</param>
        /// <param name="tolerance">The amount of tolerance to allow while still considering the numbers approximately equal.</param>
        /// <returns>True if the difference between the numbers is less than or equal to the tolerance, false otherwise.</returns>
        public static bool Approximately(this double number, double other, double tolerance)
        {
            return Math.Abs(number - other) <= tolerance;
        }
    }
}
