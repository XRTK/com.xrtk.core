// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Extensions;

namespace XRTK.SDK.Utilities.Solvers
{
    /// <summary>
    /// The base abstract class for all Solvers to derive from. It provides state tracking, smoothing parameters
    /// and implementation, automatic solver system integration, and update order. Solvers may be used without a link,
    /// as long as updateLinkedTransform is false.
    /// </summary>
    [RequireComponent(typeof(SolverHandler))]
    public abstract class Solver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        private bool updateLinkedTransform = false;

        [SerializeField]
        [Tooltip("Position lerp multiplier")]
        private float moveLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("Rotation lerp multiplier")]
        private float rotateLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("Scale lerp multiplier")]
        private float scaleLerpTime = 0;

        [SerializeField]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        private bool maintainScale = true;

        [SerializeField]
        [Tooltip("Working output is smoothed if true. Otherwise, snapped")]
        private bool smoothing = true;

        [SerializeField]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        private float lifetime = 0;

        private float currentLifetime;

        /// <summary>
        /// The handler reference for this solver that's attached to this <see cref="GameObject"/>
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected SolverHandler SolverHandler;

        /// <summary>
        /// The final position to be attained
        /// </summary>
        protected Vector3 GoalPosition;

        /// <summary>
        /// The final rotation to be attained
        /// </summary>
        protected Quaternion GoalRotation;

        /// <summary>
        /// The final scale to be attained
        /// </summary>
        protected Vector3 GoalScale;

        /// <summary>
        /// Automatically uses the shared position if the solver is set to use the 'linked transform'.
        /// UpdateLinkedTransform may be set to false, and a solver will automatically update the object directly,
        /// and not inherit work done by other solvers to the shared position
        /// </summary>
        public Vector3 WorkingPosition
        {
            get => updateLinkedTransform ? SolverHandler.GoalPosition : transform.position;
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalPosition = value;
                }
                else
                {
                    transform.position = value;
                }
            }
        }

        /// <summary>
        /// Rotation version of WorkingPosition
        /// </summary>
        public Quaternion WorkingRotation
        {
            get => updateLinkedTransform ? SolverHandler.GoalRotation : transform.rotation;
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalRotation = value;
                }
                else
                {
                    transform.rotation = value;
                }
            }
        }

        /// <summary>
        /// Scale version of WorkingPosition
        /// </summary>
        public Vector3 WorkingScale
        {
            get => updateLinkedTransform ? SolverHandler.GoalScale : transform.localScale;
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalScale = value;
                }
                else
                {
                    transform.localScale = value;
                }
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void OnValidate()
        {
            if (SolverHandler == null)
            {
                SolverHandler = GetComponent<SolverHandler>();
            }
        }

        protected virtual void Awake()
        {
            if (updateLinkedTransform && SolverHandler == null)
            {
                Debug.LogError("No SolverHandler component found on " + name + " when UpdateLinkedTransform was set to true! Disabling UpdateLinkedTransform.");
                updateLinkedTransform = false;
            }

            GoalScale = maintainScale ? transform.localScale : Vector3.one;
        }

        /// <summary>
        /// Typically when a solver becomes enabled, it should update its internal state to the system, in case it was disabled far away
        /// </summary>
        protected virtual void OnEnable()
        {
            if (SolverHandler != null)
            {
                SnapGoalTo(SolverHandler.GoalPosition, SolverHandler.GoalRotation);
            }

            currentLifetime = 0;
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Should be implemented in derived classes, but Solver can be used to flush shared transform to real transform
        /// </summary>
        public abstract void SolverUpdate();

        /// <summary>
        /// Tracks lifetime of the solver, disabling it when expired, and finally runs the orientation update logic
        /// </summary>
        public void SolverUpdateEntry()
        {
            currentLifetime += SolverHandler.DeltaTime;

            if (lifetime > 0 && currentLifetime >= lifetime)
            {
                enabled = false;
                return;
            }

            SolverUpdate();
        }

        /// <summary>
        /// Snaps the solver to the desired pose.
        /// </summary>
        /// <remarks>
        /// SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned.
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapTo(Vector3 position, Quaternion rotation)
        {
            SnapGoalTo(position, rotation);

            WorkingPosition = position;
            WorkingRotation = rotation;
        }

        /// <summary>
        /// SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        /// <summary>
        /// Add an offset position to the target goal position.
        /// </summary>
        /// <param name="offset"></param>
        public virtual void AddOffset(Vector3 offset)
        {
            GoalPosition += offset;
        }

        /// <summary>
        /// Updates all object orientations to the goal orientation for this solver, with smoothing accounted for (smoothing may be off)
        /// </summary>
        protected void UpdateTransformToGoal()
        {
            var cachedTransform = transform;

            if (smoothing)
            {
                var pos = cachedTransform.position;
                var rot = cachedTransform.rotation;
                var scale = cachedTransform.localScale;

                pos = pos.SmoothTo(GoalPosition, SolverHandler.DeltaTime, moveLerpTime);
                rot = rot.SmoothTo(GoalRotation, SolverHandler.DeltaTime, rotateLerpTime);
                scale = scale.SmoothTo(GoalScale, SolverHandler.DeltaTime, scaleLerpTime);

                cachedTransform.position = pos;
                cachedTransform.rotation = rot;
                cachedTransform.localScale = scale;
            }
            else
            {
                cachedTransform.position = GoalPosition;
                cachedTransform.rotation = GoalRotation;
                cachedTransform.localScale = GoalScale;
            }
        }

        /// <summary>
        /// Updates the Working orientation (which may be the object, or the shared orientation) to the goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingToGoal()
        {
            if (smoothing)
            {
                WorkingPosition = WorkingPosition.SmoothTo(GoalPosition, SolverHandler.DeltaTime, moveLerpTime);
                WorkingRotation = WorkingRotation.SmoothTo(GoalRotation, SolverHandler.DeltaTime, rotateLerpTime);
                WorkingScale = WorkingScale.SmoothTo(GoalScale, SolverHandler.DeltaTime, scaleLerpTime);
            }
            else
            {
                WorkingPosition = GoalPosition;
                WorkingRotation = GoalRotation;
                WorkingScale = GoalScale;
            }
        }

        /// <summary>
        /// Updates only the working position to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingPositionToGoal()
        {
            WorkingPosition = smoothing ? WorkingPosition.SmoothTo(GoalPosition, SolverHandler.DeltaTime, moveLerpTime) : GoalPosition;
        }

        /// <summary>
        /// Updates only the working rotation to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingRotationToGoal()
        {
            WorkingRotation = smoothing ? WorkingRotation.SmoothTo(GoalRotation, SolverHandler.DeltaTime, rotateLerpTime) : GoalRotation;
        }

        /// <summary>
        /// Updates only the working scale to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingScaleToGoal()
        {
            WorkingScale = smoothing ? WorkingScale.SmoothTo(GoalScale, SolverHandler.DeltaTime, scaleLerpTime) : GoalScale;
        }
    }
}