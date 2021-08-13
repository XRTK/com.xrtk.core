// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
using XRTK.EventDatum.Input;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// Provides a base implementation for <see cref="ITeleportLocomotionProvider"/>s with functionality
    /// that is common for any type of teleport locomotion provider.
    /// </summary>
    public abstract class BaseTeleportLocomotionProvider : BaseLocomotionProvider, ITeleportLocomotionProvider
    {
        /// <inheritdoc />
        public BaseTeleportLocomotionProvider(string name, uint priority, BaseTeleportLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            inputThreshold = profile.InputThreshold;
            teleportActivationAngle = profile.TeleportActivationAngle;
            angleOffset = profile.AngleOffset;
            rotateActivationAngle = profile.RotateActivationAngle;
            rotationAmount = profile.RotationAmount;
        }

        private readonly Dictionary<uint, bool> inputPreviouslyDownDict = new Dictionary<uint, bool>();
        private readonly float inputThreshold;
        private readonly float teleportActivationAngle;
        private readonly float angleOffset;
        private readonly float rotateActivationAngle;
        private readonly float rotationAmount;
        private bool canRotate;

        /// <summary>
        /// This registry keeps track of in progress teleport target requests.
        /// Each input source may request teleportation and thus a teleportation target only once
        /// at a time.
        /// </summary>
        /// <remarks>
        /// Key is an input source ID.
        /// Value is the input source itself.
        /// </remarks>
        protected Dictionary<uint, IMixedRealityInputSource> OpenTargetRequests { get; } = new Dictionary<uint, IMixedRealityInputSource>();

        /// <summary>
        /// This registry keeps track of <see cref="ITeleportTargetProvider"/>s that have answered
        /// our request for a teleportation target. Once the teleport input action has been released, we can query
        /// the provider for a target location.
        /// </summary>
        /// <remarks>
        /// Key is an input source ID.
        /// Value is the target provider that provides targets for that input source ID.
        /// </remarks>
        protected Dictionary<uint, ITeleportTargetProvider> AvailableTargetProviders { get; } = new Dictionary<uint, ITeleportTargetProvider>();

        /// <inheritdoc />
        public bool IsTeleporting { get; protected set; }

        /// <inheritdoc />
        public override void Disable()
        {
            // When being disabled, cancel any in progress teleport.
            foreach (var openRequest in OpenTargetRequests)
            {
                LocomotionSystem.RaiseTeleportCanceled(this, openRequest.Value);
            }

            AvailableTargetProviders.Clear();
            base.Disable();
        }

        /// <inheritdoc />
        public void SetTargetProvider(ITeleportTargetProvider teleportTargetProvider)
        {
            if (!AvailableTargetProviders.ContainsKey(teleportTargetProvider.InputSource.SourceId))
            {
                AvailableTargetProviders.Add(teleportTargetProvider.InputSource.SourceId, teleportTargetProvider);
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            // Is this the input action this provider is configured to look out for?
            // And did we already request a teleport target for the input source that raised it?
            if (eventData.MixedRealityInputAction != InputAction ||
                OpenTargetRequests.ContainsKey(eventData.SourceId))
            {
                return;
            }

            RaiseTeleportTargetRequest(eventData.InputSource);

            base.OnInputDown(eventData);
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            // Has our configured teleport input action been released
            // and we have an open target request for the input source?
            if (eventData.MixedRealityInputAction == InputAction &&
                OpenTargetRequests.ContainsKey(eventData.SourceId))
            {
                var inputSource = OpenTargetRequests[eventData.SourceId];
                ProcessTeleportRequest(inputSource);
            }

            base.OnInputUp(eventData);
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<float> eventData)
        {
            // Is this the input action this provider is configured to look out for?
            if (eventData.MixedRealityInputAction == InputAction)
            {
                // Depending on the input position we either raise a new request
                // for a teleportation target or we start/cancel an existing
                // request for the input source, if any.
                var singleAxisPosition = eventData.InputData;
                if (singleAxisPosition > inputThreshold &&
                    !WasInputPreviouslyDown(eventData.SourceId) &&
                    !OpenTargetRequests.ContainsKey(eventData.SourceId))
                {
                    // This is a new target request as input was pressed and we have no open
                    // request yet.
                    inputPreviouslyDownDict[eventData.SourceId] = true;
                    RaiseTeleportTargetRequest(eventData.InputSource);
                }
                else if (singleAxisPosition < inputThreshold &&
                    WasInputPreviouslyDown(eventData.SourceId) &&
                    OpenTargetRequests.ContainsKey(eventData.SourceId))
                {
                    // Input was relased and we have an open target request we can process now.
                    inputPreviouslyDownDict[eventData.SourceId] = false;
                    var inputSource = OpenTargetRequests[eventData.SourceId];
                    ProcessTeleportRequest(inputSource);
                }
            }

            base.OnInputChanged(eventData);
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            // Is this the input action this provider is configured to look out for?
            if (eventData.MixedRealityInputAction == InputAction)
            {
                // Depending on the input position we either raise a new request
                // for a teleportation target or we start/cancel an existing
                // request for the input source, if any.
                var dualAxisPosition = eventData.InputData;
                if (Mathf.Abs(dualAxisPosition.y) > inputThreshold ||
                    Mathf.Abs(dualAxisPosition.x) > inputThreshold)
                {
                    // Get the angle of the dual axis input.
                    var angle = Mathf.Atan2(dualAxisPosition.x, dualAxisPosition.y) * Mathf.Rad2Deg;

                    // Offset the angle so it's 'forward' facing.
                    angle += angleOffset;

                    var absoluteAngle = Mathf.Abs(angle);
                    if (absoluteAngle < teleportActivationAngle &&
                        !WasInputPreviouslyDown(eventData.SourceId) &&
                        !OpenTargetRequests.ContainsKey(eventData.SourceId))
                    {
                        // This is a new target request as input was pressed and we have no open
                        // request yet.
                        inputPreviouslyDownDict[eventData.SourceId] = true;
                        RaiseTeleportTargetRequest(eventData.InputSource);
                    }
                    else if (canRotate)
                    {
                        // wrap the angle value.
                        if (absoluteAngle > 180f)
                        {
                            absoluteAngle = Mathf.Abs(absoluteAngle - 360f);
                        }

                        // Calculate the offset rotation angle from the 90 degree mark.
                        // Half the rotation activation angle amount to make sure the activation angle stays centered at 90.
                        float offsetRotationAngle = 90f - rotateActivationAngle;

                        // subtract it from our current angle reading
                        offsetRotationAngle = absoluteAngle - offsetRotationAngle;

                        // if it's less than zero, then we don't have activation
                        // check to make sure we're still under our activation threshold.
                        if (offsetRotationAngle > 0 && offsetRotationAngle < rotateActivationAngle)
                        {
                            canRotate = false;
                            // Rotate the camera by the rotation amount. If our angle is positive then rotate in the positive direction, otherwise in the opposite direction.
                            LocomotionTargetTransform.RotateAround(CameraTransform.position, Vector3.up, angle >= 0.0f ? rotationAmount : -rotationAmount);
                        }
                    }
                }
                else if (Mathf.Abs(dualAxisPosition.y) < inputThreshold &&
                    Mathf.Abs(dualAxisPosition.x) < inputThreshold &&
                    WasInputPreviouslyDown(eventData.SourceId) &&
                    OpenTargetRequests.ContainsKey(eventData.SourceId))
                {
                    // Input was relased and we have an open target request we can process now.
                    inputPreviouslyDownDict[eventData.SourceId] = false;
                    var inputSource = OpenTargetRequests[eventData.SourceId];
                    ProcessTeleportRequest(inputSource);
                }
                else if (!OpenTargetRequests.ContainsKey(eventData.SourceId) && !IsTeleporting)
                {
                    canRotate = true;
                }
            }

            base.OnInputChanged(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportStarted(LocomotionEventData eventData)
        {
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                IsTeleporting = true;
            }

            base.OnTeleportStarted(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCanceled(LocomotionEventData eventData)
        {
            CleanUpTeleportRequest(eventData.EventSource.SourceId);
            base.OnTeleportCanceled(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCompleted(LocomotionEventData eventData)
        {
            CleanUpTeleportRequest(eventData.EventSource.SourceId);
            base.OnTeleportCompleted(eventData);
        }

        private void RaiseTeleportTargetRequest(IMixedRealityInputSource inputSource)
        {
            OpenTargetRequests.Add(inputSource.SourceId, inputSource);
            LocomotionSystem.RaiseTeleportTargetRequest(this, inputSource);
        }

        private void ProcessTeleportRequest(IMixedRealityInputSource inputSource)
        {
            // Is a target provider available for the input source?
            if (AvailableTargetProviders.ContainsKey(inputSource.SourceId))
            {
                // We have a target provider that anwered our previous
                // target request. Check if the provider has a valid teleportation
                // target for us and start teleport, cancel everything otherwise.
                var teleportTargetProvider = AvailableTargetProviders[inputSource.SourceId];
                if (teleportTargetProvider.TargetPose.HasValue)
                {
                    // We have a valid teleport target, we can start teleportation.
                    LocomotionSystem.RaiseTeleportStarted(this, inputSource, teleportTargetProvider.TargetPose.Value, teleportTargetProvider.HotSpot);
                }
                else
                {
                    // Input was released without a valid teleport target, cancel teleport.
                    LocomotionSystem.RaiseTeleportCanceled(this, inputSource);
                }
            }
            else
            {
                // Input was released but no target provider has answered our target request,
                // since teleport was never started, we do not cancel but simply forget about the open request.
                CleanUpTeleportRequest(inputSource.SourceId);
            }
        }

        private void CleanUpTeleportRequest(uint inputSourceId)
        {
            if (OpenTargetRequests.ContainsKey(inputSourceId))
            {
                OpenTargetRequests.Remove(inputSourceId);
                IsTeleporting = false;
            }

            if (AvailableTargetProviders.ContainsKey(inputSourceId))
            {
                AvailableTargetProviders.Remove(inputSourceId);
            }
        }

        private bool WasInputPreviouslyDown(uint inputSourceId)
        {
            if (inputPreviouslyDownDict.TryGetValue(inputSourceId, out var wasDown))
            {
                return wasDown;
            }

            inputPreviouslyDownDict.Add(inputSourceId, false);
            return false;
        }

        //public override void OnInputChanged(InputEventData<Vector2> eventData)
        //{
        //    if (RequestingLocomotionProvider != null &&
        //        eventData.SourceId == InputSource.SourceId &&
        //        eventData.Handedness == Handedness &&
        //        eventData.MixedRealityInputAction == RequestingLocomotionProvider.InputAction)
        //    {
        //        currentDualAxisInputPosition = eventData.InputData;

        //        if (Mathf.Abs(currentDualAxisInputPosition.y) > inputThreshold ||
        //            Mathf.Abs(currentDualAxisInputPosition.x) > inputThreshold)
        //        {
        //            ...
        //        }
        //        else
        //        {
        //            if (!canTeleport && !teleportEnabled)
        //            {
        //                // Reset the move flag when the user stops moving the joystick
        //                // but hasn't yet started teleport request.
        //                canRotate = true;
        //            }
        //        }
        //    }
        //}
    }
}
