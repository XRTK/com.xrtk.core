﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extensions methods for the Unity Component class.
    /// This also includes some component-related extensions for the GameObject class.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Ensure that a component of type <typeparamref name="T"/> exists on the game object.
        /// If it doesn't exist, creates it.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="component">A component on the game object for which a component of type <typeparamref name="T"/> should exist.</param>
        /// <returns>The component that was retrieved or created.</returns>
        public static T EnsureComponent<T>(this Component component) where T : Component
        {
            return EnsureComponent<T>(component.gameObject);
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the game object of the specified component.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="component">Component for which its game object's ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified game object should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this Component component, bool includeSelf = true) where T : Component
        {
            return component.transform.FindAncestorComponent<T>(includeSelf);
        }

        /// <summary>
        /// Ensure that a component of type <typeparamref name="T"/> exists on the game object.
        /// If it doesn't exist, creates it.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="gameObject">Game object on which component should be.</param>
        /// <returns>The component that was retrieved or created.</returns>
        /// <remarks>
        /// This extension has to remain in this class as it is required by the <see cref="EnsureComponent{T}(Component)"/> method
        /// </remarks>
        public static T EnsureComponent<T>(this GameObject gameObject) where T : Component
        {
            T foundComponent = gameObject.GetComponent<T>();
            return foundComponent.IsNull() ? gameObject.AddComponent<T>() : foundComponent;
        }

        /// <summary>
        /// Ensure that a component of type exists on the game object.
        /// If it doesn't exist, creates it.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="component">A component on the game object for which a component of type should exist.</param>
        /// <returns>The component that was retrieved or created.</returns>
        public static Component EnsureComponent(this GameObject gameObject, Type component)
        {
            var foundComponent = gameObject.GetComponent(component);
            return foundComponent.IsNull() ? gameObject.AddComponent(component) : foundComponent;
        }

        /// <summary>
        /// Validates the <see cref="Component"/> reference.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Component"/>.</typeparam>
        /// <param name="component">The target <see cref="Component"/>.</param>
        /// <param name="callerName">The <see cref="CallerFilePathAttribute"/> fills in this information.</param>
        public static void Validate<T>(this T component, [CallerFilePath] string callerName = "") where T : Component
        {
            if (component.IsNull())
            {
                throw new MissingReferenceException($"{Path.GetFileNameWithoutExtension(callerName)} expected a {typeof(T).Name}");
            }
        }

        /// <summary>
        /// Sets the <see cref="GameObject"/> this <see cref="Component"/> is attached to, to the specified state.
        /// </summary>
        /// <param name="component">The target <see cref="Component"/></param>
        /// <param name="isActive">The <see cref="GameObject"/>'s active state to set.</param>
        public static void SetActive(this Component component, bool isActive)
        {
            if (component.gameObject.activeInHierarchy != isActive)
            {
                component.gameObject.SetActive(isActive);
            }
        }
    }
}
