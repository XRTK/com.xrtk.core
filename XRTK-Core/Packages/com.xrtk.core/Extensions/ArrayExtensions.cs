// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

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
            // Initialise the array if it is null
            if (array == null)
            {
                return new[] { newItem };
            }

            //Extend the array, copy the items and add the new one
            var newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = newItem;
            return newArray;
        }

        /// <summary>
        /// Initialises an array with a default item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array to initialise</param>
        /// <param name="newItem">The default item to set all the array items to</param>
        /// <returns></returns>
        /// <remarks>Please note, this extension should only be used with value types such as structs.  Reference types should not be used.</remarks>
        public static T[] InitialiseArray<T>(this T[] array, T newItem)
        {
            // Initialise the array if it is null
            if (array == null)
            {
                return new[] { newItem };
            }

            //Set all the items to the provided value
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = newItem;
            }

            return array;
        }
    }
}
