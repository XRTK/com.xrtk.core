﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using XRTK.Definitions.Utilities;

namespace XRTK.Attributes
{
    /// <summary>
    /// Constraint that allows selection of classes that implement a specific interface
    /// when selecting a <see cref="SystemType"/> with the Unity inspector.
    /// </summary>
    public sealed class ImplementsAttribute : SystemTypeAttribute
    {
        /// <summary>
        /// Gets the type of interface that selectable classes must implement.
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        /// <param name="grouping">Gets or sets grouping of selectable classes. Defaults to <see cref="TypeGrouping.ByNamespaceFlat"/> unless explicitly specified.</param>
        public ImplementsAttribute(Type interfaceType, TypeGrouping grouping)
                : base(interfaceType, grouping)
        {
            InterfaceType = interfaceType;
        }

        /// <inheritdoc />
        public override bool IsConstraintSatisfied(Type type)
        {
            return base.IsConstraintSatisfied(type) && type.GetInterfaces().Any(t => t == InterfaceType);
        }
    }
}