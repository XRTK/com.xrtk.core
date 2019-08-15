// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Services.InputSystem.Simulation
{
    public class SimulatedArticulatedHand : BaseHandController
    {
        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose lastPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastGripPose = MixedRealityPose.ZeroIdentity;

        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private IMixedRealityPlatformHandControllerDataProvider dataProvider;
        private IMixedRealityPlatformHandControllerDataProvider DataProvider => dataProvider ?? (dataProvider = MixedRealityToolkit.GetService<IMixedRealityPlatformHandControllerDataProvider>());

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public SimulatedArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
        };

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        public void UpdateState(SimulatedHandData handData)
        {
            for (int i = 0; i < jointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;

                if (!jointPoses.ContainsKey(handJoint))
                {
                    jointPoses.Add(handJoint, handData.Joints[i]);
                }
                else
                {
                    jointPoses[handJoint] = handData.Joints[i];
                }
            }

            //DataProvider.RaiseHandJointsUpdated(InputSource, ControllerHandedness, jointPoses);

            UpdateVelocity();
            UpdateInteractions(handData);
        }

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private void UpdateInteractions(SimulatedHandData handData)
        {
            lastPointerPose = currentPointerPose;
            lastGripPose = currentGripPose;

            // For convenience of simulating in Unity Editor, make the ray use the index
            // finger position instead of knuckle, since the index finger doesn't move when we press.
            Vector3 pointerPosition = jointPoses[TrackedHandJoint.IndexTip].Position;
            IsPositionAvailable = IsRotationAvailable = pointerPosition != Vector3.zero;

            if (IsPositionAvailable)
            {
                //HandRay.Update(pointerPosition, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);

                Ray ray = HandRay.Ray;

                currentPointerPose.Position = ray.origin;
                currentPointerPose.Rotation = Quaternion.LookRotation(ray.direction);

                currentGripPose = jointPoses[TrackedHandJoint.Palm];

                currentIndexPose = jointPoses[TrackedHandJoint.IndexTip];
            }

            if (lastGripPose != currentGripPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem.RaiseSourcePoseChanged(InputSource, this, currentGripPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem.RaiseSourcePositionChanged(InputSource, this, currentPointerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem.RaiseSourceRotationChanged(InputSource, this, currentPointerRotation);
                }
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = currentPointerPose;
                        if (Interactions[i].Changed)
                        {
                            MixedRealityToolkit.InputSystem.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentGripPose;
                        if (Interactions[i].Changed)
                        {
                            MixedRealityToolkit.InputSystem.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = handData.IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                MixedRealityToolkit.InputSystem.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                MixedRealityToolkit.InputSystem.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.TriggerPress:
                        Interactions[i].BoolData = handData.IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                MixedRealityToolkit.InputSystem.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                MixedRealityToolkit.InputSystem.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.IndexFinger:
                        Interactions[i].PoseData = currentIndexPose;
                        if (Interactions[i].Changed)
                        {
                            MixedRealityToolkit.InputSystem.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentIndexPose);
                        }
                        break;
                }
            }
        }
    }
}