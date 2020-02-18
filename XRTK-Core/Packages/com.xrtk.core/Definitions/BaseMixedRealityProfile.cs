// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;

namespace XRTK.Definitions
{
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("isCustomProfile")]
        [FormerlySerializedAs("isDefaultProfile")]
        private bool isEditable = true;

        /// <summary>
        /// Is this profile in an editable state?
        /// </summary>
        internal bool IsEditable
        {
            get => isEditable;
            set => isEditable = value;
        }

        /// <summary>
        /// The profile's parent in the service graph hierarchy.
        /// </summary>
        public BaseMixedRealityProfile ParentProfile { get; internal set; } = null;
    }
}