// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Unity's <see cref="JsonUtility"/> "currently" (stated in 2011) does not support top-level arrays.
    /// But hey, it's on their "future" road-map. That's why we need a wrapper definition around or items.
    /// </summary>
    [Serializable]
    public class RecordedHandJoints
    {
        [SerializeField]
        private RecordedHandJoint[] joints;

        /// <summary>
        /// Gets the joints recorded in this data set.
        /// </summary>
        public RecordedHandJoint[] Joints
        {
            get => joints;
            set => joints = value;
        }
    }
}