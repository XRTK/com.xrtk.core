// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.BoundarySystem;

namespace XRTK.Interfaces.BoundarySystem
{
    /// <summary>
    /// The interface for a Boundary system in the Mixed Reality Toolkit
    /// All systems for providing Boundary functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityBoundarySystem : IMixedRealitySystem
    {
        /// <summary>
        /// Event raised when a tracked object nears, or crosses the boundary.
        /// </summary>
        event Action<GameObject, ProximityAlert> BoundaryProximityAlert;

        /// <summary>
        /// The list of currently tracked objects that will raise <see cref="BoundaryProximityAlert"/>s.
        /// </summary>
        IReadOnlyList<GameObject> TrackedObjects { get; }

        /// <summary>
        /// The <see cref="IMixedRealityBoundaryDataProvider"/> assigned to this system.
        /// </summary>
        /// <remarks>
        /// Typically with systems, there can be multiple data providers, but in this case there should only ever be one.
        /// </remarks>
        IMixedRealityBoundaryDataProvider BoundaryDataProvider { get; }

        /// <summary>
        /// Is the platform or the boundary system's rendered boundary geometry visible?
        /// </summary>
        /// <remarks>
        /// This can enables or disables both the platform's and boundary systems rendered geometry, but the platform can always override their rendering state.
        /// </remarks>
        bool IsVisible { get; set; }

        /// <summary>
        /// Has the boundary been properly configured by the user?
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Enable or disable the generated boundary geometry.
        /// </summary>
        /// <remarks>
        /// This cannot or ever will disable or override the platform's built-in boundary systems.
        /// </remarks>
        bool ShowBoundary { get; set; }

        /// <summary>
        /// The height of the play space, in meters.
        /// </summary>
        /// <remarks>
        /// This is used to create a three dimensional boundary volume.
        /// </remarks>
        float BoundaryHeight { get; set; }

        /// <summary>
        /// Enable or disable floor geometry.
        /// </summary>
        bool ShowFloor { get; set; }

        /// <summary>
        /// Enable or disable boundary wall geometry.
        /// </summary>
        bool ShowWalls { get; set; }

        /// <summary>
        /// Enable or disable boundary ceiling geometry.
        /// </summary>
        /// <remarks>
        /// The ceiling is defined as a <see cref="GameObject"/> positioned <see cref="BoundaryHeight"/> above the floor.
        /// </remarks>
        bool ShowCeiling { get; set; }

        /// <summary>
        /// Two dimensional representation of the geometry of the boundary, as provided by the platform.
        /// </summary>
        /// <remarks>
        /// BoundaryGeometry should be treated as the outline of the player's space, placed on the floor.
        /// </remarks>
        Edge[] BoundaryBounds { get; }

        /// <summary>
        /// Sets up the boundary using the <see cref="IMixedRealityBoundaryDataProvider"/> specified.
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <remarks>
        /// This method should usually be called from the <see cref="IMixedRealityService.Enable"/> method of the data provider itself.
        /// </remarks>
        void SetupBoundary(IMixedRealityBoundaryDataProvider dataProvider);

        /// <summary>
        /// Determines if a <see cref="position"/> is within the area of the boundary space.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> position to be checked.</param>
        /// <param name="referenceSpace">Is this position using local or worldspace coordinate space?</param>
        /// <returns>True if the location is within the area of the boundary space, otherwise false.</returns>
        bool IsInsideBoundary(Vector3 position, Space referenceSpace = Space.World);

        /// <summary>
        /// Returns the description of the inscribed rectangular bounds.
        /// </summary>
        /// <param name="center">The center of the rectangle.</param>
        /// <param name="angle">The orientation of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>True if an inscribed rectangle was found in the boundary geometry, false otherwise.</returns>
        bool TryGetRectangularBoundsParams(out Vector2 center, out float angle, out float width, out float height);

        /// <summary>
        /// Registers the <see cref="Transform"/> of a <see cref="GameObject"/> to be tracked for <see cref="BoundaryProximityAlert"/>s.
        /// </summary>
        void RegisterTrackedObject(GameObject gameObject);

        /// <summary>
        /// Unregisters the <see cref="Transform"/> of a <see cref="GameObject"/> from <see cref="BoundaryProximityAlert"/>s.
        /// </summary>
        void UnregisterTrackedObject(GameObject gameObject);
    }
}