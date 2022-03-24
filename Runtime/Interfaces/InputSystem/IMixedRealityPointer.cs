﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Physics;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Physics;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for handling pointers.
    /// </summary>
    public interface IMixedRealityPointer : IEqualityComparer
    {
        /// <summary>
        /// This pointer's id.
        /// </summary>
        uint PointerId { get; }

        /// <summary>
        /// This pointer's name.
        /// </summary>
        string PointerName { get; set; }

        /// <summary>
        /// The pointer's current controller reference.
        /// </summary>
        IMixedRealityController Controller { get; set; }

        /// <summary>
        /// This pointer's input source parent.
        /// </summary>
        IMixedRealityInputSource InputSourceParent { get; }

        /// <summary>
        /// The pointer's cursor.
        /// </summary>
        IMixedRealityCursor BaseCursor { get; set; }

        /// <summary>
        /// The currently active cursor modifier.
        /// </summary>
        ICursorModifier CursorModifier { get; set; }

        /// <summary>
        /// The <see cref="InteractionMode"/> for this pointer.
        /// </summary>
        InteractionMode InteractionMode { get; }

        /// <summary>
        /// The <see cref="Collider"/> used for determining and raising near interactions.
        /// </summary>
        Collider NearInteractionCollider { get; }

        /// <summary>
        /// Has the conditions been satisfied to enable the interaction of this pointer?
        /// </summary>
        bool IsInteractionEnabled { get; }

        /// <summary>
        /// The pointer's extent when raycasting.
        /// </summary>
        float PointerExtent { get; set; }

        /// <summary>
        /// The default pointer extent.
        /// </summary>
        float DefaultPointerExtent { get; }

        /// <summary>
        /// The pointers raycast collection.
        /// </summary>
        RayStep[] Rays { get; }

        /// <summary>
        /// The Physics Layers, in prioritized order, that are used to determine the <see cref="IPointerResult.CurrentPointerTarget"/> when raycasting.
        /// </summary>
        /// <remarks>
        /// If set, will override the <see cref="MixedRealityInputSystemProfile.PointerRaycastLayerMasks"/>'s default raycasting layer mask array.
        /// </remarks>
        /// <example>
        /// Allow the pointer to hit SR, but first prioritize any <see cref="Physics.DefaultRaycastLayers"/> (potentially behind SR)
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// IMixedRealityPointer.PointerRaycastLayerMasksOverride = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        LayerMask[] PointerRaycastLayerMasksOverride { get; set; }

        /// <summary>
        /// The currently targeted focus handler.
        /// </summary>
        IMixedRealityFocusHandler FocusHandler { get; set; }

        /// <summary>
        /// The currently targeted input handler.
        /// </summary>
        IMixedRealityInputHandler InputHandler { get; set; }

        /// <summary>
        /// The combined physics and graphics raycast pointer result.
        /// </summary>
        IPointerResult Result { get; set; }

        /// <summary>
        /// Is the focus currently locked to the current <see cref="IPointerResult.CurrentPointerTarget"/>?
        /// </summary>
        /// <remarks>
        /// This effectively means that the focused <see cref="IPointerResult.CurrentPointerTarget"/> result cannot be changed,
        /// even though the focus data may return hit information that is not the <see cref="IPointerResult.CurrentPointerTarget"/>.
        /// </remarks>
        bool IsFocusLocked { get; set; }

        /// <summary>
        /// Is the <see cref="IPointerResult.CurrentPointerTarget"/>'s position is locked in sync with the <see cref="Result"/> position?
        /// </summary>
        /// <remarks>
        /// When true, the <see cref="IPointerResult.CurrentPointerTarget"/>s position will also be updated to move in sync with the <see cref="Result"/> position.
        /// This position is calculated using the offset of the offset between the <see cref="IPointerResult.CurrentPointerTarget"/> position and the <see cref="Result"/> position.
        /// </remarks>
        GameObject SyncedTarget { get; set; }

        /// <summary>
        /// Overrides the <see cref="SyncedTarget"/>'s grab point.
        /// </summary>
        Vector3? OverrideGrabPoint { get; set; }

        /// <summary>
        /// Ray stabilizer used when calculating position of pointer end point.
        /// </summary>
        IBaseRayStabilizer RayStabilizer { get; set; }

        /// <summary>
        /// The physics raycast mode to use.
        /// </summary>
        RaycastMode RaycastMode { get; set; }

        /// <summary>
        /// The radius to use when <see cref="RaycastMode"/> is set to Sphere.
        /// </summary>
        float SphereCastRadius { get; set; }

        /// <summary>
        /// The Y orientation of the pointer - used for touchpad rotation and navigation
        /// </summary>
        float PointerOrientation { get; }

        /// <summary>
        /// Called before all rays have casted.
        /// </summary>
        void OnPreRaycast();

        /// <summary>
        /// Called after all rays have casted.
        /// </summary>
        void OnPostRaycast();

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources support positional information, and those that do may not always have it available.
        /// </summary>
        /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPointerPosition(out Vector3 position);

        /// <summary>
        /// Returns the pointing ray of the input source, if available.
        /// Not all input sources support pointing information, and those that do may not always have it available.
        /// </summary>
        /// <param name="pointingRay">Out parameter filled with the pointing ray if available.</param>
        /// <returns>True if a pointing ray was retrieved, false if not.</returns>
        bool TryGetPointingRay(out Ray pointingRay);

        /// <summary>
        /// Returns the rotation of the input source, if available.
        /// Not all input sources support rotation information, and those that do may not always have it available.
        /// </summary>
        /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
        /// <returns>True if an rotation was retrieved, false if not.</returns>
        bool TryGetPointerRotation(out Quaternion rotation);
    }
}
