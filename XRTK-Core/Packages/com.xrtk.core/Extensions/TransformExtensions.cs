// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Transform class
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// An extension method that will get you the full path to an object.
        /// </summary>
        /// <param name="transform">The transform you wish a full path to.</param>
        /// <param name="delimiter">The delimiter with which each object is delimited in the string.</param>
        /// <param name="prefix">Prefix with which the full path to the object should start.</param>
        /// <returns>A delimited string that is the full path to the game object in the hierarchy.</returns>
        public static string GetFullPath(this Transform transform, string delimiter = ".", string prefix = "/")
        {
            var stringBuilder = new StringBuilder();
            GetFullPath(stringBuilder, transform, delimiter, prefix);
            return stringBuilder.ToString();
        }

        private static void GetFullPath(StringBuilder stringBuilder, Transform transform, string delimiter, string prefix)
        {
            if (transform.parent == null)
            {
                stringBuilder.Append(prefix);
            }
            else
            {
                GetFullPath(stringBuilder, transform.parent, delimiter, prefix);
                stringBuilder.Append(delimiter);
            }

            stringBuilder.Append(transform.name);
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }
            return root.EnumerateHierarchyCore(new List<Transform>(0));
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object except for the branches in ignore.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        /// <param name="ignore">Transforms and all its children to be ignored</param>
        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root, ICollection<Transform> ignore)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }
            if (ignore == null)
            {
                throw new ArgumentNullException(nameof(ignore), "Ignore collection can't be null, use EnumerateHierarchy(root) instead.");
            }
            return root.EnumerateHierarchyCore(ignore);
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object except for the branches in ignore.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        /// <param name="ignore">Transforms and all its children to be ignored</param>
        private static IEnumerable<Transform> EnumerateHierarchyCore(this Transform root, ICollection<Transform> ignore)
        {
            var transformQueue = new Queue<Transform>();
            transformQueue.Enqueue(root);

            while (transformQueue.Count > 0)
            {
                var parentTransform = transformQueue.Dequeue();

                if (!parentTransform || ignore.Contains(parentTransform)) { continue; }

                for (var i = 0; i < parentTransform.childCount; i++)
                {
                    transformQueue.Enqueue(parentTransform.GetChild(i));
                }

                yield return parentTransform;
            }
        }

        /// <summary>
        /// Calculates the bounds of all the colliders attached to this GameObject and all it's children
        /// </summary>
        /// <param name="transform">
        /// Transform of root GameObject the colliders are attached to.
        /// </param>
        /// <param name="syncTransform">
        /// True, by default, this will sync the <see cref="transform"/> rotation to calculate the axis aligned orientation.
        /// </param>
        /// <returns>The total bounds of all colliders attached to this GameObject.
        /// If no colliders attached, returns a bounds of center and extents 0</returns>
        public static Bounds GetColliderBounds(this Transform transform, bool syncTransform = true)
        {
            // Store current rotation then zero out the rotation so that the bounds
            // are computed when the object is in its 'axis aligned orientation'.
            var currentRotation = transform.rotation;

            if (syncTransform)
            {
                transform.rotation = Quaternion.identity;
                Physics.SyncTransforms(); // Update collider bounds
            }

            var colliders = transform.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0) { return default; }

            var bounds = colliders[0].bounds;

            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }

            if (syncTransform)
            {
                // After bounds are computed, restore rotation...
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.rotation = currentRotation;
                Physics.SyncTransforms();
            }

            return bounds;
        }

        /// <summary>
        /// Calculates the bounds of all the renderers attached to this GameObject and all it's children
        /// </summary>
        /// <param name="transform">
        /// Transform of root GameObject the renderers are attached to.
        /// </param>
        /// <param name="syncTransform">
        /// True, by default, this will sync the <see cref="transform"/> rotation to calculate the axis aligned orientation.
        /// </param>
        /// <returns>The total bounds of all renderers attached to this GameObject.
        /// If no renderers attached, returns a bounds of center and extents 0</returns>
        public static Bounds GetRenderBounds(this Transform transform, bool syncTransform = true)
        {
            // Store current rotation then zero out the rotation so that the bounds
            // are computed when the object is in its 'axis aligned orientation'.
            var currentRotation = transform.rotation;

            if (syncTransform)
            {
                transform.rotation = Quaternion.identity;
                Physics.SyncTransforms(); // Update collider bounds
            }

            var renderers = transform.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0) { return default; }

            var bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            if (syncTransform)
            {
                // After bounds are computed, restore rotation...
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.rotation = currentRotation;
                Physics.SyncTransforms();
            }

            return bounds;
        }

        /// <summary>
        /// Checks if the provided transforms are child/parent related.
        /// </summary>
        /// <returns>True if either transform is the parent of the other or if they are the same</returns>
        public static bool IsParentOrChildOf(this Transform transform1, Transform transform2)
        {
            return transform1.IsChildOf(transform2) || transform2.IsChildOf(transform1);
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the specified transform.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="startTransform">Transform for which ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified transform should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this Transform startTransform, bool includeSelf = true) where T : Component
        {
            foreach (var transform in startTransform.EnumerateAncestors(includeSelf))
            {
                T component = transform.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Enumerates the ancestors of the specified transform.
        /// </summary>
        /// <param name="startTransform">Transform for which ancestors must be returned.</param>
        /// <param name="includeSelf">Indicates whether the specified transform should be included.</param>
        /// <returns>An enumeration of all ancestor transforms of the specified start transform.</returns>
        public static IEnumerable<Transform> EnumerateAncestors(this Transform startTransform, bool includeSelf)
        {
            if (!includeSelf)
            {
                startTransform = startTransform.parent;
            }

            for (var transform = startTransform; transform != null; transform = transform.parent)
            {
                yield return transform;
            }
        }

        /// <summary>
        /// Transforms the size from local to world.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="localSize">The local size.</param>
        /// <returns>World size.</returns>
        public static Vector3 TransformSize(this Transform transform, Vector3 localSize)
        {
            var transformedSize = localSize;
            var transformedCopy = transform;

            do
            {
                var localScale = transformedCopy.localScale;
                transformedSize.x *= localScale.x;
                transformedSize.y *= localScale.y;
                transformedSize.z *= localScale.z;
                transformedCopy = transformedCopy.parent;
            }
            while (transformedCopy != null);

            return transformedSize;
        }

        /// <summary>
        /// Transforms the size from world to local.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="worldSize">The world size</param>
        /// <returns>World size.</returns>
        public static Vector3 InverseTransformSize(this Transform transform, Vector3 worldSize)
        {
            var transformedSize = worldSize;
            var transformedCopy = transform;

            do
            {
                var localScale = transformedCopy.localScale;
                transformedSize.x /= localScale.x;
                transformedSize.y /= localScale.y;
                transformedSize.z /= localScale.z;
                transformedCopy = transformedCopy.parent;
            }
            while (transformedCopy != null);

            return transformedSize;
        }

        /// <summary>
        /// Gets the hierarchical depth of the Transform from its root. Returns -1 if the transform is the root.
        /// </summary>
        /// <param name="transform">The transform to get the depth for.</param>
        /// <returns></returns>
        public static int GetDepth(this Transform transform)
        {
            int depth = -1;

            Transform root = transform.root;

            if (root == transform)
            {
                return depth;
            }

            TryGetDepth(transform, root, ref depth);

            return depth;
        }

        /// <summary>
        /// Tries to get the hierarchical depth of the Transform from the specified parent. This method is recursive.
        /// </summary>
        /// <param name="target">The transform to get the depth for</param>
        /// <param name="parent">The starting transform to look for the target transform in</param>
        /// <param name="depth">The depth of the target transform</param>
        /// <returns>'true' if the depth could be retrieved, or 'false' because the transform is a root transform.</returns>
        public static bool TryGetDepth(Transform target, Transform parent, ref int depth)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                depth++;
                var child = parent.GetChild(i);

                if (child == target.transform || TryGetDepth(target, child, ref depth))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get a point on the <see cref="Bounds"/> edge in the specified direction
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3 GetPointOnBoundsEdge(this Transform transform, Vector3 direction, Bounds bounds = default)
        {
            if (direction != Vector3.zero)
            {
                direction /= Mathf.Max(Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.y)));
            }

            if (bounds == default)
            {
                bounds = transform.GetColliderBounds();
            }

            return bounds.center + Vector3.Scale(bounds.size, direction * 0.5f);
        }

        /// <summary>
        /// Given 2 Transforms, return a common root Transform (or null).
        /// </summary>
        /// <param name="t1">Transform to compare</param>
        /// <param name="t2">Transform to compare</param>
        public static Transform FindCommonRoot(this Transform t1, Transform t2)
        {
            if (t1 == null || t2 == null)
            {
                return null;
            }

            var t2root = t2;

            while (t1 != null)
            {
                while (t2 != null)
                {
                    if (t1 == t2)
                    {
                        return t1;
                    }

                    t2 = t2.parent;

                    if (t2 == null)
                    {
                        if (t1 == null)
                        {
                            break;
                        }

                        t1 = t1.parent;
                        t2 = t2root;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the collider and all child colliders active with the provided value.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="isActive"></param>
        public static void SetCollidersActive(this Transform transform, bool isActive)
        {
            var colliders = transform.GetComponentsInChildren<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = isActive;
            }
        }

        /// <summary>
        /// Sets the physics layer on this and all child <see cref="Transform"/>s with the provided value.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.SetLayerRecursively(layer);
        }

        /// <summary>
        /// Scales the target <see cref="Transform"/> by the provided <see cref="pivot"/> position using the
        /// provided <see cref="newScale"/>.
        /// <para/>
        /// Similar to how <seealso cref="Transform.Rotate(Vector3,Space)"/> works.
        /// </summary>
        /// <remarks>
        /// https://answers.unity.com/questions/14170/scaling-an-object-from-a-different-center.html
        /// </remarks>
        /// <param name="target"></param>
        /// <param name="pivot"></param>
        /// <param name="newScale"></param>
        public static void ScaleAround(this Transform target, Vector3 pivot, Vector3 newScale)
        {
            var point = target.localPosition;
            var distance = point - pivot; // diff from object pivot to desired pivot/origin
            var relativeScale = newScale.x / target.localScale.x; // relative scale factor
            var finalPosition = pivot + distance * relativeScale; // calc final position post-scale

            // finally, actually perform the scale / translation
            target.localScale = newScale;
            target.localPosition = finalPosition;
        }
    }
}