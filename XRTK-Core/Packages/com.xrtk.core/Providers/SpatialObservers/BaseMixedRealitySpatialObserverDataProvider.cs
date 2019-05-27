// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Services;

namespace XRTK.Providers.SpatialObservers
{
    /// <summary>
    /// Base <see cref="IMixedRealitySpatialObserverDataProvider"/> implementation
    /// </summary>
    public abstract class BaseMixedRealitySpatialObserverDataProvider : BaseDataProvider, IMixedRealitySpatialObserverDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseMixedRealitySpatialObserverDataProvider(string name, uint priority, BaseMixedRealitySpatialObserverProfile profile)
            : base(name, priority)
        {
            if (profile == null)
            {
                throw new ArgumentNullException($"Missing profile for {name}");
            }

            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewObserverId();
            StartupBehavior = profile.StartupBehavior;
            UpdateInterval = profile.UpdateInterval;
            PhysicsLayer = profile.PhysicsLayer;
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverDetected(this);

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                StartObserving();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            StopObserving();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverLost(this);
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialObserverDataProvider Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        /// <inheritdoc />
        public virtual int PhysicsLayer { get; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        public virtual void StartObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = true;
        }

        /// <inheritdoc />
        public virtual void StopObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = false;
        }

        #endregion IMixedRealitySpatialObserverDataProvider Implementation

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; }

        #endregion IMixedRealityEventSource Implementation

        #region IEquality Implementation

        /// <summary>
        /// Determines if the specified objects are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(IMixedRealitySpatialObserverDataProvider left, IMixedRealitySpatialObserverDataProvider right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialObserverDataProvider)obj);
        }

        private bool Equals(IMixedRealitySpatialObserverDataProvider other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        /// <summary>
        /// Internal component to monitor the spatial observer's anchored transform, apply the MixedRealityPlayspace transform, and apply it to its parent.
        /// </summary>
        protected class PlayspaceAnchorAdapter : MonoBehaviour
        {
            /// <summary>
            /// Compute and set the parent's transform.
            /// </summary>
            private void Update()
            {
                var playspacePosition = MixedRealityToolkit.Instance.MixedRealityPlayspace.position;
                var playspaceRotation = MixedRealityToolkit.Instance.MixedRealityPlayspace.rotation;
                var anchorTransform = transform;
                var anchorPosition = anchorTransform.position;
                var anchorRotation = anchorTransform.rotation;
                var anchorParent = anchorTransform.parent;

                anchorParent.position = playspacePosition + playspaceRotation * anchorPosition;
                anchorParent.rotation = playspaceRotation * anchorRotation;
            }
        }
    }
}