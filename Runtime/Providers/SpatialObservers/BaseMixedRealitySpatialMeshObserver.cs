﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.Extensions;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Interfaces.SpatialAwarenessSystem;
using XRTK.Services;
using XRTK.Utilities.Async;

namespace XRTK.Providers.SpatialObservers
{
    /// <summary>
    /// Base class for spatial awareness observers.
    /// </summary>
    public abstract class BaseMixedRealitySpatialMeshObserver : BaseMixedRealitySpatialObserverDataProvider, IMixedRealitySpatialMeshObserver
    {
        /// <inheritdoc />
        protected BaseMixedRealitySpatialMeshObserver(string name, uint priority, BaseMixedRealitySpatialMeshObserverProfile profile, IMixedRealitySpatialAwarenessSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                profile = MixedRealityToolkit.TryGetSystemProfile<IMixedRealitySpatialAwarenessSystem, MixedRealitySpatialAwarenessSystemProfile>(out var spatialAwarenessSystemProfile)
                    ? spatialAwarenessSystemProfile.GlobalMeshObserverProfile
                    : throw new ArgumentException($"Unable to get a valid {nameof(MixedRealitySpatialAwarenessSystemProfile)}!");
            }

            if (profile.IsNull())
            {
                throw new ArgumentNullException($"Missing a {profile.GetType().Name} profile for {name}");
            }

            MeshLevelOfDetail = profile.MeshLevelOfDetail;
            MeshRecalculateNormals = profile.MeshRecalculateNormals;
            meshDisplayOption = parentService.SpatialMeshVisibility;
            MeshVisibleMaterial = profile.MeshVisibleMaterial;
            MeshOcclusionMaterial = profile.MeshOcclusionMaterial;
            ObservationExtents = profile.ObservationExtents;
            IsStationaryObserver = profile.IsStationaryObserver;
            var additionalComponents = profile.AdditionalComponents;
            meshObjectPrefab = profile.MeshObjectPrefab;
            spatialMeshObjectPool = new Stack<SpatialMeshObject>();

            if (additionalComponents != null)
            {
                requiredMeshComponents = new Type[additionalComponents.Length + 3];
                requiredMeshComponents[0] = typeof(MeshFilter);
                requiredMeshComponents[1] = typeof(MeshRenderer);
                requiredMeshComponents[2] = typeof(MeshCollider);

                for (int i = 3; i < additionalComponents.Length; i++)
                {
                    var component = additionalComponents[i - 3].Type;
                    Debug.Assert(component != null);
                    requiredMeshComponents[i] = component;
                }
            }
            else
            {
                requiredMeshComponents = new[]
                {
                    typeof(MeshFilter),
                    typeof(MeshRenderer),
                    typeof(MeshCollider)
                };
            }
        }

        private readonly GameObject meshObjectPrefab;

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        private readonly Type[] requiredMeshComponents;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying) { return; }

            for (int i = 0; i < 10; i++)
            {
                spatialMeshObjectPool.Push(new SpatialMeshObject(Guid.Empty, CreateBlankSpatialMeshGameObject()));
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (!Application.isPlaying) { return; }

            // If we've got some spatial meshes and were disabled previously, turn them back on.
            foreach (var meshObject in spatialMeshObjects.Values)
            {
                meshObject.GameObject.SetActive(true);
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying || !IsRunning) { return; }

            lock (spatialMeshObjectPool)
            {
                // if we get low in our object pool, then create a few more.
                if (spatialMeshObjectPool.Count < 5)
                {
                    spatialMeshObjectPool.Push(new SpatialMeshObject(Guid.Empty, CreateBlankSpatialMeshGameObject()));
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying) { return; }

            // Disable any spatial meshes we might have.
            foreach (var meshObject in spatialMeshObjects.Values)
            {
                Debug.Assert(meshObject.GameObject != null);
                meshObject.GameObject.SetActive(false);
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (!Application.isPlaying) { return; }

            spatialMeshObjects.Clear();

            lock (spatialMeshObjectPool)
            {
                while (spatialMeshObjectPool.Count > 0)
                {
                    var meshObject = spatialMeshObjectPool.Pop();
                    meshObject.GameObject.Destroy();
                }

                Debug.Assert(spatialMeshObjectPool.Count == 0);
            }
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialMeshObserver Implementation

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get; set; }

        /// <inheritdoc />
        public bool MeshRecalculateNormals { get; }

        private SpatialMeshDisplayOptions meshDisplayOption;

        /// <inheritdoc />
        public SpatialMeshDisplayOptions MeshDisplayOption
        {
            get => meshDisplayOption;
            set
            {
                meshDisplayOption = value;

                if (meshDisplayOption != SpatialMeshDisplayOptions.None)
                {
                    foreach (var spatialMeshObject in SpatialMeshObjects)
                    {
                        spatialMeshObject.Value.Collider.enabled = true;
                        spatialMeshObject.Value.Renderer.enabled = meshDisplayOption == SpatialMeshDisplayOptions.Visible ||
                                                                   meshDisplayOption == SpatialMeshDisplayOptions.Occlusion;
                        spatialMeshObject.Value.Renderer.sharedMaterial = meshDisplayOption == SpatialMeshDisplayOptions.Visible
                            ? MeshVisibleMaterial
                            : MeshOcclusionMaterial;
                    }
                }
                else
                {
                    foreach (var spatialMeshObject in SpatialMeshObjects)
                    {
                        spatialMeshObject.Value.Renderer.enabled = false;
                        spatialMeshObject.Value.Collider.enabled = false;
                    }
                }
            }
        }

        /// <inheritdoc />
        public Material MeshVisibleMaterial { get; }

        /// <inheritdoc />
        public Material MeshOcclusionMaterial { get; }

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; }

        /// <inheritdoc />
        public bool IsStationaryObserver { get; }

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Quaternion ObserverOrientation { get; protected set; } = Quaternion.identity;

        private readonly Dictionary<Guid, SpatialMeshObject> spatialMeshObjects = new Dictionary<Guid, SpatialMeshObject>();

        /// <inheritdoc />
        /// <remarks>
        /// This method returns a copy of the collection maintained by the observer so that application
        /// code can iterate through the collection without concern for changes to the backing data.
        /// </remarks>
        public IReadOnlyDictionary<Guid, SpatialMeshObject> SpatialMeshObjects => new Dictionary<Guid, SpatialMeshObject>(spatialMeshObjects);

        private readonly Stack<SpatialMeshObject> spatialMeshObjectPool;

        /// <inheritdoc />
        public virtual void RaiseMeshAdded(SpatialMeshObject spatialMeshObject)
        {
            SpatialAwarenessSystem.RaiseMeshAdded(this, spatialMeshObject);
        }

        /// <inheritdoc />
        public virtual void RaiseMeshUpdated(SpatialMeshObject spatialMeshObject)
        {
            SpatialAwarenessSystem.RaiseMeshUpdated(this, spatialMeshObject);
        }

        /// <inheritdoc />
        public virtual void RaiseMeshRemoved(SpatialMeshObject spatialMeshObject)
        {
            if (spatialMeshObjects.TryGetValue(spatialMeshObject.Id, out var spatialMesh))
            {
                spatialMeshObjects.Remove(spatialMesh.Id);

                // Raise mesh removed if it was prev enabled.
                // spatialMesh.GameObject's only get enabled when successfully cooked.
                // If it's disabled then likely the mesh was removed before cooking completed.
                if (spatialMesh.GameObject.activeInHierarchy)
                {
                    SpatialAwarenessSystem.RaiseMeshRemoved(this, spatialMeshObject);
                }

                spatialMesh.GameObject.SetActive(false);
                // Recycle this spatial mesh object and add it back to the pool.
                spatialMesh.GameObject.name = "Reclaimed Spatial Mesh Object";
                spatialMesh.Mesh = null;
                spatialMesh.Id = Guid.Empty;
                spatialMesh.LastUpdated = DateTimeOffset.MinValue;

                lock (spatialMeshObjectPool)
                {
                    spatialMeshObjectPool.Push(spatialMesh);
                }
            }
            else
            {
                Debug.LogError($"{spatialMeshObject.Id} is missing from known spatial objects!");
            }
        }

        /// <summary>
        /// Request a <see cref="SpatialMeshObject"/> from the collection of known spatial objects. If that object doesn't exist take one from our pool.
        /// </summary>
        /// <param name="meshId">The id of the <see cref="SpatialMeshObject"/>.</param>
        /// <returns>A <see cref="SpatialMeshObject"/></returns>
        protected async Task<SpatialMeshObject> RequestSpatialMeshObject(Guid meshId)
        {
            if (spatialMeshObjects.TryGetValue(meshId, out var spatialMesh))
            {
                return spatialMesh;
            }

            lock (spatialMeshObjectPool)
            {
                if (spatialMeshObjectPool.Count > 0)
                {
                    spatialMesh = spatialMeshObjectPool.Pop();
                    spatialMesh.Id = meshId;
                    spatialMesh.GameObject.name = $"SpatialMesh_{meshId}";
                    spatialMeshObjects.Add(spatialMesh.Id, spatialMesh);
                    return spatialMesh;
                }
            }

            await Awaiters.UnityMainThread;
            return await RequestSpatialMeshObject(meshId);
        }

        private GameObject CreateBlankSpatialMeshGameObject()
        {
            GameObject newGameObject;

            if (meshObjectPrefab.IsNull())
            {
                newGameObject = new GameObject("Blank Spatial Mesh GameObject", requiredMeshComponents)
                {
                    layer = PhysicsLayer
                };
            }
            else
            {
                newGameObject = UnityEngine.Object.Instantiate(meshObjectPrefab);

                for (var i = 0; i < requiredMeshComponents.Length; i++)
                {
                    newGameObject.EnsureComponent(requiredMeshComponents[i]);
                }

                newGameObject.layer = PhysicsLayer;
            }

            newGameObject.transform.SetParent(SpatialAwarenessSystem.SpatialMeshesParent.transform, false);
            newGameObject.SetActive(false);
            return newGameObject;
        }

        #endregion IMixedRealitySpatialMeshObserver Implementation
    }
}
