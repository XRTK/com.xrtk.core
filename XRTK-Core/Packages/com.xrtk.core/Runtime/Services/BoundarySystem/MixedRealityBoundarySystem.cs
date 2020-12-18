// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using XRTK.Definitions.BoundarySystem;
using XRTK.Extensions;
using XRTK.Interfaces.BoundarySystem;
using XRTK.Utilities;

namespace XRTK.Services.BoundarySystem
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    [Guid("FE458876-CC0F-4B6F-9459-544DDF6A9263")]
    public class MixedRealityBoundarySystem : BaseSystem, IMixedRealityBoundarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityBoundarySystem(MixedRealityBoundaryProfile profile)
            : base(profile)
        {
            showBoundary = profile.ShowBoundary;
            BoundaryHeight = profile.BoundaryHeight;
            BoundaryMaterial = profile.BoundaryMaterial;
            boundaryPhysicsLayer = profile.PhysicsLayer;

            showFloor = profile.ShowFloor;
            FloorMaterial = profile.FloorMaterial.IsNull()
                ? profile.BoundaryMaterial
                : profile.FloorMaterial;

            showWalls = profile.ShowWalls;
            WallMaterial = profile.WallMaterial.IsNull()
                ? profile.BoundaryMaterial
                : profile.WallMaterial;

            showCeiling = profile.ShowCeiling;
            CeilingMaterial = profile.CeilingMaterial.IsNull()
                ? profile.BoundaryMaterial
                : profile.CeilingMaterial;
        }

        /// <summary>
        /// Layer used to tell the (non-floor) boundary objects to not accept raycasts
        /// </summary>
        private const int DEFAULT_IGNORE_RAYCAST_LAYER = 2;

        /// <summary>
        /// The thickness of three dimensional generated boundary objects.
        /// </summary>
        private const float BOUNDARY_OBJECT_THICKNESS = 0.005f;

        /// <summary>
        /// A small offset to avoid render conflicts, primarily with the floor.
        /// </summary>
        /// <remarks>
        /// This offset is used to avoid consuming multiple physics layers.
        /// </remarks>
        private const float BOUNDARY_OBJECT_RENDER_OFFSET = 0.001f;

        private readonly Dictionary<BoundsCache, ProximityAlert> trackedObjects = new Dictionary<BoundsCache, ProximityAlert>(3);

        private InscribedRectangle rectangularBounds;

        private GameObject boundaryVisualizationRoot = null;

        private GameObject BoundarySystemVisualizationRoot
        {
            get
            {
                if (!boundaryVisualizationRoot.IsNull())
                {
                    return boundaryVisualizationRoot;
                }

                boundaryVisualizationRoot = new GameObject(nameof(BoundarySystemVisualizationRoot));
                var playspaceTransform = MixedRealityToolkit.CameraSystem != null
                    ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform
                    : CameraCache.Main.transform.parent;
                boundaryVisualizationRoot.transform.SetParent(playspaceTransform, false);

                return boundaryVisualizationRoot;
            }
        }

        private GameObject boundaryVisualization = null;

        private GameObject BoundaryVisualization
        {
            get
            {
                if (!boundaryVisualization.IsNull())
                {
                    return boundaryVisualization;
                }

                // If we do not have boundary edges, we cannot render them.
                if (BoundaryBounds.Length == 0)
                {
                    return null;
                }

                // Get the rectangular bounds.
                if (!TryGetRectangularBoundsParams(out var center, out var angle, out var width, out var height))
                {
                    // No rectangular bounds, therefore cannot create the play area.
                    return null;
                }

                // Render the rectangular bounds.
                if (!EdgeUtilities.IsValidPoint(center))
                {
                    // Invalid rectangle / play area not found
                    return null;
                }

                boundaryVisualization = GameObject.CreatePrimitive(PrimitiveType.Quad);
                boundaryVisualization.name = nameof(BoundaryVisualization);
                boundaryVisualization.layer = boundaryPhysicsLayer;
                boundaryVisualization.transform.Translate(new Vector3(center.x, BOUNDARY_OBJECT_RENDER_OFFSET, center.y));
                boundaryVisualization.transform.Rotate(new Vector3(90f, -angle, 0.0f));
                boundaryVisualization.transform.localScale = new Vector3(width, height, 1.0f);
                boundaryVisualization.GetComponent<Renderer>().sharedMaterial = BoundaryMaterial;
                boundaryVisualization.transform.SetParent(BoundarySystemVisualizationRoot.transform, false);

                boundaryVisualization.AddComponent<LineRenderer>();

                // Get the line vertices
                var lineVertices = new List<Vector3>();

                for (int i = 0; i < BoundaryBounds.Length; i++)
                {
                    lineVertices.Add(new Vector3(BoundaryBounds[i].PointA.x, 0f, BoundaryBounds[i].PointA.y));
                }

                // Add the first vertex again to ensure the loop closes.
                lineVertices.Add(lineVertices[0]);

                // Configure the renderer properties.
                const float lineWidth = 0.01f;
                var lineRenderer = boundaryVisualization.GetComponent<LineRenderer>();
                lineRenderer.sharedMaterial = BoundaryMaterial;
                lineRenderer.useWorldSpace = false;
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.positionCount = lineVertices.Count;
                lineRenderer.SetPositions(lineVertices.ToArray());

                boundaryVisualization.SetActive(showBoundary);

                return boundaryVisualization;
            }
        }

        private GameObject floorVisualization = null;

        private GameObject FloorVisualization
        {
            get
            {
                if (!floorVisualization.IsNull())
                {
                    return floorVisualization;
                }

                var position = MixedRealityToolkit.CameraSystem != null
                    ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform.position
                    : CameraCache.Main.transform.parent.position;

                // Render the floor.
                floorVisualization = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floorVisualization.name = nameof(FloorVisualization);
                floorVisualization.transform.Translate(new Vector3(
                    position.x,
                    position.y - (floorVisualization.transform.localScale.y * 0.5f),
                    position.z));
                floorVisualization.layer = boundaryPhysicsLayer;
                floorVisualization.GetComponent<Renderer>().sharedMaterial = FloorMaterial;
                floorVisualization.transform.SetParent(BoundarySystemVisualizationRoot.transform, false);
                floorVisualization.SetActive(showFloor);

                return floorVisualization;
            }
        }

        private GameObject wallVisualization = null;

        private GameObject WallVisualization
        {
            get
            {
                if (!wallVisualization.IsNull())
                {
                    return wallVisualization;
                }

                if (BoundaryBounds.Length == 0)
                {
                    // If we do not have boundary edges, we cannot render walls.
                    return null;
                }

                wallVisualization = new GameObject(nameof(WallVisualization))
                {
                    layer = boundaryPhysicsLayer
                };

                for (int i = 0; i < BoundaryBounds.Length; i++)
                {
                    var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.name = $"Wall_{i}";
                    wall.GetComponent<Renderer>().sharedMaterial = WallMaterial;
                    wall.transform.localScale = new Vector3((BoundaryBounds[i].PointB - BoundaryBounds[i].PointA).magnitude, BoundaryHeight, BOUNDARY_OBJECT_THICKNESS);
                    wall.layer = boundaryPhysicsLayer;

                    // Position and rotate the wall.
                    var midpoint = Vector2.Lerp(BoundaryBounds[i].PointA, BoundaryBounds[i].PointB, 0.5f);
                    wall.transform.SetParent(wallVisualization.transform, false);
                    wall.transform.position = new Vector3(midpoint.x, (BoundaryHeight * 0.5f), midpoint.y);
                    var rotationAngle = MathUtilities.GetAngleBetween(BoundaryBounds[i].PointB, BoundaryBounds[i].PointA);
                    wall.transform.rotation = Quaternion.Euler(0.0f, -rotationAngle, 0.0f);
                }

                wallVisualization.transform.SetParent(BoundarySystemVisualizationRoot.transform, false);

                return wallVisualization;
            }
        }

        private GameObject ceilingVisualization = null;

        private GameObject CeilingVisualization
        {
            get
            {
                if (!ceilingVisualization.IsNull())
                {
                    return ceilingVisualization;
                }

                if (BoundaryBounds.Length == 0)
                {
                    // If we do not have boundary edges, we cannot render a ceiling.
                    return null;
                }

                // Get the smallest rectangle that contains the entire boundary.
                var boundaryBoundingBox = new Bounds();

                for (int i = 0; i < BoundaryBounds.Length; i++)
                {
                    // The boundary geometry is a closed loop. As such, we can encapsulate only PointA of each Edge.
                    boundaryBoundingBox.Encapsulate(new Vector3(BoundaryBounds[i].PointA.x, BoundaryHeight * 0.5f, BoundaryBounds[i].PointA.y));
                }

                ceilingVisualization = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ceilingVisualization.name = nameof(CeilingVisualization);
                ceilingVisualization.layer = DEFAULT_IGNORE_RAYCAST_LAYER;
                ceilingVisualization.transform.localScale = new Vector3(boundaryBoundingBox.size.x, BOUNDARY_OBJECT_THICKNESS, boundaryBoundingBox.size.z);
                ceilingVisualization.transform.Translate(new Vector3(
                    boundaryBoundingBox.center.x,
                    BoundaryHeight + (ceilingVisualization.transform.localScale.y * 0.5f),
                    boundaryBoundingBox.center.z));
                ceilingVisualization.GetComponent<Renderer>().sharedMaterial = CeilingMaterial;
                ceilingVisualization.layer = boundaryPhysicsLayer;
                ceilingVisualization.transform.SetParent(BoundarySystemVisualizationRoot.transform, false);

                return ceilingVisualization;
            }
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying || boundaryDataProvider == null) { return; }

            foreach (var trackedObjectStatus in trackedObjects)
            {
                var trackedBounds = trackedObjectStatus.Key;
                var status = trackedObjectStatus.Value;
                var isAnyCornerInsideBoundary = false;
                var isCornersFullyInsideBoundary = true;

                for (int i = 0; i < trackedBounds.BoundsCornerPoints.Length; i++)
                {
                    var result = IsInsideBoundary(trackedBounds.BoundsCornerPoints[i]);
                    isAnyCornerInsideBoundary |= result;
                    isCornersFullyInsideBoundary &= result;
                }

                switch (status)
                {
                    case ProximityAlert.Clear:
                        if (isAnyCornerInsideBoundary && !isCornersFullyInsideBoundary)
                        {
                            status = ProximityAlert.Touch;
                        }

                        // In case object moves faster than updates can track it's ejection or
                        // if the object was already outside of the boundary.
                        if (!isCornersFullyInsideBoundary)
                        {
                            status = ProximityAlert.Exit;
                        }

                        break;
                    case ProximityAlert.Touch:
                        if (isCornersFullyInsideBoundary)
                        {
                            status = ProximityAlert.Clear;
                        }

                        if (!isAnyCornerInsideBoundary)
                        {
                            status = ProximityAlert.Exit;
                        }

                        break;
                    case ProximityAlert.Exit:
                        // Left and re-entered the boundary
                        if (isAnyCornerInsideBoundary)
                        {
                            status = ProximityAlert.Enter;
                        }

                        break;
                    case ProximityAlert.Enter:
                        if (isAnyCornerInsideBoundary)
                        {
                            status = isCornersFullyInsideBoundary
                                ? ProximityAlert.Clear
                                : ProximityAlert.Touch;
                        }
                        else
                        {
                            status = ProximityAlert.Exit;
                        }

                        break;
                }

                if (status != trackedObjectStatus.Value)
                {
                    trackedObjects[trackedBounds] = status;
                    BoundaryProximityAlert?.Invoke(trackedBounds.gameObject, status);
                }
            }
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying) { return; }

            if (!boundaryVisualizationRoot.IsNull())
            {
                boundaryVisualizationRoot.SetActive(false);
            }
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            base.Destroy();

            if (!Application.isPlaying) { return; }

            // Destroys the parent and all the child objects
            boundaryVisualizationRoot.Destroy();
        }

        #endregion IMixedRealityService Implementation

        #region Implementation of IMixedRealityBoundarySystem

        /// <inheritdoc />
        public event Action<GameObject, ProximityAlert> BoundaryProximityAlert;

        /// <inheritdoc />
        public IReadOnlyList<GameObject> TrackedObjects => trackedObjects.Keys.Select(cache => cache.gameObject).ToList();

        private IMixedRealityBoundaryDataProvider boundaryDataProvider = null;

        /// <inheritdoc />
        public IMixedRealityBoundaryDataProvider BoundaryDataProvider
        {
            get => boundaryDataProvider ?? (boundaryDataProvider = MixedRealityToolkit.GetService<IMixedRealityBoundaryDataProvider>());
            private set => boundaryDataProvider = value;
        }

        /// <inheritdoc />
        public bool IsVisible
        {
            get => (BoundaryDataProvider != null && BoundaryDataProvider.IsPlatformBoundaryVisible) ||
                   !BoundarySystemVisualizationRoot.IsNull() && BoundarySystemVisualizationRoot.activeInHierarchy &&
                   (ShowBoundary ||
                    ShowFloor ||
                    ShowWalls ||
                    ShowCeiling);
            set
            {
                if (!BoundarySystemVisualizationRoot.IsNull())
                {
                    BoundarySystemVisualizationRoot.SetActive(value);
                }
            }
        }

        private bool showBoundary;

        /// <inheritdoc />
        public bool ShowBoundary
        {
            get => showBoundary;
            set
            {
                showBoundary = value;

                if (!BoundaryVisualization.IsNull())
                {
                    BoundaryVisualization.SetActive(value);
                }
            }
        }

        /// <inheritdoc />
        public bool IsConfigured => BoundaryDataProvider?.IsPlatformConfigured ?? false;

        /// <inheritdoc />
        public float BoundaryHeight { get; set; }

        private readonly int boundaryPhysicsLayer;

        private bool showFloor;

        /// <inheritdoc />
        public bool ShowFloor
        {
            get => showFloor;
            set
            {
                showFloor = value;

                if (!FloorVisualization.IsNull())
                {
                    FloorVisualization.SetActive(value);
                }
            }
        }

        private Material FloorMaterial { get; }

        private Material BoundaryMaterial { get; }

        private bool showWalls = false;

        /// <inheritdoc />
        public bool ShowWalls
        {
            get => showWalls;
            set
            {
                showWalls = value;

                if (!WallVisualization.IsNull())
                {
                    WallVisualization.SetActive(value);
                }
            }
        }

        private Material WallMaterial { get; }

        private bool showCeiling = false;

        /// <inheritdoc />
        public bool ShowCeiling
        {
            get => showCeiling;
            set
            {
                showCeiling = value;

                if (!CeilingVisualization.IsNull())
                {
                    CeilingVisualization.SetActive(value);
                }
            }
        }

        private Material CeilingMaterial { get; }

        /// <inheritdoc />
        public Edge[] BoundaryBounds { get; private set; } = new Edge[0];

        /// <inheritdoc />
        public void SetupBoundary(IMixedRealityBoundaryDataProvider dataProvider)
        {
            BoundaryDataProvider = dataProvider;

            if (!BoundaryDataProvider.IsPlatformConfigured) { return; }

            // Reset the bounds
            BoundaryBounds = new Edge[0];
            rectangularBounds = null;

            // Get the boundary geometry.
            var boundaryGeometry = new List<Vector3>(0);
            var boundaryEdges = new List<Edge>(0);

            if (BoundaryDataProvider.TryGetBoundaryGeometry(ref boundaryGeometry) && boundaryGeometry.Count > 0)
            {
                for (int i = 0; i < boundaryGeometry.Count; i++)
                {
                    var pointA = boundaryGeometry[i];
                    var pointB = boundaryGeometry[(i + 1) % boundaryGeometry.Count];
                    boundaryEdges.Add(new Edge(pointA, pointB));
                }

                BoundaryBounds = boundaryEdges.ToArray();
                // We always use the same seed so that from run to run, the inscribed bounds are consistent.
                rectangularBounds = new InscribedRectangle(BoundaryBounds, Mathf.Abs("Mixed Reality Toolkit".GetHashCode()));
            }
            else
            {
                Debug.LogWarning("No Boundary Geometry found");
            }

            // Clear the prev visualization objects.
            if (!BoundarySystemVisualizationRoot.IsNull())
            {
                boundaryVisualizationRoot.Destroy();
            }

            // Initialize the visualization objects.
            ShowBoundary = showBoundary;
            ShowCeiling = showCeiling;
            ShowFloor = showFloor;
            ShowWalls = showWalls;
            IsVisible = ShowBoundary || ShowCeiling || ShowFloor || ShowWalls;
        }

        /// <inheritdoc />
        public bool IsInsideBoundary(Vector3 position, Space referenceSpace = Space.World)
        {
            if (!EdgeUtilities.IsValidPoint(position))
            {
                // Invalid location.
                return false;
            }

            // Handle the user teleporting (boundary moves with them).
            var playspaceTransform = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform
                : CameraCache.Main.transform.parent;

            if (referenceSpace == Space.World)
            {
                position = playspaceTransform.InverseTransformPoint(position);
            }

            if (position.y < 0 ||
                position.y > BoundaryHeight)
            {
                // Location below the floor or above the boundary height.
                return false;
            }

            // Boundary coordinates are always "on the floor"
            var point = new Vector2(position.x, position.z);
            return EdgeUtilities.IsInsideBoundary(BoundaryBounds, point);
        }

        /// <inheritdoc />
        public bool TryGetRectangularBoundsParams(out Vector2 center, out float angle, out float width, out float height)
        {
            if (rectangularBounds == null || !rectangularBounds.IsValid)
            {
                center = EdgeUtilities.InvalidPoint;
                angle = 0f;
                width = 0f;
                height = 0f;
                return false;
            }

            // Handle the user teleporting (boundary moves with them).
            var playspaceTransform = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform
                : CameraCache.Main.transform.parent;
            var transformedCenter = playspaceTransform.TransformPoint(
                new Vector3(rectangularBounds.Center.x, 0f, rectangularBounds.Center.y));

            center = new Vector2(transformedCenter.x, transformedCenter.z);
            angle = rectangularBounds.Angle;
            width = rectangularBounds.Width;
            height = rectangularBounds.Height;
            return true;
        }

        /// <inheritdoc />
        public void RegisterTrackedObject(GameObject gameObject)
        {
            var boundsCached = gameObject.EnsureComponent<BoundsCache>();

            if (!trackedObjects.TryGetValue(boundsCached, out _))
            {
                trackedObjects.Add(boundsCached, ProximityAlert.Clear);
            }
        }

        /// <inheritdoc />
        public void UnregisterTrackedObject(GameObject gameObject)
        {
            var boundsCache = gameObject.GetComponent<BoundsCache>();
            if (boundsCache == null) { return; }

            if (trackedObjects.TryGetValue(boundsCache, out _))
            {
                trackedObjects.Remove(boundsCache);
            }
        }

        #endregion Implementation of IMixedRealityBoundarySystem
    }
}
