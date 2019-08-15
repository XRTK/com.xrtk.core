// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    public class HandControllerDataProvider : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        private InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> jointPoseInputEventData;
        private InputEventData<HandMeshUpdatedEventData> handMeshInputEventData;
        private List<IMixedRealityHandJointHandler> handJointUpdatedEventListeners = new List<IMixedRealityHandJointHandler>();
        private List<IMixedRealityHandMeshHandler> handMeshUpdatedEventListeners = new List<IMixedRealityHandMeshHandler>();

        private IMixedRealityHandController leftHand;
        private IMixedRealityHandController rightHand;

        private Dictionary<TrackedHandJoint, Transform> leftHandFauxJoints = new Dictionary<TrackedHandJoint, Transform>();
        private Dictionary<TrackedHandJoint, Transform> rightHandFauxJoints = new Dictionary<TrackedHandJoint, Transform>();

        public IReadOnlyList<IMixedRealityHandJointHandler> HandJointUpdatedEventListeners => handJointUpdatedEventListeners;

        public IReadOnlyList<IMixedRealityHandMeshHandler> HandMeshUpdatedEventListeners => handMeshUpdatedEventListeners;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public HandControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            jointPoseInputEventData = new InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>>(EventSystem.current);
            handMeshInputEventData = new InputEventData<HandMeshUpdatedEventData>(EventSystem.current);
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            leftHand = null;
            rightHand = null;

            foreach (var detectedController in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                var hand = detectedController as IMixedRealityHandController;
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
                foreach (var fauxJoint in leftHandFauxJoints)
                {
                    if (leftHand.TryGetJoint(fauxJoint.Key, out MixedRealityPose pose))
                    {
                        fauxJoint.Value.SetPositionAndRotation(pose.Position, pose.Rotation);
                    }
                }
            }

            if (rightHand != null)
            {
                foreach (var fauxJoint in rightHandFauxJoints)
                {
                    if (rightHand.TryGetJoint(fauxJoint.Key, out MixedRealityPose pose))
                    {
                        fauxJoint.Value.SetPositionAndRotation(pose.Position, pose.Rotation);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // Check existence of fauxJoints before destroying. This avoids a (harmless) race
            // condition when the service is getting destroyed at the same time that the gameObjects
            // are being destroyed at shutdown.
            if (leftHandFauxJoints != null)
            {
                foreach (var fauxJoint in leftHandFauxJoints.Values)
                {
                    if (fauxJoint != null)
                    {
                        Object.Destroy(fauxJoint.gameObject);
                    }
                }
                leftHandFauxJoints.Clear();
            }

            if (rightHandFauxJoints != null)
            {
                foreach (var fauxJoint in rightHandFauxJoints.Values)
                {
                    if (fauxJoint != null)
                    {
                        Object.Destroy(fauxJoint.gameObject);
                    }
                }
                rightHandFauxJoints.Clear();
            }
        }

        #region IMixedRealityHandControllerDataProvider Implementation

        public void Register(GameObject listener)
        {
            IMixedRealityHandJointHandler handJointHandler = listener.GetComponent<IMixedRealityHandJointHandler>();
            if (handJointHandler != null)
            {
                handJointUpdatedEventListeners.Add(handJointHandler);
            }

            IMixedRealityHandMeshHandler handMeshHandler = listener.GetComponent<IMixedRealityHandMeshHandler>();
            if (handMeshHandler != null)
            {
                handMeshUpdatedEventListeners.Add(handMeshHandler);
            }
        }

        public void Unregister(GameObject listener)
        {
            IMixedRealityHandJointHandler handJointHandler = listener.GetComponent<IMixedRealityHandJointHandler>();
            if (handJointHandler != null && handJointUpdatedEventListeners.Contains(handJointHandler))
            {
                handJointUpdatedEventListeners.Remove(handJointHandler);
            }

            IMixedRealityHandMeshHandler handMeshHandler = listener.GetComponent<IMixedRealityHandMeshHandler>();
            if (handMeshHandler != null && handMeshUpdatedEventListeners.Contains(handMeshHandler))
            {
                handMeshUpdatedEventListeners.Remove(handMeshHandler);
            }
        }

        public void RaiseHandJointsUpdated(IMixedRealityInputSource source, Handedness handedness, IDictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            jointPoseInputEventData.Initialize(source, handedness, MixedRealityInputAction.None, jointPoses);
            for (int i = 0; i < HandJointUpdatedEventListeners.Count; i++)
            {
                HandJointUpdatedEventListeners[i].OnJointUpdated(jointPoseInputEventData);
            }
        }

        public void RaiseHandMeshUpdated(IMixedRealityInputSource source, Handedness handedness, HandMeshUpdatedEventData handMeshInfo)
        {
            handMeshInputEventData.Initialize(source, handedness, MixedRealityInputAction.None, handMeshInfo);
            for (int i = 0; i < HandMeshUpdatedEventListeners.Count; i++)
            {
                HandMeshUpdatedEventListeners[i].OnMeshUpdated(handMeshInputEventData);
            }
        }

        /// <inheritdoc />
        public Transform RequestJointTransform(TrackedHandJoint joint, Handedness handedness)
        {
            IMixedRealityHandController hand = null;
            Dictionary<TrackedHandJoint, Transform> fauxJoints;
            if (handedness == Handedness.Left)
            {
                hand = leftHand;
                fauxJoints = leftHandFauxJoints;
            }
            else if (handedness == Handedness.Right)
            {
                hand = rightHand;
                fauxJoints = rightHandFauxJoints;
            }
            else
            {
                return null;
            }

            Transform jointTransform = null;
            if (fauxJoints != null && !fauxJoints.TryGetValue(joint, out jointTransform))
            {
                jointTransform = new GameObject().transform;

                // Since this service survives scene loading and unloading, the fauxJoints it manages need to as well.
                Object.DontDestroyOnLoad(jointTransform.gameObject);
                jointTransform.name = string.Format("Joint Tracker: {1} {0}", joint, handedness);

                if (hand != null && hand.TryGetJoint(joint, out MixedRealityPose pose))
                {
                    jointTransform.SetPositionAndRotation(pose.Position, pose.Rotation);
                }

                fauxJoints.Add(joint, jointTransform);
            }

            return jointTransform;
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

        #endregion IMixedRealityHandControllerDataProvider Implementation
    }
}
