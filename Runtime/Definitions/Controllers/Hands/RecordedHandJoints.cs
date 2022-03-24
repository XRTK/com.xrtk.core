// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Wrapper definition around <see cref="RecordedHandJoint"/> for storing an array of <see cref="RecordedHandJoint"/>s,
    /// since Unity's <see cref="JsonUtility"/> does not support top-level arrays in JSON.
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