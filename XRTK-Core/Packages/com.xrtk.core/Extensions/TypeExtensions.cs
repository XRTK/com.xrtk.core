// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/> instances.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();

        /// <summary>
        /// Attempts to resolve the class reference by the <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        /// <param name="classRef">The <see cref="Type.AssemblyQualifiedName"/> of the type to get.</param>
        /// <returns>The <see cref="Type"/> from <see cref="classRef"/></returns>
        public static Type ResolveType(string classRef)
        {
            if (!TypeMap.TryGetValue(classRef, out var type))
            {
                type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
                TypeMap[classRef] = type;
            }

            return type;
        }

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
