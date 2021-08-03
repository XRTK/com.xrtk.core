// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
using XRTK.EventDatum.Input;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// Provides a base implementation for <see cref="ITeleportLocomotionProvider"/>s with functionality
    /// that is common for any type of teleport locomotion provider.
    /// </summary>
    public abstract class BaseTeleportLocomotionProvider : BaseLocomotionProvider, ITeleportLocomotionProvider
    {
        /// <inheritdoc />
        public BaseTeleportLocomotionProvider(string name, uint priority, BaseTeleportLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            inputThreshold = profile.InputThreshold;
            teleportActivationAngle = profile.TeleportActivationAngle;
            angleOffset = profile.AngleOffset;
            rotateActivationAngle = profile.RotateActivationAngle;
            rotationAmount = profile.RotationAmount;
            backStrafeActivationAngle = profile.BackStrafeActivationAngle;
            strafeAmount = profile.StrafeAmount;
        }

        private float inputThreshold;
        private float teleportActivationAngle;
        private float angleOffset;
        private float rotateActivationAngle;
        private float rotationAmount;
        private float backStrafeActivationAngle;
        private float strafeAmount;

        /// <summary>
        /// This registry keeps track of in progress teleport target requests.
        /// Each input source may request teleportation and thus a teleportation target only once
        /// at a time.
        /// </summary>
        protected Dictionary<uint, IMixedRealityInputSource> TargetRequestsDict { get; } = new Dictionary<uint, IMixedRealityInputSource>();

        /// <inheritdoc />
        public bool IsTeleporting { get; protected set; }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            // Is this the input action this provider is configured to look out for?
            // And did we already request a teleport target for the input source that raised it?
            if (!eventData.used && eventData.MixedRealityInputAction == InputAction &&
                !TargetRequestsDict.ContainsKey(eventData.SourceId))
            {
                TargetRequestsDict.Add(eventData.SourceId, eventData.InputSource);
                LocomotionSystem.RaiseTeleportTargetRequest(this, eventData.InputSource);
                Debug.Log($"{GetType().Name} - Requested teleport target from input source {eventData.SourceId} by digital input.");
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);

            if (eventData.MixedRealityInputAction == InputAction &&
                TargetRequestsDict.ContainsKey(eventData.SourceId))
            {
                TargetRequestsDict.Remove(eventData.SourceId);
            }
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<float> eventData)
        {
            base.OnInputChanged(eventData);

            // Is this the input action this provider is configured to look out for?
            if (!eventData.used && eventData.MixedRealityInputAction == InputAction)
            {
                var singleAxisPosition = eventData.InputData;

                // Depending on the input position we either raise a new request
                // for a teleportation target or we cancel an existing
                // request for the input source, if any.
                if (singleAxisPosition > inputThreshold &&
                    !TargetRequestsDict.ContainsKey(eventData.SourceId))
                {
                    TargetRequestsDict.Add(eventData.SourceId, eventData.InputSource);
                    LocomotionSystem.RaiseTeleportTargetRequest(this, eventData.InputSource);
                    Debug.Log($"{GetType().Name} - Requested teleport target from input source {eventData.SourceId} by single axis input.");
                }
                else if (TargetRequestsDict.ContainsKey(eventData.SourceId))
                {
                    TargetRequestsDict.Remove(eventData.SourceId);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            base.OnInputChanged(eventData);

            // Is this the input action this provider is configured to look out for?
            if (!eventData.used && eventData.MixedRealityInputAction == InputAction)
            {
                var dualAxisPosition = eventData.InputData;

                if (Mathf.Abs(dualAxisPosition.y) > inputThreshold ||
                    Mathf.Abs(dualAxisPosition.x) > inputThreshold)
                {
                    // Get the angle of the dual axis input
                    var angle = Mathf.Atan2(dualAxisPosition.x, dualAxisPosition.y) * Mathf.Rad2Deg;

                    // Offset the angle so it's 'forward' facing
                    angle += angleOffset;

                    // Depending on the angle we either raise a new request
                    // for a teleportation target or we cancel an existing
                    // request for the input source, if any.
                    if (Mathf.Abs(angle) < teleportActivationAngle &&
                        !TargetRequestsDict.ContainsKey(eventData.SourceId))
                    {
                        TargetRequestsDict.Add(eventData.SourceId, eventData.InputSource);
                        LocomotionSystem.RaiseTeleportTargetRequest(this, eventData.InputSource);
                        Debug.Log($"{GetType().Name} - Requested teleport target from input source {eventData.SourceId} by dual axis input.");
                    }
                    else if (TargetRequestsDict.ContainsKey(eventData.SourceId))
                    {
                        TargetRequestsDict.Remove(eventData.SourceId);
                    }
                }
            }
        }
    }
}
