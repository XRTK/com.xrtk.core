// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Interfaces;

namespace XRTK.Extensions
{
    /// <summary>
    /// <see cref="Array"/> type method extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Wraps the index around to the beginning of the array if the provided index is longer than the array.
        /// </summary>
        /// <param name="array">The array to wrap the index around.</param>
        /// <param name="index">The index to look for.</param>
        public static int WrapIndex(this Array array, int index)
        {
            int length = array.Length;
            return ((index % length) + length) % length;
        }

        /// <summary>
        /// Checks whether the given array is not null and has at least one entry
        /// </summary>
        /// <param name="array"></param>
        public static bool IsValidArray(this Array array)
        {
            return array != null && array.Length > 0;
        }

        /// <summary>
        /// Extends an existing array to add a new item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array to extend</param>
        /// <param name="newItem">The item to add to the array</param>
        /// <returns></returns>
        public static T[] AddItem<T>(this T[] array, T newItem)
        {
            if (array == null)
            {
                return new[] { newItem };
            }

            var newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = newItem;
            return newArray;
        }
    }
}