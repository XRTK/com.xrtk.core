// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Physics;
using XRTK.EventDatum.Input;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Utilities;
using XRTK.Utilities.Physics;

namespace XRTK.Services.InputSystem
{
    /// <summary>
    /// The focus provider handles the focused objects per input source.
    /// </summary>
    /// <remarks>There are convenience properties for getting only Gaze Pointer if needed.</remarks>
    public class FocusProvider : BaseService, IMixedRealityFocusProvider
    {
        private readonly HashSet<PointerData> pointers = new HashSet<PointerData>();
        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly HashSet<GameObject> pendingOverallFocusExitSet = new HashSet<GameObject>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();
        private readonly PointerHitResult physicsHitResult = new PointerHitResult();
        private readonly PointerHitResult graphicsHitResult = new PointerHitResult();

        #region IFocusProvider Properties

        /// <inheritdoc />
        public override string Name => "Focus Provider";

        /// <inheritdoc />
        public override uint Priority => 2;

        /// <inheritdoc />
        float IMixedRealityFocusProvider.GlobalPointingExtent
        {
            get
            {
                if (MixedRealityToolkit.HasActiveProfile &&
                    MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                    MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile != null)
                {
                    return MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointingExtent;
                }

                return 10f;
            }
        }

        private LayerMask[] focusLayerMasks = null;

        /// <inheritdoc />
        public LayerMask[] FocusLayerMasks
        {
            get
            {
                if (focusLayerMasks == null)
                {
                    if (MixedRealityToolkit.HasActiveProfile &&
                        MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                        MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile != null)
                    {
                        return focusLayerMasks = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointingRaycastLayerMasks;
                    }

                    return focusLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };
                }

                return focusLayerMasks;
            }
        }

        private Camera uiRaycastCamera = null;

        /// <inheritdoc />
        public Camera UIRaycastCamera
        {
            get
            {
                if (uiRaycastCamera == null)
                {
                    EnsureUiRaycastCameraSetup();
                }

                return uiRaycastCamera;
            }
        }

        #endregion IFocusProvider Properties

        /// <summary>
        /// Checks if the <see cref="MixedRealityToolkit"/> is setup correctly to start this service.
        /// </summary>
        private bool IsSetupValid
        {
            get
            {
                if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled) { return false; }

                if (MixedRealityToolkit.InputSystem == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Input System is required for this feature.");
                    return false;
                }

                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Input System Profile is required for this feature.");
                    return false;
                }

                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Pointer Profile is required for this feature.");
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// GazeProvider is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        private PointerData gazeProviderPointingData;

        /// <summary>
        /// Cached <see cref="Vector3"/> reference to the new raycast position.
        /// </summary>
        /// <remarks>Only used to update UI raycast results.</remarks>
        private Vector3 newUiRaycastPosition = Vector3.zero;

        [Serializable]
        private class PointerData : IPointerResult, IEquatable<PointerData>
        {
            private const int IgnoreRaycastLayer = 2;

            public readonly IMixedRealityPointer Pointer;

            private FocusDetails focusDetails;

            /// <inheritdoc />
            public Vector3 StartPoint { get; private set; }

            /// <inheritdoc />
            public Vector3 EndPoint => focusDetails.EndPoint;

            /// <inheritdoc />
            public Vector3 EndPointLocalSpace => focusDetails.EndPointLocalSpace;

            /// <inheritdoc />
            public GameObject CurrentPointerTarget { get; private set; }

            private GameObject syncedPointerTarget;

            /// <inheritdoc />
            public GameObject PreviousPointerTarget { get; private set; }

            /// <inheritdoc />
            public GameObject LastHitObject => focusDetails.HitObject;

            /// <inheritdoc />
            public int RayStepIndex { get; private set; }

            /// <inheritdoc />
            public float RayDistance => focusDetails.RayDistance;

            /// <inheritdoc />
            public Vector3 Normal => focusDetails.Normal;

            /// <inheritdoc />
            public Vector3 NormalLocalSpace => focusDetails.NormalLocalSpace;

            /// <inheritdoc />
            public Vector3 Direction { get; private set; }

            /// <inheritdoc />
            public RaycastHit LastRaycastHit => focusDetails.LastRaycastHit;

            /// <inheritdoc />
            public RaycastResult LastGraphicsRaycastResult => focusDetails.LastGraphicsRaycastResult;

            /// <inheritdoc />
            public Vector3 GrabPointLocalSpace { get; private set; }

            /// <inheritdoc />
            public Vector3 GrabPoint { get; private set; }

            /// <summary>
            /// The graphic input event data used for raycasting uGUI elements.
            /// </summary>
            public GraphicInputEventData GraphicEventData
            {
                get
                {
                    if (graphicData == null)
                    {
                        graphicData = new GraphicInputEventData(EventSystem.current);
                        graphicData.Clear();
                    }

                    Debug.Assert(graphicData != null);

                    return graphicData;
                }
            }

            private GraphicInputEventData graphicData;
            private int prevPhysicsLayer;
            private Vector3 lastPosition;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pointer"></param>
            public PointerData(IMixedRealityPointer pointer)
            {
                focusDetails = new FocusDetails();
                Pointer = pointer;
            }

            public void UpdateHit(PointerHitResult hitResult, GameObject syncTarget)
            {
                focusDetails.LastRaycastHit = hitResult.RaycastHit;
                focusDetails.LastGraphicsRaycastResult = hitResult.GraphicsRaycastResult;

                if (hitResult.RayStepIndex >= 0)
                {
                    RayStepIndex = hitResult.RayStepIndex;
                    StartPoint = hitResult.Ray.Origin;

                    focusDetails.RayDistance = hitResult.RayDistance;
                    focusDetails.EndPoint = hitResult.HitPointOnObject;
                    focusDetails.Normal = hitResult.HitNormalOnObject;
                }
                else
                {
                    // If we don't have a valid ray cast, use the whole pointer ray.focusDetails.EndPoint
                    var firstStep = Pointer.Rays[0];
                    var finalStep = Pointer.Rays[Pointer.Rays.Length - 1];
                    RayStepIndex = 0;

                    StartPoint = firstStep.Origin;

                    var rayDistance = 0.0f;

                    for (int i = 0; i < Pointer.Rays.Length; i++)
                    {
                        rayDistance += Pointer.Rays[i].Length;
                    }

                    focusDetails.RayDistance = rayDistance;
                    focusDetails.EndPoint = finalStep.Terminus;
                    focusDetails.Normal = -finalStep.Direction;
                }

                Direction = focusDetails.EndPoint - lastPosition;
                lastPosition = focusDetails.EndPoint;

                focusDetails.HitObject = hitResult.HitObject;

                if (syncTarget != null)
                {
                    if (syncedPointerTarget == null && CurrentPointerTarget != null && CurrentPointerTarget == syncTarget)
                    {
                        Debug.Assert(CurrentPointerTarget != null, "No Sync Target Set!");

                        syncedPointerTarget = CurrentPointerTarget;

                        prevPhysicsLayer = CurrentPointerTarget.layer;
                        Debug.Assert(prevPhysicsLayer != IgnoreRaycastLayer, $"Failed to get a valid raycast layer for {syncedPointerTarget.name}: {LayerMask.LayerToName(prevPhysicsLayer)}");
                        CurrentPointerTarget.SetLayerRecursively(IgnoreRaycastLayer);

                        var grabPoint = Pointer.OverrideGrabPoint ?? focusDetails.EndPoint;

                        if (grabPoint == Vector3.zero)
                        {
                            GrabPoint = CurrentPointerTarget.transform.TransformPoint(grabPoint);
                            GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);
                        }
                        else
                        {
                            GrabPoint = grabPoint;
                            GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);
                        }
                    }
                    else if (syncTarget != CurrentPointerTarget)
                    {
                        GetCurrentTarget();
                    }

                    if (syncedPointerTarget != null)
                    {
                        GrabPoint = CurrentPointerTarget.transform.TransformPoint(GrabPointLocalSpace);
                        GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);

                        // Visualize the relevant points and their relation
                        if (Application.isEditor && MixedRealityRaycaster.DebugEnabled)
                        {
                            DebugUtilities.DrawPoint(GrabPoint, Color.red);
                            DebugUtilities.DrawPoint(focusDetails.EndPoint, Color.yellow);

                            Debug.DrawLine(focusDetails.EndPoint, GrabPoint, Color.magenta);

                            var currentPosition = CurrentPointerTarget.transform.position;
                            var targetPosition = (focusDetails.EndPoint + currentPosition) - GrabPoint;

                            Debug.DrawLine(GrabPoint, currentPosition, Color.magenta);
                            Debug.DrawLine(currentPosition, GrabPoint, Color.magenta);
                            DebugUtilities.DrawPoint(currentPosition, Color.cyan);
                            DebugUtilities.DrawPoint(targetPosition, Color.blue);

                            Debug.DrawLine(targetPosition, currentPosition, Color.blue);
                        }
                    }
                }
                else
                {
                    GetCurrentTarget();
                }

                void GetCurrentTarget()
                {
                    if (syncedPointerTarget != null)
                    {
                        syncedPointerTarget.SetLayerRecursively(prevPhysicsLayer);
                        syncedPointerTarget = null;
                    }

                    PreviousPointerTarget = CurrentPointerTarget;
                    CurrentPointerTarget = focusDetails.HitObject;
                    Pointer.OverrideGrabPoint = null;
                    GrabPoint = Vector3.zero;
                    GrabPointLocalSpace = Vector3.zero;
                }

                if (CurrentPointerTarget != null)
                {
                    focusDetails.EndPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(focusDetails.EndPoint);
                    focusDetails.NormalLocalSpace = CurrentPointerTarget.transform.InverseTransformDirection(focusDetails.Normal);
                }
                else
                {
                    focusDetails.EndPointLocalSpace = Vector3.zero;
                    focusDetails.NormalLocalSpace = Vector3.zero;
                }
            }

            /// <summary>
            /// Update focus information while focus is locked. If the object is moving,
            /// this updates the hit point to its new world transform.
            /// </summary>
            public void UpdateFocusLockedHit()
            {
                if (focusDetails.HitObject != null)
                {
                    // In case the focused object is moving, we need to update the focus point based on the object's new transform.
                    focusDetails.EndPoint = focusDetails.HitObject.transform.TransformPoint(focusDetails.EndPointLocalSpace);
                    focusDetails.Normal = focusDetails.HitObject.transform.TransformDirection(focusDetails.NormalLocalSpace);

                    focusDetails.EndPointLocalSpace = focusDetails.HitObject.transform.InverseTransformPoint(focusDetails.EndPoint);
                    focusDetails.NormalLocalSpace = focusDetails.HitObject.transform.InverseTransformDirection(focusDetails.Normal);
                }

                StartPoint = Pointer.Rays[0].Origin;

                for (int i = 0; i < Pointer.Rays.Length; i++)
                {
                    // TODO: figure out how reliable this is. Should focusDetails.RayDistance be updated?
                    if (Pointer.Rays[i].Contains(focusDetails.EndPoint))
                    {
                        RayStepIndex = i;
                        break;
                    }
                }
            }

            /// <summary>
            /// Rest the currently focused object data.
            /// </summary>
            /// <param name="clearPreviousObject">Optional flag to choose not to clear the previous object.</param>
            public void ResetFocusedObjects(bool clearPreviousObject = true)
            {
                PreviousPointerTarget = clearPreviousObject ? null : CurrentPointerTarget;

                focusDetails.EndPoint = focusDetails.EndPoint;
                focusDetails.EndPointLocalSpace = focusDetails.EndPointLocalSpace;
                focusDetails.Normal = focusDetails.Normal;
                focusDetails.NormalLocalSpace = focusDetails.NormalLocalSpace;
                focusDetails.HitObject = null;
            }

            /// <inheritdoc />
            public bool Equals(PointerData other)
            {
                if (other is null) { return false; }
                if (ReferenceEquals(this, other)) { return true; }
                return Pointer.PointerId == other.Pointer.PointerId;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (obj is null) { return false; }
                if (ReferenceEquals(this, obj)) { return true; }
                return obj is PointerData pointer && Equals(pointer);
            }

            /// <inheritdoc />
            public override int GetHashCode() => Pointer != null ? Pointer.GetHashCode() : 0;
        }

        /// <summary>
        /// Helper class for storing intermediate hit results. Should be applied to the PointerData once all
        /// possible hits of a pointer have been processed.
        /// </summary>
        private class PointerHitResult
        {
            public RaycastHit RaycastHit;
            public RaycastResult GraphicsRaycastResult;

            public GameObject HitObject;
            public Vector3 HitPointOnObject;
            public Vector3 HitNormalOnObject;

            public RayStep Ray;
            public int RayStepIndex = -1;
            public float RayDistance;

            /// <summary>
            /// Clears the pointer result.
            /// </summary>
            public void Clear()
            {
                RaycastHit = default;
                GraphicsRaycastResult = default;

                HitObject = null;
                HitPointOnObject = Vector3.zero;
                HitNormalOnObject = Vector3.zero;

                Ray = default;
                RayStepIndex = -1;
                RayDistance = 0.0f;
            }

            /// <summary>
            /// Set hit focus information from a closest-collider-to pointer check.
            /// </summary>
            public void Set(GameObject hitObject, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = default;
                GraphicsRaycastResult = default;

                HitObject = hitObject;
                HitPointOnObject = hitPointOnObject;
                HitNormalOnObject = hitNormalOnObject;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit focus information from a physics raycast.
            /// </summary>
            public void Set(RaycastHit hit, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = hit;
                GraphicsRaycastResult = default;

                HitObject = hit.transform.gameObject;
                HitPointOnObject = hit.point;
                HitNormalOnObject = hit.normal;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit information from a canvas raycast.
            /// </summary>
            public void Set(RaycastResult result, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = default;
                GraphicsRaycastResult = result;

                HitObject = result.gameObject;
                HitPointOnObject = hitPointOnObject;
                HitNormalOnObject = hitNormalOnObject;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!IsSetupValid) { return; }

            foreach (var inputSource in MixedRealityToolkit.InputSystem.DetectedInputSources)
            {
                RegisterPointers(inputSource);
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                UpdateCanvasEventSystems();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!IsSetupValid) { return; }

            UpdatePointers();
            UpdateFocusedObjects();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (uiRaycastCamera != null)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(uiRaycastCamera.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(uiRaycastCamera.gameObject);
                }
            }
        }

        #endregion IMixedRealityService Implementation

        #region Focus Details by IMixedRealityPointer

        /// <inheritdoc />
        public GameObject GetFocusedObject(IMixedRealityPointer pointingSource)
        {
            if (pointingSource == null)
            {
                Debug.LogError("No Pointer passed to get focused object");
                return null;
            }

            return !TryGetFocusDetails(pointingSource, out var focusDetails) ? null : focusDetails.CurrentPointerTarget;
        }

        /// <inheritdoc />
        public bool TryGetFocusDetails(IMixedRealityPointer pointer, out IPointerResult focusDetails)
        {
            if (TryGetPointerData(pointer, out var pointerData))
            {
                focusDetails = pointerData;
                return true;
            }

            focusDetails = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetSpecificPointerGraphicEventData(IMixedRealityPointer pointer, out GraphicInputEventData graphicInputEventData)
        {
            if (TryGetPointerData(pointer, out var pointerData))
            {
                Debug.Assert(pointerData.GraphicEventData != null);
                graphicInputEventData = pointerData.GraphicEventData;
                graphicInputEventData.selectedObject = pointerData.GraphicEventData.pointerCurrentRaycast.gameObject;
                return true;
            }

            graphicInputEventData = null;
            return false;
        }
        #endregion Focus Details by IMixedRealityPointer

        #region Utilities

        /// <inheritdoc />
        public uint GenerateNewPointerId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == newId)
                {
                    return GenerateNewPointerId();
                }
            }

            return newId;
        }

        /// <summary>
        /// Utility for validating the UIRaycastCamera.
        /// </summary>
        /// <returns>The UIRaycastCamera</returns>
        private void EnsureUiRaycastCameraSetup()
        {
            const string uiRayCastCameraName = "UIRaycastCamera";
            var cameraTransform = CameraCache.Main.transform;
            var uiCameraTransform = cameraTransform.Find(uiRayCastCameraName);
            GameObject cameraObject = null;

            if (uiCameraTransform != null)
            {
                cameraObject = uiCameraTransform.gameObject;
            }

            if (cameraObject == null)
            {
                cameraObject = new GameObject { name = uiRayCastCameraName };
                cameraObject.transform.parent = cameraTransform;
            }

            Debug.Assert(cameraObject.transform.parent == cameraTransform);
            cameraObject.transform.localPosition = Vector3.zero;
            cameraObject.transform.localRotation = Quaternion.identity;

            // The raycast camera is used to raycast into the UI scene,
            // it doesn't need to render anything so is disabled
            // The default settings are all that is necessary 
            uiRaycastCamera = cameraObject.EnsureComponent<Camera>();
            uiRaycastCamera.enabled = false;
        }

        /// <summary>
        /// Helper for assigning world space canvases event cameras.
        /// </summary>
        /// <remarks>Warning! Very expensive. Use sparingly at runtime.</remarks>
        public void UpdateCanvasEventSystems()
        {
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusProvider before updating your canvases.");

            // This will also find disabled GameObjects in the scene.
            // Warning! this look up is very expensive!
            var sceneCanvases = Resources.FindObjectsOfTypeAll<Canvas>();

            for (var i = 0; i < sceneCanvases.Length; i++)
            {
                if (sceneCanvases[i].isRootCanvas && sceneCanvases[i].renderMode == RenderMode.WorldSpace)
                {
                    sceneCanvases[i].worldCamera = uiRaycastCamera;
                }
            }
        }

        /// <inheritdoc />
        public bool IsPointerRegistered(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");
            return TryGetPointerData(pointer, out _);
        }

        /// <inheritdoc />
        public bool RegisterPointer(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            if (IsPointerRegistered(pointer)) { return false; }

            var pointerData = new PointerData(pointer);
            pointers.Add(pointerData);
            // Initialize the pointer result
            UpdatePointer(pointerData);
            return true;
        }

        private void RegisterPointers(IMixedRealityInputSource inputSource)
        {
            // If our input source does not have any pointers, then skip.
            if (inputSource.Pointers == null) { return; }

            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                RegisterPointer(inputSource.Pointers[i]);

                // Special Registration for Gaze
                if (inputSource.SourceId == MixedRealityToolkit.InputSystem.GazeProvider.GazeInputSource.SourceId && gazeProviderPointingData == null)
                {
                    gazeProviderPointingData = new PointerData(inputSource.Pointers[i]);
                }
            }
        }

        /// <inheritdoc />
        public bool UnregisterPointer(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            if (!TryGetPointerData(pointer, out var pointerData)) { return false; }

            // Raise focus events if needed.
            if (pointerData.CurrentPointerTarget != null)
            {
                var unfocusedObject = pointerData.CurrentPointerTarget;
                var objectIsStillFocusedByOtherPointer = false;

                foreach (var otherPointer in pointers)
                {
                    if (otherPointer.Pointer.PointerId != pointer.PointerId &&
                        otherPointer.CurrentPointerTarget == unfocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                if (!objectIsStillFocusedByOtherPointer)
                {
                    // Policy: only raise focus exit if no other pointers are still focusing the object
                    MixedRealityToolkit.InputSystem.RaiseFocusExit(pointer, unfocusedObject);
                }

                MixedRealityToolkit.InputSystem.RaisePreFocusChanged(pointer, unfocusedObject, null);
            }

            pointers.Remove(pointerData);
            return true;
        }

        /// <summary>
        /// Returns the registered PointerData for the provided pointing input source.
        /// </summary>
        /// <param name="pointer">the pointer who's data we're looking for</param>
        /// <param name="data">The data associated to the pointer</param>
        /// <returns>Pointer Data if the pointing source is registered.</returns>
        private bool TryGetPointerData(IMixedRealityPointer pointer, out PointerData data)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == pointer.PointerId)
                {
                    data = pointerData;
                    return true;
                }
            }

            data = null;
            return false;
        }

        private void UpdatePointers()
        {
            int pointerCount = 0;

            foreach (var pointer in pointers)
            {
                UpdatePointer(pointer);

                // TODO remove profile call here and use a value set on the pointer itself.
                var pointerProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile;

                if (pointerProfile == null || !pointerProfile.DebugDrawPointingRays) { continue; }

                // TODO Let's only set this once on start.This will overwrite the property each update.
                MixedRealityRaycaster.DebugEnabled = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.DebugDrawPointingRays;

                Color rayColor;

                if (pointerProfile.DebugDrawPointingRayColors != null &&
                    pointerProfile.DebugDrawPointingRayColors.Length > 0)
                {
                    rayColor = pointerProfile.DebugDrawPointingRayColors[pointerCount++ % pointerProfile.DebugDrawPointingRayColors.Length];
                }
                else
                {
                    rayColor = Color.green;
                }

                Debug.DrawRay(pointer.StartPoint, (pointer.EndPoint - pointer.StartPoint), rayColor);
            }
        }

        private void UpdatePointer(PointerData pointer)
        {
            // Call the pointer's OnPreRaycast function
            // This will give it a chance to prepare itself for raycasts
            // eg, by building its Rays array
            pointer.Pointer.OnPreRaycast();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointer.Pointer.IsInteractionEnabled)
            {
                // Don't clear the previous focused object since we still want to trigger FocusExit events
                pointer.ResetFocusedObjects(false);

                // Only set the result if it's null
                // Otherwise we'd get incorrect data.
                if (pointer.Pointer.Result == null)
                {
                    pointer.Pointer.Result = pointer;
                }
            }
            else
            {
                // If the pointer is locked, keep the focused object the same.
                // This will ensure that we execute events on those objects
                // even if the pointer isn't pointing at them.
                // We don't want to update focused locked hits if we're syncing the pointer's target position.
                if (pointer.Pointer.IsFocusLocked && pointer.Pointer.SyncedTarget == null)
                {
                    pointer.UpdateFocusLockedHit();
                }
                else
                {
                    // Otherwise, continue
                    var prioritizedLayerMasks = (pointer.Pointer.PrioritizedLayerMasksOverride ?? FocusLayerMasks);

                    physicsHitResult.Clear();

                    // Perform raycast to determine focused object
                    RaycastPhysics(pointer.Pointer, prioritizedLayerMasks, physicsHitResult);
                    var currentHitResult = physicsHitResult;

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        graphicsHitResult.Clear();
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        RaycastGraphics(pointer.Pointer, pointer.GraphicEventData, prioritizedLayerMasks, graphicsHitResult);

                        currentHitResult = GetPrioritizedHitResult(currentHitResult, graphicsHitResult, prioritizedLayerMasks);
                    }

                    // Apply the hit result only now so changes in the current target are detected only once per frame.
                    pointer.UpdateHit(currentHitResult, pointer.Pointer.SyncedTarget);
                }

                // Set the pointer's result last
                pointer.Pointer.Result = pointer;
            }

            Debug.Assert(pointer.Pointer.Result != null);

            // Call the pointer's OnPostRaycast function
            // This will give it a chance to respond to raycast results
            // eg by updating its appearance
            pointer.Pointer.OnPostRaycast();
        }

        #region Physics Raycasting

        private static PointerHitResult GetPrioritizedHitResult(PointerHitResult hit1, PointerHitResult hit2, LayerMask[] prioritizedLayerMasks)
        {
            if (hit1.HitObject != null && hit2.HitObject != null)
            {
                // Check layer prioritization.
                if (prioritizedLayerMasks.Length > 1)
                {
                    // Get the index in the prioritized layer masks
                    int layerIndex1 = hit1.HitObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                    int layerIndex2 = hit2.HitObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                    if (layerIndex1 != layerIndex2)
                    {
                        return (layerIndex1 < layerIndex2) ? hit1 : hit2;
                    }
                }

                // Check which hit is closer.
                return hit1.RayDistance < hit2.RayDistance ? hit1 : hit2;
            }

            return hit1.HitObject != null ? hit1 : hit2;
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="hitResult"></param>
        private static void RaycastPhysics(IMixedRealityPointer pointer, LayerMask[] prioritizedLayerMasks, PointerHitResult hitResult)
        {
            float rayStartDistance = 0;
            var pointerRays = pointer.Rays;

            if (pointerRays == null)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            if (pointerRays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer");
                return;
            }

            // Check raycast for each step in the pointing source
            for (int i = 0; i < pointerRays.Length; i++)
            {
                switch (pointer.RaycastMode)
                {
                    case RaycastMode.Simple:
                        if (MixedRealityRaycaster.RaycastSimplePhysicsStep(pointerRays[i], prioritizedLayerMasks, out var simplePhysicsHit))
                        {
                            // Set the pointer source's origin ray to this step
                            UpdatePointerRayOnHit(pointerRays, simplePhysicsHit, i, rayStartDistance, hitResult);
                            return;
                        }
                        break;
                    case RaycastMode.Box:
                        Debug.LogWarning("Box Raycasting Mode not supported for pointers.");
                        break;
                    case RaycastMode.Sphere:
                        if (MixedRealityRaycaster.RaycastSpherePhysicsStep(pointerRays[i], pointer.SphereCastRadius, prioritizedLayerMasks, out var spherePhysicsHit))
                        {
                            // Set the pointer source's origin ray to this step
                            UpdatePointerRayOnHit(pointerRays, spherePhysicsHit, i, rayStartDistance, hitResult);
                            return;
                        }
                        break;
                    // TODO SphereOverlap
                    default:
                        Debug.LogError($"Invalid raycast mode {pointer.RaycastMode} for {pointer.PointerName} pointer.");
                        break;
                }

                rayStartDistance += pointer.Rays[i].Length;
            }
        }

        private static void UpdatePointerRayOnHit(RayStep[] raySteps, RaycastHit physicsHit, int hitRayIndex, float rayStartDistance, PointerHitResult hitResult)
        {
            var origin = raySteps[hitRayIndex].Origin;
            var terminus = physicsHit.point;
            raySteps[hitRayIndex].UpdateRayStep(ref origin, ref terminus);
            hitResult.Set(physicsHit, raySteps[hitRayIndex], hitRayIndex, rayStartDistance + physicsHit.distance);
        }

        #endregion Physics Raycasting

        #region uGUI Graphics Raycasting

        /// <summary>
        /// Perform a Unity Graphics Raycast to determine which uGUI element is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="graphicEventData"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="hitResult"></param>
        private void RaycastGraphics(IMixedRealityPointer pointer, PointerEventData graphicEventData, LayerMask[] prioritizedLayerMasks, PointerHitResult hitResult)
        {
            if (UIRaycastCamera == null)
            {
                Debug.LogError("Missing UIRaycastCamera!");
                return;
            }

            if (!uiRaycastCamera.nearClipPlane.Equals(0.01f))
            {
                uiRaycastCamera.nearClipPlane = 0.01f;
            }

            if (pointer.Rays == null)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            if (pointer.Rays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer");
                return;
            }

            // Cast rays for every step until we score a hit
            float totalDistance = 0.0f;

            for (int i = 0; i < pointer.Rays.Length; i++)
            {
                if (RaycastGraphicsStep(graphicEventData, pointer.Rays[i], prioritizedLayerMasks, out var raycastResult) &&
                    raycastResult.isValid &&
                    raycastResult.distance < pointer.Rays[i].Length &&
                    raycastResult.module != null &&
                    raycastResult.module.eventCamera == UIRaycastCamera)
                {
                    totalDistance += raycastResult.distance;

                    newUiRaycastPosition.x = raycastResult.screenPosition.x;
                    newUiRaycastPosition.y = raycastResult.screenPosition.y;
                    newUiRaycastPosition.z = raycastResult.distance;

                    var worldPos = uiRaycastCamera.ScreenToWorldPoint(newUiRaycastPosition);
                    var normal = -raycastResult.gameObject.transform.forward;

                    hitResult.Set(raycastResult, worldPos, normal, pointer.Rays[i], i, totalDistance);
                    return;
                }

                totalDistance += pointer.Rays[i].Length;
            }
        }

        /// <summary>
        /// Raycasts each graphic <see cref="RayStep"/>
        /// </summary>
        /// <param name="graphicEventData"></param>
        /// <param name="step"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="uiRaycastResult"></param>
        private bool RaycastGraphicsStep(PointerEventData graphicEventData, RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastResult uiRaycastResult)
        {
            uiRaycastResult = default;
            var currentEventSystem = EventSystem.current;

            if (currentEventSystem == null)
            {
                Debug.LogError("Current Event System is Invalid!");
                return false;
            }

            if (step.Direction == Vector3.zero)
            {
                Debug.LogError("RayStep Direction is Invalid!");
                return false;
            }

            // Move the uiRaycast camera to the current pointer's position.
            uiRaycastCamera.transform.position = step.Origin;
            uiRaycastCamera.transform.rotation = Quaternion.LookRotation(step.Direction, Vector3.up);

            // We always raycast from the center of the camera.
            var newPosition = graphicRaycastMultiplier;
            newPosition.x *= uiRaycastCamera.pixelWidth;
            newPosition.y *= uiRaycastCamera.pixelHeight;
            graphicEventData.position = newPosition;

            // Graphics raycast
            uiRaycastResult = currentEventSystem.Raycast(graphicEventData, prioritizedLayerMasks);
            graphicEventData.pointerCurrentRaycast = uiRaycastResult;

            return uiRaycastCamera.gameObject != null;
        }

        private readonly Vector2 graphicRaycastMultiplier = new Vector2(0.5f, 0.5f);

        #endregion uGUI Graphics Raycasting

        /// <summary>
        /// Raises the Focus Events to the Input Manger if needed.
        /// </summary>
        private void UpdateFocusedObjects()
        {
            Debug.Assert(pendingPointerSpecificFocusChange.Count == 0);
            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);

            // NOTE: We compute the set of events to send before sending the first event
            //       just in case someone responds to the event by adding/removing a
            //       pointer which would change the structures we're iterating over.

            foreach (var pointer in pointers)
            {
                if (pointer.PreviousPointerTarget == pointer.CurrentPointerTarget) { continue; }

                pendingPointerSpecificFocusChange.Add(pointer);

                // Initially, we assume all pointer-specific focus changes will
                // also result in an overall focus change...

                if (pointer.PreviousPointerTarget != null)
                {
                    pendingOverallFocusExitSet.Add(pointer.PreviousPointerTarget);
                }

                if (pointer.CurrentPointerTarget != null)
                {
                    pendingOverallFocusEnterSet.Add(pointer.CurrentPointerTarget);
                }
            }

            // ... but now we trim out objects whose overall focus was maintained the same by a different pointer:

            foreach (var pointer in pointers)
            {
                pendingOverallFocusExitSet.Remove(pointer.CurrentPointerTarget);
                pendingOverallFocusEnterSet.Remove(pointer.PreviousPointerTarget);
            }

            // Now we raise the events:
            for (int iChange = 0; iChange < pendingPointerSpecificFocusChange.Count; iChange++)
            {
                var change = pendingPointerSpecificFocusChange[iChange];
                var pendingUnfocusedObject = change.PreviousPointerTarget;
                var pendingFocusObject = change.CurrentPointerTarget;

                MixedRealityToolkit.InputSystem.RaisePreFocusChanged(change.Pointer, pendingUnfocusedObject, pendingFocusObject);

                if (pendingOverallFocusExitSet.Contains(pendingUnfocusedObject))
                {
                    MixedRealityToolkit.InputSystem.RaiseFocusExit(change.Pointer, pendingUnfocusedObject);
                    pendingOverallFocusExitSet.Remove(pendingUnfocusedObject);
                }

                if (pendingOverallFocusEnterSet.Contains(pendingFocusObject))
                {
                    MixedRealityToolkit.InputSystem.RaiseFocusEnter(change.Pointer, pendingFocusObject);
                    pendingOverallFocusEnterSet.Remove(pendingFocusObject);
                }

                MixedRealityToolkit.InputSystem.RaiseFocusChanged(change.Pointer, pendingUnfocusedObject, pendingFocusObject);
            }

            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);
            pendingPointerSpecificFocusChange.Clear();
        }

        #endregion Accessors

        #region ISourceState Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            RegisterPointers(eventData.InputSource);
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            // If the input source does not have pointers, then skip.
            if (eventData.InputSource.Pointers == null) { return; }

            for (var i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                // Special unregistration for Gaze
                if (gazeProviderPointingData != null && eventData.InputSource.Pointers[i].PointerId == gazeProviderPointingData.Pointer.PointerId)
                {
                    // If the source lost is the gaze input source, then reset it.
                    if (eventData.InputSource.SourceId == MixedRealityToolkit.InputSystem.GazeProvider.GazeInputSource.SourceId)
                    {
                        gazeProviderPointingData.ResetFocusedObjects();
                        gazeProviderPointingData = null;
                    }
                    // Otherwise, don't unregister the gaze pointer, since the gaze input source is still active.
                    else
                    {
                        continue;
                    }
                }

                UnregisterPointer(eventData.InputSource.Pointers[i]);
            }
        }

        #endregion ISourceState Implementation
    }
}