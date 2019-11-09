// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
using XRTK.Definitions.BoundarySystem;
using XRTK.EventDatum.Boundary;
using XRTK.Interfaces.BoundarySystem;
using XRTK.Utilities;

namespace XRTK.Services.BoundarySystem
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    public class MixedRealityBoundarySystem : BaseEventSystem, IMixedRealityBoundarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityBoundarySystem(MixedRealityBoundaryVisualizationProfile profile)
            : base(profile)
        {
            showFloor = profile.ShowFloor;
            floorPhysicsLayer = profile.FloorPhysicsLayer;
            FloorScale = profile.FloorScale;
            FloorMaterial = profile.FloorMaterial;
            showPlayArea = profile.ShowPlayArea;
            PlayAreaMaterial = profile.PlayAreaMaterial;
            playAreaPhysicsLayer = profile.PlayAreaPhysicsLayer;
            BoundaryHeight = profile.BoundaryHeight;
            showTrackedArea = profile.ShowTrackedArea;
            TrackedAreaMaterial = profile.TrackedAreaMaterial;
            trackedAreaPhysicsLayer = profile.TrackedAreaPhysicsLayer;
            showBoundaryWalls = profile.ShowBoundaryWalls;
            BoundaryWallMaterial = profile.BoundaryWallMaterial;
            boundaryWallsPhysicsLayer = profile.BoundaryWallsPhysicsLayer;
            showCeiling = profile.ShowBoundaryCeiling;
            BoundaryCeilingMaterial = profile.BoundaryCeilingMaterial;
            ceilingPhysicsLayer = profile.CeilingPhysicsLayer;
        }

        #region IMixedRealityService Implementation

        private BoundaryEventData boundaryEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying) { return; }

            boundaryEventData = new BoundaryEventData(EventSystem.current);

            CalculateBoundaryBounds();
            Boundary.visible = true;

            if (ShowFloor)
            {
                GetFloorVisualization();
            }

            if (ShowPlayArea)
            {
                GetPlayAreaVisualization();
            }

            if (ShowTrackedArea)
            {
                GetTrackedAreaVisualization();
            }

            if (ShowBoundaryWalls)
            {
                GetBoundaryWallVisualization();
            }

            if (ShowBoundaryWalls)
            {
                GetBoundaryCeilingVisualization();
            }

            RaiseBoundaryVisualizationChanged();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            // First, detach the child objects (we are tracking them separately)
            // and clean up the parent.
            if (boundaryVisualizationParent != null)
            {

                if (Application.isEditor)
                {
                    Object.DestroyImmediate(boundaryVisualizationParent);
                }
                else
                {
                    boundaryVisualizationParent.transform.DetachChildren();
                    Object.Destroy(boundaryVisualizationParent);
                }

                boundaryVisualizationParent = null;
            }

            // Next, clean up the detached children.
            if (currentFloorObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentFloorObject);
                }
                else
                {
                    Object.Destroy(currentFloorObject);
                }

                currentFloorObject = null;
            }

            if (currentPlayAreaObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentPlayAreaObject);
                }
                else
                {
                    Object.Destroy(currentPlayAreaObject);
                }

                currentPlayAreaObject = null;
            }

            if (currentTrackedAreaObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentTrackedAreaObject);
                }
                else
                {
                    Object.Destroy(currentTrackedAreaObject);
                }

                currentTrackedAreaObject = null;
            }

            if (currentBoundaryWallObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentBoundaryWallObject);
                }
                else
                {
                    Object.Destroy(currentBoundaryWallObject);
                }

                currentBoundaryWallObject = null;
            }

            if (currentCeilingObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentCeilingObject);
                }
                else
                {
                    Object.Destroy(currentCeilingObject);
                }

                currentCeilingObject = null;
            }

            showFloor = false;
            showPlayArea = false;
            showTrackedArea = false;
            showBoundaryWalls = false;
            showCeiling = false;

            RaiseBoundaryVisualizationChanged();
        }

        /// <summary>
        /// Raises an event to indicate that the visualization of the boundary has been changed by the boundary system.
        /// </summary>
        private void RaiseBoundaryVisualizationChanged()
        {
            if (!Application.isPlaying) { return; }
            boundaryEventData.Initialize(this, ShowFloor, ShowPlayArea, ShowTrackedArea, ShowBoundaryWalls, ShowBoundaryCeiling);
            HandleEvent(boundaryEventData, OnVisualizationChanged);
        }

        /// <summary>
        /// Event sent whenever the boundary visualization changes.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealityBoundaryHandler> OnVisualizationChanged =
            delegate (IMixedRealityBoundaryHandler handler, BaseEventData eventData)
        {
            var boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
            handler.OnBoundaryVisualizationChanged(boundaryEventData);
        };

        #endregion IMixedRealityService Implementation

        #region IMixedRealtyEventSystem Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            base.HandleEvent(eventData, eventHandler);
        }

        /// <summary>
        /// Registers the <see cref="GameObject"/> to listen for boundary events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// UnRegisters the <see cref="GameObject"/> to listen for boundary events.
        /// /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Boundary System";

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealityBoundarySystem Implementation

        /// <summary>
        /// Layer used to tell the (non-floor) boundary objects to not accept raycasts
        /// </summary>
        private const int DefaultIgnoreRaycastLayer = 2;

        /// <summary>
        /// The thickness of three dimensional generated boundary objects.
        /// </summary>
        private const float BoundaryObjectThickness = 0.005f;

        /// <summary>
        /// A small offset to avoid render conflicts, primarily with the floor.
        /// </summary>
        /// <remarks>
        /// This offset is used to avoid consuming multiple physics layers.
        /// </remarks>
        private const float BoundaryObjectRenderOffset = 0.001f;

        private GameObject boundaryVisualizationParent;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the teleportable boundary visualizations.
        /// </summary>
        private GameObject BoundaryVisualizationParent
        {
            get
            {
                if (boundaryVisualizationParent != null)
                {
                    return boundaryVisualizationParent;
                }

                var visualizationParent = new GameObject("Boundary System Visualizations");
                visualizationParent.transform.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;
                return boundaryVisualizationParent = visualizationParent;
            }
        }

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; }

        private bool showFloor;

        /// <inheritdoc/>
        public bool ShowFloor
        {
            get => showFloor;
            set
            {
                if (showFloor == value) { return; }

                showFloor = value;

                if (value && (currentFloorObject == null))
                {
                    GetFloorVisualization();
                }

                if (currentFloorObject != null)
                {
                    currentFloorObject.SetActive(value);
                }

                RaiseBoundaryVisualizationChanged();
            }
        }

        /// <inheritdoc/>
        public Vector2 FloorScale { get; }

        /// <inheritdoc/>
        public Material FloorMaterial { get; }

        private bool showPlayArea;

        private int floorPhysicsLayer;

        /// <inheritdoc/>
        public int FloorPhysicsLayer
        {
            get
            {
                if (currentFloorObject != null)
                {
                    floorPhysicsLayer = currentFloorObject.layer;
                }

                return floorPhysicsLayer;
            }
            set
            {
                floorPhysicsLayer = value;
                if (currentFloorObject != null)
                {
                    currentFloorObject.layer = floorPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public bool ShowPlayArea
        {
            get => showPlayArea;
            set
            {
                if (showPlayArea == value) { return; }

                showPlayArea = value;

                if (value && (currentPlayAreaObject == null))
                {
                    GetPlayAreaVisualization();
                }

                if (currentPlayAreaObject != null)
                {
                    currentPlayAreaObject.SetActive(value);
                }

                RaiseBoundaryVisualizationChanged();
            }
        }

        private bool showTrackedArea;

        /// <inheritdoc/>
        public int PlayAreaPhysicsLayer
        {
            get
            {
                if (currentPlayAreaObject != null)
                {
                    playAreaPhysicsLayer = currentPlayAreaObject.layer;
                }

                return playAreaPhysicsLayer;
            }
            set
            {
                playAreaPhysicsLayer = value;

                if (currentPlayAreaObject != null)
                {
                    currentPlayAreaObject.layer = playAreaPhysicsLayer;
                }
            }
        }

        private int playAreaPhysicsLayer;

        /// <inheritdoc/>
        public Material PlayAreaMaterial { get; }

        /// <inheritdoc/>
        public bool ShowTrackedArea
        {
            get => showTrackedArea;
            set
            {
                if (showTrackedArea == value) { return; }
                showTrackedArea = value;

                if (value && (currentTrackedAreaObject == null))
                {
                    GetTrackedAreaVisualization();
                }

                if (currentTrackedAreaObject != null)
                {
                    currentTrackedAreaObject.SetActive(value);
                }

                RaiseBoundaryVisualizationChanged();
            }
        }

        private bool showBoundaryWalls = false;

        private int trackedAreaPhysicsLayer;

        /// <inheritdoc/>
        public int TrackedAreaPhysicsLayer
        {
            get
            {
                if (currentTrackedAreaObject != null)
                {
                    trackedAreaPhysicsLayer = currentTrackedAreaObject.layer;
                }

                return trackedAreaPhysicsLayer;
            }
            set
            {
                trackedAreaPhysicsLayer = value;

                if (currentTrackedAreaObject != null)
                {
                    currentTrackedAreaObject.layer = trackedAreaPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public Material TrackedAreaMaterial { get; }

        /// <inheritdoc/>
        public bool ShowBoundaryWalls
        {
            get => showBoundaryWalls;
            set
            {
                if (showBoundaryWalls == value) { return; }

                showBoundaryWalls = value;

                if (value && (currentBoundaryWallObject == null))
                {
                    GetBoundaryWallVisualization();
                }

                if (currentBoundaryWallObject != null)
                {
                    currentBoundaryWallObject.SetActive(value);
                }

                RaiseBoundaryVisualizationChanged();
            }
        }

        private bool showCeiling = false;

        private int boundaryWallsPhysicsLayer;

        /// <inheritdoc/>
        public int BoundaryWallsPhysicsLayer
        {
            get
            {
                if (currentBoundaryWallObject != null)
                {
                    boundaryWallsPhysicsLayer = currentBoundaryWallObject.layer;
                }

                return boundaryWallsPhysicsLayer;
            }
            set
            {
                boundaryWallsPhysicsLayer = value;

                if (currentBoundaryWallObject != null)
                {
                    currentBoundaryWallObject.layer = boundaryWallsPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public Material BoundaryWallMaterial { get; }

        /// <inheritdoc/>
        public bool ShowBoundaryCeiling
        {
            get => showCeiling;
            set
            {
                if (showCeiling == value) { return; }

                showCeiling = value;

                if (value && (currentCeilingObject == null))
                {
                    GetBoundaryCeilingVisualization();
                }

                if (currentCeilingObject != null)
                {
                    currentCeilingObject.SetActive(value);
                }

                RaiseBoundaryVisualizationChanged();
            }
        }

        private int ceilingPhysicsLayer;

        /// <inheritdoc/>
        public int CeilingPhysicsLayer
        {
            get
            {
                if (currentCeilingObject != null)
                {
                    ceilingPhysicsLayer = currentCeilingObject.layer;
                }

                return ceilingPhysicsLayer;
            }
            set
            {
                ceilingPhysicsLayer = value;

                if (currentCeilingObject != null)
                {
                    currentFloorObject.layer = ceilingPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public Material BoundaryCeilingMaterial { get; }

        /// <inheritdoc/>
        public Edge[] Bounds { get; private set; } = new Edge[0];

        /// <inheritdoc/>
        public float? FloorHeight { get; private set; }

        /// <inheritdoc/>
        public bool Contains(Vector3 location, Boundary.Type boundaryType = Boundary.Type.TrackedArea)
        {
            if (!EdgeUtilities.IsValidPoint(location))
            {
                // Invalid location.
                return false;
            }

            if (!FloorHeight.HasValue)
            {
                // No floor.
                return false;
            }

            // Handle the user teleporting (boundary moves with them).
            location = MixedRealityToolkit.CameraSystem.CameraRig.PlayspaceTransform.InverseTransformPoint(location);

            if (FloorHeight.Value > location.y ||
                BoundaryHeight < location.y)
            {
                // Location below the floor or above the boundary height.
                return false;
            }

            // Boundary coordinates are always "on the floor"
            var point = new Vector2(location.x, location.z);

            switch (boundaryType)
            {
                case Boundary.Type.PlayArea when rectangularBounds != null:
                    // Check the inscribed rectangle.
                    return rectangularBounds.IsInsideBoundary(point);
                case Boundary.Type.TrackedArea:
                    // Check the geometry
                    return EdgeUtilities.IsInsideBoundary(Bounds, point);
                default:
                    // Not in either boundary type.
                    return false;
            }
        }

        /// <inheritdoc/>
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
            var transformedCenter = MixedRealityToolkit.CameraSystem.CameraRig.PlayspaceTransform.TransformPoint(
                new Vector3(rectangularBounds.Center.x, 0f, rectangularBounds.Center.y));

            center = new Vector2(transformedCenter.x, transformedCenter.z);
            angle = rectangularBounds.Angle;
            width = rectangularBounds.Width;
            height = rectangularBounds.Height;
            return true;
        }

        /// <inheritdoc/>
        public GameObject GetFloorVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentFloorObject != null)
            {
                return currentFloorObject;
            }

            if (!FloorHeight.HasValue)
            {
                // We were unable to locate the floor.
                return null;
            }

            var floorScale = FloorScale;
            var position = MixedRealityToolkit.CameraSystem.CameraRig.PlayspaceTransform.position;

            // Render the floor.
            currentFloorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            currentFloorObject.name = "Boundary System Floor";
            currentFloorObject.transform.localScale = new Vector3(floorScale.x, BoundaryObjectThickness, floorScale.y);
            currentFloorObject.transform.Translate(new Vector3(
                position.x,
                FloorHeight.Value - (currentFloorObject.transform.localScale.y * 0.5f),
                position.z));
            currentFloorObject.layer = FloorPhysicsLayer;
            currentFloorObject.GetComponent<Renderer>().sharedMaterial = FloorMaterial;

            return currentFloorObject;
        }

        /// <inheritdoc/>
        public GameObject GetPlayAreaVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentPlayAreaObject != null)
            {
                return currentPlayAreaObject;
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

            currentPlayAreaObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            currentPlayAreaObject.name = "Play Area";
            currentPlayAreaObject.layer = PlayAreaPhysicsLayer;
            currentPlayAreaObject.transform.Translate(new Vector3(center.x, BoundaryObjectRenderOffset, center.y));
            currentPlayAreaObject.transform.Rotate(new Vector3(90f, -angle, 0.0f));
            currentPlayAreaObject.transform.localScale = new Vector3(width, height, 1.0f);
            currentPlayAreaObject.GetComponent<Renderer>().sharedMaterial = PlayAreaMaterial;

            currentPlayAreaObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentPlayAreaObject;
        }

        /// <inheritdoc/>
        public GameObject GetTrackedAreaVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentTrackedAreaObject != null)
            {
                return currentTrackedAreaObject;
            }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render them.
                return null;
            }

            // Get the line vertices
            var lineVertices = new List<Vector3>();

            for (int i = 0; i < Bounds.Length; i++)
            {
                lineVertices.Add(new Vector3(Bounds[i].PointA.x, 0f, Bounds[i].PointA.y));
            }

            // Add the first vertex again to ensure the loop closes.
            lineVertices.Add(lineVertices[0]);

            // We use an empty object and attach a line renderer.
            currentTrackedAreaObject = new GameObject("Tracked Area")
            {
                layer = DefaultIgnoreRaycastLayer
            };

            var position = MixedRealityToolkit.CameraSystem.CameraRig.PlayspaceTransform.position;

            currentTrackedAreaObject.AddComponent<LineRenderer>();
            currentTrackedAreaObject.transform.Translate(
                new Vector3(position.x, BoundaryObjectRenderOffset, position.z));
            currentPlayAreaObject.layer = TrackedAreaPhysicsLayer;

            // Configure the renderer properties.
            const float lineWidth = 0.01f;
            var lineRenderer = currentTrackedAreaObject.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial = TrackedAreaMaterial;
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = lineVertices.Count;
            lineRenderer.SetPositions(lineVertices.ToArray());

            currentTrackedAreaObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentTrackedAreaObject;
        }

        /// <inheritdoc/>
        public GameObject GetBoundaryWallVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentBoundaryWallObject != null)
            {
                return currentBoundaryWallObject;
            }

            if (!FloorHeight.HasValue)
            {
                // We need a floor on which to place the walls.
                return null;
            }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render walls.
                return null;
            }

            currentBoundaryWallObject = new GameObject("Tracked Area Walls")
            {
                layer = BoundaryWallsPhysicsLayer
            };

            // Create and parent the child objects
            const float wallDepth = BoundaryObjectThickness;

            for (int i = 0; i < Bounds.Length; i++)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i}";
                wall.GetComponent<Renderer>().sharedMaterial = BoundaryWallMaterial;
                wall.transform.localScale = new Vector3((Bounds[i].PointB - Bounds[i].PointA).magnitude, BoundaryHeight, wallDepth);
                wall.layer = DefaultIgnoreRaycastLayer;

                // Position and rotate the wall.
                var mid = Vector2.Lerp(Bounds[i].PointA, Bounds[i].PointB, 0.5f);
                wall.transform.position = new Vector3(mid.x, (BoundaryHeight * 0.5f), mid.y);
                var rotationAngle = MathUtilities.GetAngleBetween(Bounds[i].PointB, Bounds[i].PointA);
                wall.transform.rotation = Quaternion.Euler(0.0f, -rotationAngle, 0.0f);

                wall.transform.parent = currentBoundaryWallObject.transform;
            }

            currentBoundaryWallObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentBoundaryWallObject;
        }

        /// <inheritdoc/>
        public GameObject GetBoundaryCeilingVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentCeilingObject != null)
            {
                return currentCeilingObject;
            }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render a ceiling.
                return null;
            }

            // Get the smallest rectangle that contains the entire boundary.
            var boundaryBoundingBox = new Bounds();

            for (int i = 0; i < Bounds.Length; i++)
            {
                // The boundary geometry is a closed loop. As such, we can encapsulate only PointA of each Edge.
                boundaryBoundingBox.Encapsulate(new Vector3(Bounds[i].PointA.x, BoundaryHeight * 0.5f, Bounds[i].PointA.y));
            }

            // Render the ceiling.
            const float ceilingDepth = BoundaryObjectThickness;
            currentCeilingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            currentCeilingObject.name = "Ceiling";
            currentCeilingObject.layer = DefaultIgnoreRaycastLayer;
            currentCeilingObject.transform.localScale = new Vector3(boundaryBoundingBox.size.x, ceilingDepth, boundaryBoundingBox.size.z);
            currentCeilingObject.transform.Translate(new Vector3(
                boundaryBoundingBox.center.x,
                BoundaryHeight + (currentCeilingObject.transform.localScale.y * 0.5f),
                boundaryBoundingBox.center.z));
            currentCeilingObject.GetComponent<Renderer>().sharedMaterial = BoundaryCeilingMaterial;
            currentCeilingObject.layer = CeilingPhysicsLayer;
            currentCeilingObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentCeilingObject;
        }

        #endregion IMixedRealityBoundarySystem Implementation

        /// <summary>
        /// The largest rectangle that is contained withing the play space geometry.
        /// </summary>
        private InscribedRectangle rectangularBounds;

        private GameObject currentFloorObject;
        private GameObject currentPlayAreaObject;
        private GameObject currentTrackedAreaObject;
        private GameObject currentBoundaryWallObject;
        private GameObject currentCeilingObject;

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed play space volumes.
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            // Reset the bounds
            Bounds = new Edge[0];
            FloorHeight = null;
            rectangularBounds = null;

            // Boundaries are supported for Room Scale experiences only.
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                return;
            }

            // Get the boundary geometry.
            var boundaryGeometry = new List<Vector3>(0);
            var boundaryEdges = new List<Edge>(0);

            if (Boundary.TryGetGeometry(boundaryGeometry, Boundary.Type.TrackedArea) && boundaryGeometry.Count > 0)
            {
                // FloorHeight starts out as null. Use a suitably high value for the floor to ensure
                // that we do not accidentally set it too low.
                var floorHeight = float.MaxValue;

                for (int i = 0; i < boundaryGeometry.Count; i++)
                {
                    var pointA = boundaryGeometry[i];
                    var pointB = boundaryGeometry[(i + 1) % boundaryGeometry.Count];
                    boundaryEdges.Add(new Edge(pointA, pointB));

                    floorHeight = Mathf.Min(floorHeight, boundaryGeometry[i].y);
                }

                FloorHeight = floorHeight;
                Bounds = boundaryEdges.ToArray();
                CreateInscribedBounds();
            }
            else if (XRSettings.enabled && Boundary.configured)
            {
                Debug.LogWarning("Failed to calculate boundary bounds.");
            }
        }

        /// <summary>
        /// Creates the two dimensional volume described by the largest rectangle that
        /// is contained withing the play space geometry and the configured height.
        /// </summary>
        private void CreateInscribedBounds()
        {
            // We always use the same seed so that from run to run, the inscribed bounds are consistent.
            rectangularBounds = new InscribedRectangle(Bounds, Mathf.Abs("Mixed Reality Toolkit".GetHashCode()));
        }
    }
}
