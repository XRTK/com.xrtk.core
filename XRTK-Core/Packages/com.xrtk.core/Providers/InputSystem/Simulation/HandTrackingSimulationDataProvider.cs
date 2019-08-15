// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.InputSystem.Simulation;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;
using XRTK.Services;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class HandTrackingSimulationDataProvider : BaseControllerDataProvider, IHandTrackingSimulationDataProvider
    {
        private HandTrackingSimulationDataProviderProfile profile;
        private SimulatedHandDataProvider handDataProvider = null;
        private long lastHandControllerUpdateTimeStamp = 0;
        private readonly Dictionary<Handedness, SimulatedHand> trackedHandControllers = new Dictionary<Handedness, SimulatedHand>();
        private InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> jointPoseInputEventData;
        private InputEventData<HandMeshUpdatedEventData> handMeshInputEventData;

        public List<IMixedRealityHandJointHandler> HandJointUpdatedEventListeners { get; } = new List<IMixedRealityHandJointHandler>();

        public List<IMixedRealityHandMeshHandler> HandMeshUpdatedEventListeners { get; } = new List<IMixedRealityHandMeshHandler>();

        /// <summary>
        /// Gets left hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();

        /// <summary>
        /// Gets right hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; private set; } = true;

        /// <inheritdoc />
        public override IMixedRealityController[] GetActiveControllers() => trackedHandControllers.Values.ToArray();

        /// <summary>
        /// Dictionary to capture all active hands detected.
        /// </summary>
        public IReadOnlyDictionary<Handedness, SimulatedHand> TrackedHands => trackedHandControllers;

        public HandTrackingSimulationDataProvider(string name, uint priority, HandTrackingSimulationDataProviderProfile profile)
            : base(name, priority, profile)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            ArticulatedHandPose.LoadGesturePoses(profile.GestureDefinitions);

            jointPoseInputEventData = new InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>>(EventSystem.current);
            handMeshInputEventData = new InputEventData<HandMeshUpdatedEventData>(EventSystem.current);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (handDataProvider == null)
            {
                handDataProvider = new SimulatedHandDataProvider(profile);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            RemoveAllHandControllers();

            if (handDataProvider != null)
            {
                handDataProvider = null;
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (profile.SimulateHandTracking && UserInputEnabled)
            {
                handDataProvider.UpdateHandData(HandDataLeft, HandDataRight);
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (profile.SimulateHandTracking)
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandControllerUpdateTimeStamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    if (HandDataLeft.Timestamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Left, HandDataLeft);
                    }
                    if (HandDataRight.Timestamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Right, HandDataRight);
                    }

                    lastHandControllerUpdateTimeStamp = currentTime.Ticks;
                }
            }
        }

        public void Register(GameObject listener)
        {
            IMixedRealityHandJointHandler handJointHandler = listener.GetComponent<IMixedRealityHandJointHandler>();
            if (handJointHandler != null)
            {
                HandJointUpdatedEventListeners.Add(handJointHandler);
            }

            IMixedRealityHandMeshHandler handMeshHandler = listener.GetComponent<IMixedRealityHandMeshHandler>();
            if (handMeshHandler != null)
            {
                HandMeshUpdatedEventListeners.Add(handMeshHandler);
            }
        }

        public void Unregister(GameObject listener)
        {
            IMixedRealityHandJointHandler handJointHandler = listener.GetComponent<IMixedRealityHandJointHandler>();
            if (handJointHandler != null && HandJointUpdatedEventListeners.Contains(handJointHandler))
            {
                HandJointUpdatedEventListeners.Remove(handJointHandler);
            }

            IMixedRealityHandMeshHandler handMeshHandler = listener.GetComponent<IMixedRealityHandMeshHandler>();
            if (handMeshHandler != null && HandMeshUpdatedEventListeners.Contains(handMeshHandler))
            {
                HandMeshUpdatedEventListeners.Remove(handMeshHandler);
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

        // Register input sources for hands based on changes of the data provider
        private void UpdateHandInputSource(Handedness handedness, SimulatedHandData handData)
        {
            if (!profile.SimulateHandTracking)
            {
                RemoveAllHandControllers();
            }
            else
            {
                if (handData != null && handData.IsTracked)
                {
                    SimulatedHand controller = GetOrAddHandController(handedness);
                    if (controller != null)
                    {
                        controller.UpdateState(handData);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create {typeof(SimulatedArticulatedHand)} controller");
                    }
                }
                else
                {
                    RemoveHandController(handedness);
                }
            }
        }

        private SimulatedHand GetOrAddHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out SimulatedHand controller))
            {
                return controller;
            }

            IMixedRealityPointer[] pointers = RequestPointers(typeof(SimulatedArticulatedHand), handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);

            if (controller == null || !controller.SetupConfiguration(typeof(SimulatedArticulatedHand)))
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
                controller.TryRenderControllerModel(typeof(SimulatedArticulatedHand));
            }

            trackedHandControllers.Add(handedness, controller);
            return controller;
        }

        private void RemoveHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out SimulatedHand controller))
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
    }
}