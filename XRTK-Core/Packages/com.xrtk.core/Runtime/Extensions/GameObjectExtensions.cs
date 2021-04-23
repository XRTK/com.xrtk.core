// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets the child <see cref="GameObject"/> states.
        /// </summary>
        /// <param name="root">The root <see cref="GameObject"/>.</param>
        /// <param name="isActive">The active value to set.</param>
        public static void SetChildrenActive(this GameObject root, bool isActive)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                root.transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        public static void SetLayerRecursively(this GameObject root, int layer)
        {
            foreach (var child in root.transform.EnumerateHierarchy())
            {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it and cache the previous layers in the out parameter.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        /// <param name="cache">The previously set layer for each object</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/></exception>
        public static void SetLayerRecursively(this GameObject root, int layer, out Dictionary<GameObject, int> cache)
        {
            cache = new Dictionary<GameObject, int>();

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                var childGameObject = child.gameObject;
                cache[childGameObject] = childGameObject.layer;
                childGameObject.layer = layer;
            }
        }

        /// <summary>
        /// Reapplies previously cached hierarchy layers
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="cache">The previously set layer for each object</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="cache"/> is <see langword="null"/></exception>
        public static void ApplyLayerCacheRecursively(this GameObject root, Dictionary<GameObject, int> cache)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }
            if (cache == null) { throw new ArgumentNullException(nameof(cache)); }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                var childGameObject = child.gameObject;
                if (!cache.TryGetValue(childGameObject, out var layer)) { continue; }
                childGameObject.layer = layer;
                cache.Remove(childGameObject);
            }
        }

        /// <summary>
        /// Determines whether or not a game object's layer is included in the specified layer mask.
        /// </summary>
        /// <param name="gameObject">The game object whose layer to test.</param>
        /// <param name="layerMask">The layer mask to test against.</param>
        /// <returns>True if <paramref name="gameObject"/>'s layer is included in <paramref name="layerMask"/>, false otherwise.</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            LayerMask gameObjectMask = 1 << gameObject.layer;
            return (gameObjectMask & layerMask) == gameObjectMask;
        }

        /// <summary>
        /// Apply the specified delegate to all objects in the hierarchy under a specified game object.
        /// </summary>
        /// <param name="root">Root game object of the hierarchy.</param>
        /// <param name="action">Delegate to apply.</param>
        public static void ApplyToHierarchy(this GameObject root, Action<GameObject> action)
        {
            action(root);
            var items = root.GetComponentsInChildren<Transform>();

            for (var i = 0; i < items.Length; i++)
            {
                action(items[i].gameObject);
            }
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the specified game object.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="gameObject">Game object for which ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified game object should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this GameObject gameObject, bool includeSelf = true) where T : Component
        {
            return gameObject.transform.FindAncestorComponent<T>(includeSelf);
        }

        /// <summary>
        /// Perform an action on every component of type T that is on this GameObject
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <param name="gameObject">this gameObject</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEachComponent<T>(this GameObject gameObject, Action<T> action)
        {
            foreach (T i in gameObject.GetComponents<T>())
            {
                action(i);
            }
        }

        /// <summary>
        /// Sets the <see cref="Renderer"/>s and optionally <see cref="Collider"/>s in this <see cref="GameObject"/> to the provided <see cref="bool"/> state.
        /// </summary>
        /// <remarks>If the GameObject or it's children are inactive, they will be set active, but the GameObjects will not be set inactive.</remarks>
        /// <param name="gameObject"></param>
        /// <param name="isActive"></param>
        /// <param name="includeColliders"></param>
        public static void SetRenderingActive(this GameObject gameObject, bool isActive, bool includeColliders = true)
        {
            if (isActive)
            {
                gameObject.SetActive(true);
            }

            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isActive;
            }

            foreach (var graphic in gameObject.GetComponentsInChildren<Graphic>())
            {
                graphic.enabled = isActive;
            }

            if (includeColliders)
            {
                foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = isActive;
                }
            }
        }

        /// <summary>
        /// Given 2 GameObjects, return a common root GameObject (or null).
        /// </summary>
        /// <param name="g1">GameObject to compare</param>
        /// <param name="g2">GameObject to compare</param>
        public static GameObject FindCommonRoot(this GameObject g1, GameObject g2)
        {
            if (g1 == null || g2 == null)
            {
                return null;
            }

            var t1 = g1.transform;

            while (t1 != null)
            {
                var t2 = g2.transform;

                while (t2 != null)
                {
                    if (t1 == t2)
                    {
                        return t1.gameObject;
                    }

                    t2 = t2.parent;
                }

                t1 = t1.parent;
            }

            return null;
        }

        /// <summary>
        /// Checks if the provided GameObjects are child/parent related.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns>True if either GameObject is the parent of the other or if they are the same</returns>
        public static bool IsParentOrChildOf(this GameObject g1, GameObject g2)
        {
            if (g1 == null || g2 == null)
            {
                return false;
            }

            var t1 = g1.transform;
            var t2 = g2.transform;

            if (t1 == null || t2 == null)
            {
                return false;
            }

            return t1.IsParentOrChildOf(t2);
        }

        /// <summary>
        /// Gets a <see cref="Component"/> on the <see cref="GameObject"/> if it's already attached to it.
        /// If the <see cref="Component"/> does not exist on the <see cref="GameObject"/>, it will be added and returned.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Component"/> to lookup on the <see cref="GameObject"/>.</typeparam>
        /// <param name="gameObject"><see cref="GameObject"/> instance.</param>
        /// <returns>The existing or new instance of <see cref="Component"/>.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component.IsNull() ? gameObject.AddComponent<T>() : component;
        }
    }
}
