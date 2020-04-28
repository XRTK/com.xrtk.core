// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using XRTK.Interfaces;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/> instances.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<Guid, Type> TypeCache = new Dictionary<Guid, Type>();

        private static IEnumerable<Type> allTypes = null;

        private static IEnumerable<Type> AllTypes => allTypes ?? (allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract));

        /// <summary>
        /// Attempts to resolve the type using a the <see cref="System.Type.AssemblyQualifiedName"/> or <see cref="System.Type.GUID"/> as <see cref="string"/>.
        /// </summary>
        /// <param name="typeRef">The <see cref="System.Type.GUID"/> or <see cref="System.Type.AssemblyQualifiedName"/> as <see cref="string"/>.</param>
        /// <param name="resolvedType"></param>
        /// <returns>The resolved <see cref="Type"/></returns>
        public static bool TryResolveType(string typeRef, out Type resolvedType)
        {
            resolvedType = null;

            if (string.IsNullOrEmpty(typeRef)) { return false; }

            if (Guid.TryParse(typeRef, out var guid))
            {
                if (!TypeCache.TryGetValue(guid, out resolvedType))
                {
                    resolvedType = AllTypes.FirstOrDefault(type => type.GUID == guid);
                }
            }
            else
            {
                resolvedType = Type.GetType(typeRef);

                if (resolvedType != null &&
                    resolvedType.GUID != Guid.Empty)
                {
                    guid = resolvedType.GUID;
                }
            }

            if (resolvedType != null && !resolvedType.IsAbstract)
            {
                if (!TypeCache.ContainsKey(guid))
                {
                    if (resolvedType.GUID == Guid.Empty)
                    {
                        Debug.LogWarning($"{resolvedType.Name} is missing a {nameof(GuidAttribute)}. This extension has been upgraded to use System.Type.GUID instead of System.Type.AssemblyQualifiedName");
                    }

                    TypeCache.Add(guid, resolvedType);
                }

                return true;
            }

            return false;
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
