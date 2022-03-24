﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;

namespace XRTK.Utilities.Lines.DataProviders
{
    /// <summary>
    /// Creates a parabolic line based on physics.
    /// </summary>
    public class ParabolaPhysicalLineDataProvider : ParabolaLineDataProvider
    {
        [SerializeField]
        [Vector3Range(-1f, 1f)]
        private Vector3 direction = Vector3.forward;

        public Vector3 Direction
        {
            get => direction;
            set
            {
                direction.x = Mathf.Clamp(value.x, -1f, 1f);
                direction.y = Mathf.Clamp(value.y, -1f, 1f);
                direction.z = Mathf.Clamp(value.z, -1f, 1f);
            }
        }

        [SerializeField]
        private float velocity = 2f;

        public float Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        [SerializeField]
        private float distanceMultiplier = 1f;

        public float DistanceMultiplier
        {
            get => distanceMultiplier;
            set => distanceMultiplier = value;
        }

        [SerializeField]
        private bool useCustomGravity = false;

        public bool UseCustomGravity
        {
            get => useCustomGravity;
            set => useCustomGravity = value;
        }

        [SerializeField]
        private Vector3 gravity = Vector3.down * 9.8f;

        public Vector3 Gravity
        {
            get => gravity;
            set => gravity = value;
        }

        #region Line Data Provider Implementation

        /// <inheritdoc />
        public override int PointCount => 2;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return StartPoint.Position;
                case 1:
                    return GetPointInternal(1f);
                default:
                    Debug.LogError("Invalid point index!");
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Sets the point at index.
        /// </summary>
        /// <remarks>
        /// This specific override doesn't set any points.
        /// </remarks>
        /// <param name="pointIndex"></param>
        /// <param name="point"></param>
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            // Intentionally does nothing. StartPoint is always the base.FirstPoint and EndPoint is always calculated by the physics.
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetPointAlongPhysicalParabola(StartPoint.Position, direction, velocity, useCustomGravity ? gravity : UnityEngine.Physics.gravity, normalizedDistance * distanceMultiplier);
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return Vector3.up;
        }

        #endregion Line Data Provider Implementation
    }
}