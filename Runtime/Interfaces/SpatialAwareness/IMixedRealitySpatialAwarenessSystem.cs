// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.Interfaces.Events;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Interfaces.SpatialAwarenessSystem.Handlers;

namespace XRTK.Interfaces.SpatialAwarenessSystem
{
    /// <summary>
    /// The interface definition for Spatial Awareness features in the Mixed Reality Toolkit.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        GameObject SpatialAwarenessRootParent { get; }

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created <see cref="SpatialMeshObject"/>s.
        /// </summary>
        GameObject SpatialMeshesParent { get; }

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        GameObject SurfacesParent { get; }

        /// <summary>
        /// Gets or sets the provided mesh <see cref="SpatialMeshDisplayOptions"/> on all the <see cref="DetectedSpatialObservers"/>
        /// </summary>
        /// <remarks>
        /// Is is possible to override any previously set display options on any specific observers.
        /// </remarks>
        SpatialMeshDisplayOptions SpatialMeshVisibility { get; set; }

        #region Observers Utilities

        /// <summary>
        /// Indicates the current running state of the spatial awareness observer.
        /// </summary>
        bool IsObserverRunning(IMixedRealitySpatialAwarenessDataProvider observer);

        /// <summary>
        /// Generates a new unique observer id.<para/>
        /// </summary>
        /// <remarks>All <see cref="IMixedRealitySpatialAwarenessDataProvider"/>s are required to call this method in their initialization.</remarks>
        /// <returns>a new unique Id for the observer.</returns>
        uint GenerateNewObserverId();

        /// <summary>
        /// Starts / restarts the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to resume.</remarks>
        void StartObserver(IMixedRealitySpatialAwarenessDataProvider observer);

        /// <summary>
        /// Stops / pauses the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        void SuspendObserver(IMixedRealitySpatialAwarenessDataProvider observer);

        /// <summary>
        /// List of the spatial observers as detected by the spatial awareness system.
        /// </summary>
        HashSet<IMixedRealitySpatialAwarenessDataProvider> DetectedSpatialObservers { get; }

        #endregion Observer Utilities

        #region Observer Events

        /// <summary>
        /// Raise the event that a <see cref="IMixedRealitySpatialAwarenessDataProvider"/> has been detected.
        /// </summary>
        void RaiseSpatialAwarenessObserverDetected(IMixedRealitySpatialAwarenessDataProvider observer);

        /// <summary>
        /// Raise the event that a <see cref="IMixedRealitySpatialAwarenessDataProvider"/> has been lost.
        /// </summary>
        void RaiseSpatialAwarenessObserverLost(IMixedRealitySpatialAwarenessDataProvider observer);

        #endregion Observer Events

        #region Mesh Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject meshObject);

        #endregion Mesh Events

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceAdded"/> method to indicate a planar surface has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceAdded(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceUpdated(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceRemoved(IMixedRealitySpatialSurfaceObserver observer, Guid surfaceId);

        #endregion Surface Finding Events
    }
}
