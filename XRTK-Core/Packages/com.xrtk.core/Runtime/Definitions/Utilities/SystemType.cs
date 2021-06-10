// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class SystemType : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="property">The serialized property of the type reference.</param>
        public SystemType(UnityEditor.SerializedProperty property)
        {
            TypeExtensions.TryResolveType(property.FindPropertyRelative(nameof(reference)).stringValue, out var resolvedType);
            Type = resolvedType;
        }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        public SystemType(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="typeGuid"><see cref="T:Type.GUID"/> reference as <see cref="string"/>.</param>
        public SystemType(string typeGuid)
        {
            TypeExtensions.TryResolveType(typeGuid, out var resolvedType);
            Type = resolvedType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="guid"><see cref="T:Type.GUID"/> reference of the type to instantiate.</param>
        public SystemType(Guid guid)
        {
            TypeExtensions.TryResolveType(guid, out var resolvedType);
            Type = resolvedType;
        }

        #region ISerializationCallbackReceiver Members

        [SerializeField]
        private string reference = string.Empty;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            TypeExtensions.TryResolveType(reference, out type);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        #endregion ISerializationCallbackReceiver Members

        private Type type;

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        public Type Type
        {
            get => type;
            set
            {
                if (value != null)
                {
                    bool isValid = value.IsValueType && !value.IsEnum && !value.IsAbstract || value.IsClass;

                    if (!isValid)
                    {
                        Debug.LogError($"'{value.FullName}' is not a valid class or struct type.");
                    }
                }

                type = value;

                if (type != null)
                {
                    guid = type.GUID;
                    reference = guid.ToString();
                }
                else
                {
                    guid = Guid.Empty;
                    reference = string.Empty;
                }
            }
        }

        public static implicit operator Guid(SystemType type)
        {
            return type.type == null ? Guid.Empty : type.Guid;
        }

        public static implicit operator string(SystemType type)
        {
            return type.reference;
        }

        public static implicit operator Type(SystemType type)
        {
            return type?.Type;
        }

        public static implicit operator SystemType(Type type)
        {
            return new SystemType(type);
        }

        public static implicit operator SystemType(Guid guid)
        {
            return new SystemType(guid);
        }

        private Guid guid = Guid.Empty;

        /// <summary>
        /// Gets the <see cref="T:System.Guid"/> associated with the <see cref="T:System.Type" />.
        /// </summary>
        public Guid Guid
        {
            get
            {
                if (guid == Guid.Empty)
                {
                    guid = type.GUID;
                }

                return guid;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Type?.FullName ?? (string.IsNullOrWhiteSpace(reference) ? "{None}" : reference);
        }
    }
}
