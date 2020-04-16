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
    /// <remarks>The <see cref="JsonUtility"/> also does not support properties, so we gotta use fields. Oh and it also
    /// does not support auto mapping of JSON naming "items" to C# naming "Items".</remarks>
    [Serializable]
    public class RecordedHandJoints
    {
        /// <summary>
        /// Gets the joints recorded in this data set.
        /// </summary>
        public RecordedHandJoint[] items;
    }
}