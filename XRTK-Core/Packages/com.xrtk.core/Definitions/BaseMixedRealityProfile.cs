// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions
{
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        [SerializeField]
        private bool isCustomProfile = true;

        internal bool IsCustomProfile => isCustomProfile;

        [SerializeField]
        private BaseMixedRealityProfile parentProfile = null;

        /// <summary>
        /// The profile's parent.
        /// </summary>
        public BaseMixedRealityProfile ParentProfile
        {
            get => parentProfile;
            internal set => parentProfile = value;
        }
    }
}