// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation for all hand controller data providers. Takes care of all the platform agnostic
    /// hand tracking logic.
    /// </summary>
    public abstract class BaseHandControllerDataProvider : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        private readonly Dictionary<Handedness, DefaultHandController> trackedHandControllers = new Dictionary<Handedness, DefaultHandController>();

        private InputEventData<HandData> handDataUpdateEventData;
        private readonly List<IMixedRealityHandDataHandler> handDataUpdateEventHandlers = new List<IMixedRealityHandDataHandler>();

        private DefaultHandController leftHand;
        private readonly Dictionary<TrackedHandJoint, Transform> leftHandJointTransforms = new Dictionary<TrackedHandJoint, Transform>();
        private DefaultHandController rightHand;
        private readonly Dictionary<TrackedHandJoint, Transform> rightHandJointTransforms = new Dictionary<TrackedHandJoint, Transform>();

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Hand controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public BaseHandControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                handDataUpdateEventData = new InputEventData<HandData>(EventSystem.current);
            }
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

            foreach (IMixedRealityController detectedController in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                if (detectedController is DefaultHandController hand)
                {
                    if (detectedController.ControllerHandedness == Handedness.Left && leftHand == null)
                    {
                        leftHand = hand;
                    }
                    else if (detectedController.ControllerHandedness == Handedness.Right && rightHand == null)
                    {
                        rightHand = hand;
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

            UpdateHandData(Handedness.Left, OnGetHandData(Handedness.Left));
            UpdateHandData(Handedness.Right, OnGetHandData(Handedness.Right));
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

        private DefaultHandController GetOrAddHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out DefaultHandController controller))
            {
                return controller;
            }

            IMixedRealityPointer[] pointers = RequestPointers(typeof(DefaultHandController), handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            controller = System.Activator.CreateInstance(typeof(DefaultHandController), TrackingState.Tracked, handedness, inputSource, null) as DefaultHandController;

            if (controller == null || !controller.SetupConfiguration(typeof(DefaultHandController)))
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
                controller.TryRenderControllerModel(typeof(DefaultHandController));
            }

            trackedHandControllers.Add(handedness, controller);
            return controller;
        }

        private void RemoveHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out DefaultHandController controller))
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

        /// <inheritdoc />
        public void Register(IMixedRealityHandDataHandler handler)
        {
            if (handler != null)
            {
                handDataUpdateEventHandlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityHandDataHandler handler)
        {
            if (handDataUpdateEventHandlers.Contains(handler))
            {
                handDataUpdateEventHandlers.Remove(handler);
            }
        }

        /// <summary>
        /// Override to provide platofrm specific implementations for hand tracking.
        /// </summary>
        /// <param name="handedness">The handedness the udpated data is requested for.</param>
        /// <returns>Updated hand data for the current frame.</returns>
        protected HandData OnGetHandData(Handedness handedness)
        {
            return null;
        }

        /// <inheritdoc />
        public void UpdateHandData(Handedness handedness, HandData handData)
        {
            if (handData != null && handData.IsTracked)
            {
                DefaultHandController controller = GetOrAddHandController(handedness);
                if (controller != null)
                {
                    controller.UpdateState(handData);

                    handDataUpdateEventData.Initialize(controller.InputSource, handedness, MixedRealityInputAction.None, handData);
                    for (int i = 0; i < handDataUpdateEventHandlers.Count; i++)
                    {
                        IMixedRealityHandDataHandler handler = handDataUpdateEventHandlers[i];
                        handler.OnHandDataUpdated(handDataUpdateEventData);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to create {typeof(DefaultHandController).Name} controller");
                }
            }
            else
            {
                RemoveHandController(handedness);
            }
        }
    }
}
