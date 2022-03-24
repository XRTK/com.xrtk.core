// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.EventDatum.SpatialAwarenessSystem;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Interfaces.SpatialAwarenessSystem;
using XRTK.Interfaces.SpatialAwarenessSystem.Handlers;
using XRTK.Providers.SpatialObservers;
using XRTK.Utilities;

namespace XRTK.Services.SpatialAwarenessSystem
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    [System.Runtime.InteropServices.Guid("05EF9DDC-13C2-47D4-84C5-1C9CB6CC5C1C")]
    public class MixedRealitySpatialAwarenessSystem : BaseEventSystem, IMixedRealitySpatialAwarenessSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealitySpatialAwarenessSystem(MixedRealitySpatialAwarenessSystemProfile profile) : base(profile)
        {
            spatialMeshVisibility = profile.MeshDisplayOption;
        }

        private GameObject spatialAwarenessParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        public GameObject SpatialAwarenessRootParent => spatialAwarenessParent != null ? spatialAwarenessParent : (spatialAwarenessParent = CreateSpatialAwarenessParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent
        {
            get
            {
                var spatialAwarenessSystemObject = new GameObject("Spatial Awareness System");
                var rigTransform = MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem)
                    ? cameraSystem.MainCameraRig.RigTransform
                    : CameraCache.Main.transform.parent;
                spatialAwarenessSystemObject.transform.SetParent(rigTransform, false);
                return spatialAwarenessSystemObject;
            }
        }

        private GameObject meshParent = null;

        /// <inheritdoc />
        public GameObject SpatialMeshesParent => meshParent != null ? meshParent : (meshParent = CreateSecondGenerationParent("Meshes"));

        private GameObject surfaceParent = null;

        /// <inheritdoc />
        public GameObject SurfacesParent => surfaceParent != null ? surfaceParent : (surfaceParent = CreateSecondGenerationParent("Surfaces"));

        /// <inheritdoc />
        public SpatialMeshDisplayOptions SpatialMeshVisibility
        {
            get => spatialMeshVisibility;
            set
            {
                spatialMeshVisibility = value;

                foreach (var observer in DetectedSpatialObservers)
                {
                    if (observer is BaseMixedRealitySpatialMeshObserver meshObserver)
                    {
                        meshObserver.MeshDisplayOption = spatialMeshVisibility;
                    }
                }
            }
        }

        private SpatialMeshDisplayOptions spatialMeshVisibility;

        /// <summary>
        /// Creates a parent that is a child of the Spatial Awareness System parent so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness objects will be parented.
        /// </returns>
        private GameObject CreateSecondGenerationParent(string name)
        {
            var secondGeneration = new GameObject(name);
            secondGeneration.transform.SetParent(SpatialAwarenessRootParent.transform, false);
            return secondGeneration;
        }

        #region IMixedRealitySpatialAwarenessSystem Implementation

        /// <inheritdoc />
        public HashSet<IMixedRealitySpatialAwarenessDataProvider> DetectedSpatialObservers { get; } = new HashSet<IMixedRealitySpatialAwarenessDataProvider>();

        /// <inheritdoc />
        public bool IsObserverRunning(IMixedRealitySpatialAwarenessDataProvider observer)
        {
            foreach (var detectedObserver in DetectedSpatialObservers)
            {
                if (detectedObserver.SourceId == observer.SourceId)
                {
                    return observer.IsRunning;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public uint GenerateNewObserverId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var observer in DetectedSpatialObservers)
            {
                if (observer.SourceId == newId)
                {
                    return GenerateNewObserverId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public void StartObserver(IMixedRealitySpatialAwarenessDataProvider observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StartObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObserver(IMixedRealitySpatialAwarenessDataProvider observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StopObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverDetected(IMixedRealitySpatialAwarenessDataProvider observer)
        {
            DetectedSpatialObservers.Add(observer);
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverLost(IMixedRealitySpatialAwarenessDataProvider observer)
        {
            DetectedSpatialObservers.Remove(observer);
        }

        #endregion IMixedRealitySpatialAwarenessSystem Implementation

        #region IMixedRealityService Implementation

        private MixedRealitySpatialAwarenessEventData<SpatialMeshObject> meshEventData = null;
        private MixedRealitySpatialAwarenessEventData<GameObject> surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                var eventSystem = EventSystem.current;
                meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialMeshObject>(eventSystem);
                surfaceFindingEventData = new MixedRealitySpatialAwarenessEventData<GameObject>(eventSystem);
            }
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            base.Destroy();

            if (!Application.isPlaying) { return; }

            if (spatialAwarenessParent != null)
            {
                spatialAwarenessParent.transform.DetachChildren();
                spatialAwarenessParent.Destroy();
            }

            // Detach the mesh objects (they are to be cleaned up by the observer) and cleanup the parent
            if (meshParent != null)
            {
                meshParent.transform.DetachChildren();
                meshParent.Destroy();
            }

            // Detach the surface objects (they are to be cleaned up by the observer) and cleanup the parent
            if (surfaceParent != null)
            {
                surfaceParent.transform.DetachChildren();
                surfaceParent.Destroy();
            }
        }

        #region Mesh Events

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshRemoved(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshRemoved(spatialEventData);
            };

        #endregion Mesh Events

        #region Surface Finding Events

        /// <inheritdoc />
        public void RaiseSurfaceAdded(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceAdded);
        }

        /// <summary>
        /// Event sent whenever a planar surface is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceAdded =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceUpdated(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceUpdated);
        }

        /// <summary>
        /// Event sent whenever a planar surface is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceUpdated =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceRemoved(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, null);
            HandleEvent(surfaceFindingEventData, OnSurfaceRemoved);
        }

        /// <summary>
        /// Event sent whenever a planar surface is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceRemoved =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceRemoved(spatialEventData);
            };

        #endregion Surface Finding Events

        #endregion IMixedRealityService Implementation
    }
}
