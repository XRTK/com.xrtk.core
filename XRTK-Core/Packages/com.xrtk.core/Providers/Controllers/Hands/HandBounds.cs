// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Utility behavior to access the axis aligned bounds of IMixedRealityHands.
    /// </summary>
    public class HandBounds : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
    {
        private Dictionary<Handedness, Bounds> bounds = new Dictionary<Handedness, Bounds>();

        /// <summary>
        /// Gets the bounds associated with a handedness.
        /// </summary>
        public IReadOnlyDictionary<Handedness, Bounds> Bounds => bounds;

        [SerializeField]
        [Tooltip("Should a gizmo be drawn to represent the hand bounds.")]
        private bool drawBoundsGizmo = false;

        /// <summary>
        /// Should a gizmo be drawn to represent the hand bounds.
        /// </summary>
        public bool DrawBoundsGizmo
        {
            get { return drawBoundsGizmo; }
            set { drawBoundsGizmo = value; }
        }

        #region MonoBehaviour Implementation

        private void OnEnable()
        {
            MixedRealityToolkit.InputSystem?.Register(gameObject);
        }

        private void OnDisable()
        {
            MixedRealityToolkit.InputSystem?.Unregister(gameObject);
        }

        private void OnDrawGizmos()
        {
            if (drawBoundsGizmo)
            {
                foreach (var kvp in bounds)
                {
                    Gizmos.DrawWireCube(kvp.Value.center, kvp.Value.size);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {

        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHandController;
            if (hand != null)
            {
                bounds.Remove(hand.ControllerHandedness);
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        #region IMixedRealityHandJointHandler Implementation

        /// <inheritdoc />
        public void OnJointUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            MixedRealityPose palmPose;

            if (eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out palmPose))
            {
                var newBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in eventData.InputData)
                {
                    if (kvp.Key == TrackedHandJoint.None ||
                        kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newBounds.Encapsulate(kvp.Value.Position);
                }

                bounds[eventData.Handedness] = newBounds;
            }
        }

        #endregion IMixedRealityHandJointHandler Implementation
    }
}
