﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Utilities.Physics.Distorters
{
    public class DistorterBulge : Distorter
    {
        [SerializeField]
        private Vector3 bulgeLocalCenter = Vector3.zero;

        public Vector3 BulgeLocalCenter
        {
            get => bulgeLocalCenter;
            set => bulgeLocalCenter = value;
        }

        public Vector3 BulgeWorldCenter
        {
            get => transform.TransformPoint(bulgeLocalCenter);
            set => bulgeLocalCenter = transform.InverseTransformPoint(value);
        }

        [SerializeField]
        private AnimationCurve bulgeFalloff = new AnimationCurve();

        public AnimationCurve BulgeFalloff
        {
            get => bulgeFalloff;
            set => bulgeFalloff = value;
        }

        [SerializeField]
        private float bulgeRadius = 1f;

        public float BulgeRadius
        {
            get => bulgeRadius;
            set => bulgeRadius = value < 0f ? 0f : value;
        }

        [SerializeField]
        private float scaleDistort = 2f;

        public float ScaleDistort
        {
            get => scaleDistort;
            set => scaleDistort = value;
        }

        [SerializeField]
        private float bulgeStrength = 1f;

        public float BulgeStrength
        {
            get => bulgeStrength;
            set => bulgeStrength = value;
        }

        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeWorldCenter);

            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                Vector3 direction = (point - BulgeWorldCenter).normalized;
                point = point + (direction * distortion * bulgeStrength);
            }

            return point;
        }

        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeWorldCenter);

            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                return Vector3.one + (Vector3.one * distortion * scaleDistort);
            }

            return Vector3.one;
        }

        private void OnDrawGizmos()
        {
            Vector3 bulgePoint = transform.TransformPoint(bulgeLocalCenter);
            Color gColor = Color.red;
            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawWireSphere(bulgePoint, bulgeRadius);
            const int steps = 8;

            for (int i = 0; i < steps; i++)
            {
                float normalizedStep = (1f / steps) * i;
                gColor.a = (1f - bulgeFalloff.Evaluate(normalizedStep)) * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawSphere(bulgePoint, bulgeRadius * bulgeFalloff.Evaluate(normalizedStep));
            }
        }
    }
}
