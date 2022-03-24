﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Utilities.Lines.DataProviders
{
    /// <summary>
    /// Creates an elliptical line shape.
    /// </summary>
    /// <remarks>This line loops.</remarks>
    public class EllipseLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        [Range(0, 2048)]
        private int resolution = 36;

        public int Resolution
        {
            get => resolution;
            set => resolution = Mathf.Clamp(value, 0, 2048);
        }

        [SerializeField]
        private Vector2 radius = Vector2.one;

        public Vector2 Radius
        {
            get => radius;
            set
            {
                if (value.x < 0)
                {
                    value.x = 0;
                }

                if (value.y < 0)
                {
                    value.y = 0;
                }

                radius = value;
            }
        }

        #region BaseMixedRealityLineDataProvider Implementation

        /// <inheritdoc />
        public override int PointCount => resolution;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetEllipsePoint(radius, normalizedDistance * 2f * Mathf.PI);
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            float angle = ((float)pointIndex / resolution) * 2f * Mathf.PI;
            return LineUtility.GetEllipsePoint(radius, angle);
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            // Does nothing for an ellipse
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);

            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        #endregion BaseMixedRealityLineDataProvider Implementation
    }
}