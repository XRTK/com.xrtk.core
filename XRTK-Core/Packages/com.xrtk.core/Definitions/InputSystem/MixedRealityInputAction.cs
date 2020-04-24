// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Control like a Button, Joystick, Sensor, etc.
    /// </summary>
    [Serializable]
    public struct MixedRealityInputAction : IEqualityComparer, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description"></param>
        private MixedRealityInputAction(string description) : this()
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException($"{nameof(description)} cannot be empty");
            }

            Profile = DefaultGuidString;
            this.id = 0;
            this.description = description;
            this.axisConstraint = AxisType.None;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="axisConstraint"></param>
        public MixedRealityInputAction(uint id, string description, AxisType axisConstraint = AxisType.None) : this(description)
        {
            if (id == 0 && description != "None")
            {
                throw new ArgumentException($"{nameof(id)} cannot be 0");
            }

            this.id = id;
            this.axisConstraint = axisConstraint;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profileGuid"></param>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="axisConstraint"></param>
        public MixedRealityInputAction(Guid profileGuid, uint id, string description, AxisType axisConstraint = AxisType.None)
            : this(id, description, axisConstraint)
        {
            Profile = profileGuid.ToString("N");
        }

        /// <summary>
        /// Default input action that doesn't represent any defined action.
        /// </summary>
        /// <remarks>
        /// Any action that has an id of 0 is considered the same as "None".
        /// </remarks>
        public static readonly MixedRealityInputAction None = new MixedRealityInputAction("None");

        private static readonly string DefaultGuidString = default(Guid).ToString("N");

        [SerializeField]
        private string profileGuid;

        private string Profile
        {
            get => profileGuid;
            set
            {
                profileGuid = value;

                if (Guid.TryParse(profileGuid, out var temp))
                {
                    ProfileGuid = temp;
                }
            }
        }

        /// <summary>
        /// The guid reference to the <see cref="MixedRealityInputActionsProfile"/> this action belongs to.
        /// </summary>
        public Guid ProfileGuid { get; private set; }

        [SerializeField]
        private uint id;

        /// <summary>
        /// The Unique Id of this Input Action.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        private string description;

        /// <summary>
        /// A short description of the Input Action.
        /// </summary>
        public string Description => description;

        [SerializeField]
        private AxisType axisConstraint;

        /// <summary>
        /// The Axis constraint for the Input Action
        /// </summary>
        public AxisType AxisConstraint => axisConstraint;

        public static bool operator ==(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return !left.Equals(right);
        }

        #region IEqualityComparer Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            if (left is null || right is null) { return false; }
            if (!(left is MixedRealityInputAction) || !(right is MixedRealityInputAction)) { return false; }
            return ((MixedRealityInputAction)left).Equals((MixedRealityInputAction)right);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        public bool Equals(MixedRealityInputAction other)
        {
            // TODO remove backwards compatibility for actions that haven't been re-serialized.
            if (ProfileGuid == default && Id != 0 ||
                other.ProfileGuid == default && other.Id != 0)
            {
                return Id == other.Id && AxisConstraint == other.AxisConstraint;
            }

            return ProfileGuid == other.ProfileGuid &&
                   Id == other.Id &&
                   AxisConstraint == other.AxisConstraint;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return !(obj is null) && obj is MixedRealityInputAction action && Equals(action);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is MixedRealityInputAction action ? action.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return $"{ProfileGuid}.{Id}.{AxisConstraint}".GetHashCode();
        }

        #endregion IEqualityComparer Implementation

        #region ISerializationCallbackReceiver Implementation

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            Profile = ProfileGuid.ToString("N");
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (Guid.TryParse(profileGuid, out var temp))
            {
                ProfileGuid = temp;
            }
        }

        #endregion ISerializationCallbackReceiver Implementation
    }
}
