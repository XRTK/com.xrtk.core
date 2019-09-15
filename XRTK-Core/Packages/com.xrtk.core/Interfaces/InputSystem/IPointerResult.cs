// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace XRTK.Interfaces.InputSystem
{
    /// <summary>
    /// Interface defining a pointer result.
    /// </summary>
    public interface IPointerResult
    {
        /// <summary>
        /// The starting point of the Pointer RaySteps.
        /// </summary>
        Vector3 StartPoint { get; }

        /// <summary>
        /// The ending point of the Pointer RaySteps.
        /// </summary>
        Vector3 EndPoint { get; }

        /// <summary>
        /// The hit point of the raycast in local space in relation to the focused object.
        /// </summary>
        Vector3 EndPointLocalSpace { get; }

        /// <summary>
        /// The current pointer's target <see cref="GameObject"/>
        /// </summary>
        GameObject CurrentPointerTarget { get; }

        /// <summary>
        /// The previous pointer target.
        /// </summary>
        GameObject PreviousPointerTarget { get; }

        /// <summary>
        /// The last <see cref="GameObject"/> hit by the raycast.
        /// </summary>
        /// <remarks>
        /// This may be different from the <see cref="CurrentPointerTarget"/> if the pointer is locked or synced.
        /// </remarks>
        GameObject LastHitObject { get; }

        /// <summary>
        /// The index of the step that produced the last raycast hit, 0 when no raycast hit.
        /// </summary>
        int RayStepIndex { get; }

        /// <summary>
        /// Distance along the ray until a hit, or until the end of the ray if no hit.
        /// </summary>
        float RayDistance { get; }

        /// <summary>
        /// The normal of the raycast.
        /// </summary>
        Vector3 Normal { get; }

        /// <summary>
        /// The normal of the raycast in local space in relation to the focused object.
        /// </summary>
        Vector3 NormalLocalSpace { get; }

        /// <summary>
        /// The direction this pointer is traveling, calculated from the last known position.
        /// </summary>
        Vector3 Direction { get; }

        /// <summary>
        /// The last physics raycast hit info.
        /// </summary>
        RaycastHit LastRaycastHit { get; }

        /// <summary>
        /// The last raycast hit info for graphic raycast.
        /// </summary>
        RaycastResult LastGraphicsRaycastResult { get; }

        /// <summary>
        /// The current grab position of the <see cref="CurrentPointerTarget"/> in world space.
        /// </summary>
        Vector3 GrabPoint { get; }

        /// <summary>
        /// The current grab position of the <see cref="CurrentPointerTarget"/> in local space.
        /// </summary>
        Vector3 GrabPointLocalSpace { get; }
    }
}