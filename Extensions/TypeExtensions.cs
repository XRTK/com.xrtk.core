// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/> instances.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Recursively looks for generic type arguments in type hierarchy, starting with the
        /// root type provided. If no generic type arguments are found on a type, it's base
        /// type is checked.
        /// </summary>
        /// <param name="root">Root type to start looking for generic type arguments at.</param>
        /// <param name="maxRecursionDepth">The maximum recursion depth until execution gets canceled even if no results found.</param>
        /// <returns>Found generic type arguments array or null, if none found.</returns>
        public static Type[] FindTopmostGenericTypeArguments(this Type root, int maxRecursionDepth = 5)
        {
            var genericTypeArgs = root?.GenericTypeArguments;

            if (genericTypeArgs != null && genericTypeArgs.Length > 0)
            {
                return genericTypeArgs;
            }

            if (maxRecursionDepth > 0 && root != null)
            {
                return FindTopmostGenericTypeArguments(root.BaseType, --maxRecursionDepth);
            }

            Debug.LogError($"{nameof(FindTopmostGenericTypeArguments)} - Maximum recursion depth reached without finding generic type arguments.");
            return null;
        }
    }
}
