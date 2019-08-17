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
        /// The offset distance of the <see cref="CurrentPointerTarget"/>'s position minus the <see cref="EndPoint"/>.
        /// </summary>
        /// <remarks>
        /// If there's isn't an active <see cref="CurrentPointerTarget"/>, then the Vector3.zero is returned.
        /// </remarks>
        Vector3 Offset { get; }

        /// <summary>
        /// The offset distance of the <see cref="CurrentPointerTarget"/>'s position minus the <see cref="EndPoint"/> in local space.
        /// </summary>
        Vector3 OffsetLocalSpace { get; }

        /// <summary>
        /// The last physics raycast hit info.
        /// </summary>
        RaycastHit LastRaycastHit { get; }

        /// <summary>
        /// The last raycast hit info for graphic raycast.
        /// </summary>
        RaycastResult LastGraphicsRaycastResult { get; }
    }
}