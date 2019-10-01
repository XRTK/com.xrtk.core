// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    public abstract class BaseHandControllerDataProvider : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        private HandControllerDataProviderProfile profile;
        private readonly Dictionary<Handedness, BaseHandController> trackedHandControllers = new Dictionary<Handedness, BaseHandController>();

        private InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> jointPoseInputEventData;
        private InputEventData<HandMeshUpdatedEventData> handMeshInputEventData;
        private List<IMixedRealityHandJointHandler> handJointUpdatedEventListeners = new List<IMixedRealityHandJointHandler>();
        private List<IMixedRealityHandMeshHandler> handMeshUpdatedEventListeners = new List<IMixedRealityHandMeshHandler>();

        private BaseHandController leftHand;
        private Dictionary<TrackedHandJoint, Transform> leftHandJointTransforms = new Dictionary<TrackedHandJoint, Transform>();
        private BaseHandController rightHand;
        private Dictionary<TrackedHandJoint, Transform> rightHandJointTransforms = new Dictionary<TrackedHandJoint, Transform>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public BaseHandControllerDataProvider(string name, uint priority, HandControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            jointPoseInputEventData = new InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>>(EventSystem.current);
            handMeshInputEventData = new InputEventData<HandMeshUpdatedEventData>(EventSystem.current);

            //ArticulatedHandPose.LoadGesturePoses(profile.GestureDefinitions);
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // Check existence of fauxJoints before destroying. This avoids a (harmless) race
            // condition when the service is getting destroyed at the same time that the gameObjects
            // are being destroyed at shutdown.
            if (leftHandJointTransforms != null)
            {
                foreach (var fauxJoint in leftHandJointTransforms.Values)
                {
                    if (fauxJoint != null)
                    {
                        Object.Destroy(fauxJoint.gameObject);
                    }
                }

                leftHandJointTransforms.Clear();
            }

            if (rightHandJointTransforms != null)
            {
                foreach (var fauxJoint in rightHandJointTransforms.Values)
                {
                    if (fauxJoint != null)
                    {
                        Object.Destroy(fauxJoint.gameObject);
                    }
                }

                rightHandJointTransforms.Clear();
            }

            RemoveAllHandControllers();
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            leftHand = null;
            rightHand = null;

            foreach (var detectedController in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                var hand = detectedController as BaseHandController;
                if (hand != null)
                {
                    if (detectedController.ControllerHandedness == Handedness.Left)
                    {
                        if (leftHand == null)
                        {
                            leftHand = hand;
                        }
                    }
                    else if (detectedController.ControllerHandedness == Handedness.Right)
                    {
                        if (rightHand == null)
                        {
                            rightHand = hand;
                        }
                    }
                }
            }

            if (leftHand != null)
            {
                foreach (var fauxJoint in leftHandJointTransforms)
                {
                    if (leftHand.TryGetJointPose(fauxJoint.Key, out MixedRealityPose pose))
                    {
                        fauxJoint.Value.SetPositionAndRotation(pose.Position, pose.Rotation);
                    }
                }
            }

            if (rightHand != null)
            {
                foreach (var fauxJoint in rightHandJointTransforms)
                {
                    if (rightHand.TryGetJointPose(fauxJoint.Key, out MixedRealityPose pose))
                    {
                        fauxJoint.Value.SetPositionAndRotation(pose.Position, pose.Rotation);
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool TryGetJointTransform(TrackedHandJoint joint, Handedness handedness, out Transform jointTransform)
        {
            Dictionary<TrackedHandJoint, Transform> fauxJoints;
            IMixedRealityHandController hand;

            if (handedness == Handedness.Left)
            {
                hand = leftHand;
                fauxJoints = leftHandJointTransforms;
            }
            else if (handedness == Handedness.Right)
            {
                hand = rightHand;
                fauxJoints = rightHandJointTransforms;
            }
            else
            {
                jointTransform = null;
                return false;
            }

            if (fauxJoints != null && fauxJoints.TryGetValue(joint, out Transform existingJointTransform))
            {
                jointTransform = existingJointTransform;
                return true;
            }

            Transform newJointTransform = new GameObject().transform;
            newJointTransform.name = $"Joint Tracker: {joint} {handedness}";

            // Since this service survives scene loading and unloading, the fauxJoints it manages need to as well.
            Object.DontDestroyOnLoad(newJointTransform.gameObject);

            if (hand != null && hand.TryGetJointPose(joint, out MixedRealityPose pose))
            {
                newJointTransform.SetPositionAndRotation(pose.Position, pose.Rotation);
            }

            fauxJoints.Add(joint, newJointTransform);
            jointTransform = newJointTransform;
            return true;
        }

        /// <inheritdoc />
        public bool IsHandTracked(Handedness handedness)
        {
            switch (handedness)
            {
                case Handedness.None:
                    return leftHand == null && rightHand == null;
                case Handedness.Left:
                    return leftHand != null;
                case Handedness.Right:
                    return rightHand != null;
                case Handedness.Both:
                    return leftHand != null && rightHand != null;
                case Handedness.Any:
                    return leftHand != null || rightHand != null;
                case Handedness.Other:
                default:
                    return false;
            }
        }

        #region Hand Controller Management

        private BaseHandController GetOrAddHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out BaseHandController controller))
            {
                return controller;
            }

            IMixedRealityPointer[] pointers = RequestPointers(profile.HandControllerType.Type, handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            controller = System.Activator.CreateInstance(profile.HandControllerType.Type, TrackingState.Tracked, handedness, inputSource, null) as BaseHandController;

            if (controller == null || !controller.SetupConfiguration(profile.HandControllerType.Type))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            MixedRealityToolkit.InputSystem.RaiseSourceDetected(controller.InputSource, controller);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers)
            {
                controller.TryRenderControllerModel(profile.HandControllerType.Type);
            }

            trackedHandControllers.Add(handedness, controller);
            return controller;
        }

        private void RemoveHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out BaseHandController controller))
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
                trackedHandControllers.Remove(handedness);
            }
        }

        private void RemoveAllHandControllers()
        {
            foreach (var controller in trackedHandControllers.Values)
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
            }

            trackedHandControllers.Clear();
        }

        #endregion

        #region Handler registration and data updates

        /// <inheritdoc />
        public void Register(IMixedRealityHandJointHandler handler)
        {
            if (handler != null)
            {
                handJointUpdatedEventListeners.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityHandJointHandler handler)
        {
            if (handJointUpdatedEventListeners.Contains(handler))
            {
                handJointUpdatedEventListeners.Remove(handler);
            }
        }

        /// <inheritdoc />
        public void Register(IMixedRealityHandMeshHandler handler)
        {
            if (handler != null)
            {
                handMeshUpdatedEventListeners.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityHandMeshHandler handler)
        {
            if (handMeshUpdatedEventListeners.Contains(handler))
            {
                handMeshUpdatedEventListeners.Remove(handler);
            }
        }

        private void HandControllerDataProvider_OnHandDataUpdate(Handedness handedness, HandData handData)
        {
            if (handData != null && handData.IsTracked)
            {
                BaseHandController controller = GetOrAddHandController(handedness);
                if (controller != null)
                {
                    controller.UpdateState(handData);

                    //IMixedRealityInputSource source

                    //IDictionary<TrackedHandJoint, MixedRealityPose> jointPoses
                    //jointPoseInputEventData.Initialize(source, handedness, MixedRealityInputAction.None, jointPoses);
                    //for (int i = 0; i < HandJointUpdatedEventHandlers.Count; i++)
                    //{
                    //    HandJointUpdatedEventHandlers[i].OnJointUpdated(jointPoseInputEventData);
                    //}

                    //HandMeshUpdatedEventData handMeshInfo
                    //handMeshInputEventData.Initialize(source, handedness, MixedRealityInputAction.None, handMeshInfo);
                    //for (int i = 0; i < HandMeshUpdatedEventHandlers.Count; i++)
                    //{
                    //    HandMeshUpdatedEventHandlers[i].OnMeshUpdated(handMeshInputEventData);
                    //}
                }
                else
                {
                    Debug.LogError($"Failed to create {profile.HandControllerType} controller");
                }
            }
            else
            {
                RemoveHandController(handedness);
            }
        }

        #endregion
    }
}
