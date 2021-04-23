// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace XRTK.Definitions.Physics
{
    /// <summary>
    /// Contains information about which game object has the focus currently.
    /// Also contains information about the normal of that point.
    /// </summary>
    public struct FocusDetails
    {
        /// <summary>
        /// Distance along the ray until a hit, or until the end of the ray if no hit
        /// </summary>
        public float RayDistance { get; set; }

        /// <summary>
        /// The hit point of the raycast.
        /// </summary>
        public Vector3 EndPoint { get; set; }

        /// <summary>
        /// The hit point of the raycast in local space in relation to the focused object.
        /// </summary>
        public Vector3 EndPointLocalSpace { get; set; }

        /// <summary>
        /// The normal of the raycast.
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// The normal of the raycast in local space in relation to the focused object.
        /// </summary>
        public Vector3 NormalLocalSpace { get; set; }

        /// <summary>
        /// The object hit by the last raycast.
        /// </summary>
        public GameObject HitObject { get; set; }

        /// <summary>
        /// The last raycast hit info.
        /// </summary>
        public RaycastHit LastRaycastHit { get; set; }

        /// <summary>
        /// The last raycast hit info for graphic raycast
        /// </summary>
        public RaycastResult LastGraphicsRaycastResult { get; set; }
    }
}
