// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Interfaces
{
    /// <summary>
    /// Generic interface for all Mixed Reality Services
    /// </summary>
    public interface IMixedRealityService : IDisposable
    {
        /// <summary>
        /// The service display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Optional Priority to reorder registered managers based on their respective priority, reduces the risk of race conditions by prioritizing the order in which services are evaluated.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The initialize function is used to setup the service once created.
        /// This method is called once all services have been registered in the Mixed Reality Toolkit.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Optional Reset function to perform that will Reset the service, for example, whenever there is a profile change.
        /// </summary>
        void Reset();

        /// <summary>
        /// Optional Enable function to enable / re-enable the service.
        /// </summary>
        void Enable();

        /// <summary>
        /// Optional Update function to perform per-frame updates of the service.
        /// </summary>
        void Update();

        /// <summary>
        /// Optional LateUpdate function to perform updates after the end of the last Update cycle.
        /// </summary>
        void LateUpdate();

        /// <summary>
        /// Optional FixedUpdate function to perform updates that are fixed to the Physics timestep.
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Optional Disable function to pause the service.
        /// </summary>
        void Disable();

        /// <summary>
        /// Optional Destroy function to perform cleanup of the service before the Mixed Reality Toolkit is destroyed.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Optional function that is called when the application gains or looses focus.
        /// </summary>
        /// <param name="isFocused"></param>
        void OnApplicationFocus(bool isFocused);

        /// <summary>
        /// Optional function that is called when the application is paused or un-paused.
        /// </summary>
        /// <param name="isPaused"></param>
        void OnApplicationPause(bool isPaused);
    }
}
