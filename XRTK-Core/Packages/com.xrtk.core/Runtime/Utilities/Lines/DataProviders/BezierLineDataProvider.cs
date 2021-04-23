// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Utilities.Lines.DataProviders;

namespace XRTK.Utilities.Lines
{
    public class BezierLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [Serializable]
        private class BezierInertia
        {
            [SerializeField]
            private float inertia = 4f;

            [SerializeField]
            private float dampen = 5f;

            [SerializeField]
            private float seekTargetStrength = 1f;

            [SerializeField]
            private Vector3 p1Target = new Vector3(0, 0, 0.25f);

            [SerializeField]
            private Vector3 p2Target = new Vector3(0, 0, 0.75f);

            private Vector3 p1Velocity;
            private Vector3 p1Position;
            private Vector3 p1Offset;

            private Vector3 p2Velocity;
            private Vector3 p2Position;
            private Vector3 p2Offset;

            public BezierInertia(Vector2 p1Position, Vector2 p2Position)
            {
                this.p1Position = p1Position;
                this.p2Position = p2Position;
            }

            public void Update(BezierLineDataProvider bezierData)
            {
                var p1BasePoint = bezierData.GetPoint(1);
                var p2BasePoint = bezierData.GetPoint(2);

                var p1WorldTarget = bezierData.LineTransform.TransformPoint(p1Target);
                var p2WorldTarget = bezierData.LineTransform.TransformPoint(p2Target);

                p1Offset = p1BasePoint - p1Position;
                p2Offset = p2BasePoint - p2Position;

                p1Offset += p1WorldTarget - p1Position;
                p2Offset += p2WorldTarget - p2Position;

                p1Velocity = Vector3.Lerp(p1Velocity, p1Offset, Time.deltaTime * inertia);
                p1Velocity = Vector3.Lerp(p1Velocity, Vector3.zero, Time.deltaTime * dampen);

                p2Velocity = Vector3.Lerp(p2Velocity, p2Offset, Time.deltaTime * inertia);
                p2Velocity = Vector3.Lerp(p2Velocity, Vector3.zero, Time.deltaTime * dampen);

                p1Position = p1Position + p1Velocity;
                p2Position = p2Position + p2Velocity;

                p1Position = Vector3.Lerp(p1Position, p1WorldTarget, seekTargetStrength * Time.deltaTime);
                p2Position = Vector3.Lerp(p2Position, p2WorldTarget, seekTargetStrength * Time.deltaTime);

                bezierData.SetPoint(1, p1Position);
                bezierData.SetPoint(2, p2Position);
            }
        }

        [Serializable]
        private struct BezierPointSet
        {
            public BezierPointSet(float spread)
            {
                Point1 = Vector3.right * spread * 0.5f;
                Point2 = Vector3.right * spread * 0.25f;
                Point3 = Vector3.left * spread * 0.25f;
                Point4 = Vector3.left * spread * 0.5f;
            }

            public Vector3 Point1;
            public Vector3 Point2;
            public Vector3 Point3;
            public Vector3 Point4;
        }


        public override int PointCount => 4;

        [SerializeField]
        private BezierInertia inertia;

        [SerializeField]
        private BezierPointSet controlPoints = new BezierPointSet(0.5f);

        [SerializeField]
        [Tooltip("If true, control points 2 and 3 will be transformed relative to points 1 and 4 respectively")]
        private bool useLocalTangentPoints = false;

        private Vector3 localOffset;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (inertia == null)
            {
                inertia = new BezierInertia(GetPoint(1), GetPoint(2));
            }
        }

        protected override void Update()
        {
            base.Update();
            inertia.Update(this);
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return controlPoints.Point1;
                case 1:
                    return controlPoints.Point2;
                case 2:
                    return controlPoints.Point3;
                case 3:
                    return controlPoints.Point4;
                default:
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    localOffset = Vector3.zero;
                    // If we're using local tangent points, apply this change to control point 2
                    if (useLocalTangentPoints)
                    {
                        localOffset = point - controlPoints.Point1;
                    }

                    controlPoints.Point1 = point;
                    controlPoints.Point2 += localOffset;
                    break;
                case 1:
                    controlPoints.Point2 = point;
                    break;
                case 2:
                    controlPoints.Point3 = point;
                    break;
                case 3:
                    localOffset = Vector3.zero;
                    if (useLocalTangentPoints)
                    {
                        localOffset = point - controlPoints.Point4;
                    }

                    controlPoints.Point4 = point;
                    controlPoints.Point3 += localOffset;
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.InterpolateBezierPoints(controlPoints.Point1, controlPoints.Point2, controlPoints.Point3,
                controlPoints.Point4, normalizedDistance);
        }

        protected override float GetUnClampedWorldLengthInternal()
        {
            var distance = 0f;
            var last = GetUnClampedPoint(0f);

            for (int i = 1; i < UnclampedWorldLengthSearchSteps; i++)
            {
                var current = GetUnClampedPoint((float)i / UnclampedWorldLengthSearchSteps);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Bezier up vectors just use transform up
            return transform.up;
        }
    }
}
