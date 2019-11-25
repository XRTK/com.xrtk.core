// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Utilities;
using XRTK.Utilities;
using UnityEngine;
using XRTK.Services;

namespace XRTK.SDK.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    public class Orbital : Solver
    {
        [SerializeField]
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        private SolverOrientationType orientationType = SolverOrientationType.FollowTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        /// <remarks>
        /// Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.
        /// </remarks>
        public SolverOrientationType OrientationType
        {
            get => orientationType;
            set => orientationType = value;
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.")]
        private Vector3 localOffset = new Vector3(0, -1, 1);

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 LocalOffset
        {
            get => localOffset;
            set => localOffset = value;
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientationType. Mixing local and world offsets is not recommended.")]
        private Vector3 worldOffset = Vector3.zero;

        /// <summary>
        /// XYZ offset for this object in worldspace, best used with the YawOnly orientationType.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 WorldOffset
        {
            get => worldOffset;
            set => worldOffset = value;
        }

        [SerializeField]
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleSteppingForWorldOffset = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleSteppingForWorldOffset
        {
            get => useAngleSteppingForWorldOffset;
            set => useAngleSteppingForWorldOffset = value;
        }

        [Range(2, 24)]
        [SerializeField]
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapple steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapple steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get => tetherAngleSteps;
            set => tetherAngleSteps = Mathf.Clamp(value, 2, 24);
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            var desiredPos = SolverHandler.TransformTarget != null
                ? SolverHandler.TransformTarget.position
                : Vector3.zero;
            var targetRot = SolverHandler.TransformTarget != null
                ? SolverHandler.TransformTarget.rotation
                : Quaternion.Euler(0, 1, 0);
            var yawOnlyRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
            desiredPos += (SnapToTetherAngleSteps(targetRot) * LocalOffset);
            desiredPos += (SnapToTetherAngleSteps(yawOnlyRot) * WorldOffset);

            var desiredRot = CalculateDesiredRotation(desiredPos);

            GoalPosition = desiredPos;
            GoalRotation = desiredRot;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        private Quaternion SnapToTetherAngleSteps(Quaternion rotationToSnap)
        {
            if (!UseAngleSteppingForWorldOffset || SolverHandler.TransformTarget == null)
            {
                return rotationToSnap;
            }

            var stepAngle = 360f / tetherAngleSteps;
            var numberOfSteps = Mathf.RoundToInt(SolverHandler.TransformTarget.transform.eulerAngles.y / stepAngle);
            var newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(rotationToSnap.eulerAngles.x, newAngle, rotationToSnap.eulerAngles.z);
        }

        private Quaternion CalculateDesiredRotation(Vector3 desiredPos)
        {
            var desiredRot = Quaternion.identity;
            var cameraTransform = MixedRealityToolkit.CameraSystem == null
                ? CameraCache.Main.transform
                : MixedRealityToolkit.CameraSystem.CameraRig.CameraTransform;

            switch (orientationType)
            {
                case SolverOrientationType.YawOnly:
                    var targetYRotation = SolverHandler.TransformTarget != null
                        ? SolverHandler.TransformTarget.eulerAngles.y
                        : 0.0f;
                    desiredRot = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case SolverOrientationType.Unmodified:
                    desiredRot = transform.rotation;
                    break;
                case SolverOrientationType.CameraAligned:
                    desiredRot = cameraTransform.rotation;
                    break;
                case SolverOrientationType.FaceTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null
                        ? Quaternion.LookRotation(SolverHandler.TransformTarget.position - desiredPos)
                        : Quaternion.identity;
                    break;
                case SolverOrientationType.CameraFacing:
                    desiredRot = SolverHandler.TransformTarget != null
                        ? Quaternion.LookRotation(cameraTransform.position - desiredPos)
                        : Quaternion.identity;
                    break;
                case SolverOrientationType.FollowTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null
                        ? SolverHandler.TransformTarget.rotation
                        : Quaternion.identity;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }

            if (UseAngleSteppingForWorldOffset)
            {
                desiredRot = SnapToTetherAngleSteps(desiredRot);
            }

            return desiredRot;
        }
    }
}