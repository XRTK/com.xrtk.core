// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions.Physics
{
    [Serializable]
    public struct RayStep
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="terminus"></param>
        public RayStep(Vector3 origin, Vector3 terminus) : this()
        {
            Origin = origin;
            Terminus = terminus;
            Length = Vector3.Distance(origin, terminus);
            Direction = (Terminus - Origin).normalized;
            epsilon = 0.01f;
        }

        private readonly float epsilon;

        public Vector3 Origin { get; private set; }

        public Vector3 Terminus { get; private set; }

        public Vector3 Direction { get; private set; }

        public float Length { get; private set; }

        public Vector3 GetPoint(float distance)
        {
            return Vector3.MoveTowards(Origin, Terminus, distance);
        }

        public void UpdateRayStep(Vector3 origin, Vector3 terminus)
        {
            Origin = origin;
            Terminus = terminus;
            Length = Vector3.Distance(origin, terminus);
            Direction = (Terminus - Origin).normalized;
        }

        public void CopyRay(Ray ray, float rayLength)
        {
            Length = rayLength;
            Origin = ray.origin;
            Direction = ray.direction;
            Terminus = Origin + (Direction * Length);
        }

        /// <summary>
        /// Does this ray step contain the point provided?
        /// </summary>
        /// <param name="point">The point to find in the ray step.</param>
        /// <returns>True if this ray step contains the point provided.</returns>
        public bool Contains(Vector3 point)
        {
            return Vector3.Distance(Origin, point) + Vector3.Distance(point, Terminus) - Length < epsilon;
        }

        public static implicit operator Ray(RayStep step)
        {
            return new Ray(step.Origin, step.Direction);
        }

        #region static utility functions

        /// <summary>
        /// Returns a point along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetPointByDistance(RayStep[] steps, float distance)
        {
             Debug.Assert(steps != null);
             Debug.Assert(steps.Length > 0);

            Vector3 point = Vector3.zero;
            float remainingDistance = distance;

            for (int i = 0; i < steps.Length; i++)
            {
                if (remainingDistance > steps[i].Length)
                {
                    remainingDistance -= steps[i].Length;
                }
                else
                {
                    point = Vector3.Lerp(steps[i].Origin, steps[i].Terminus, remainingDistance / steps[i].Length);
                    remainingDistance = 0;
                    break;
                }
            }

            if (remainingDistance > 0)
            {
                // If we reach the end and still have distance left, set the point to the terminus of the last step
                point = steps[steps.Length - 1].Terminus;
            }

            return point;
        }

        /// <summary>
        /// Returns a RayStep along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RayStep GetStepByDistance(RayStep[] steps, float distance)
        {
             Debug.Assert(steps != null);
             Debug.Assert(steps.Length > 0);

            RayStep step = new RayStep();
            float remainingDistance = distance;

            for (int i = 0; i < steps.Length; i++)
            {
                if (remainingDistance > steps[i].Length)
                {
                    remainingDistance -= steps[i].Length;
                }
                else
                {
                    step = steps[i];
                    remainingDistance = 0;
                    break;
                }
            }

            if (remainingDistance > 0)
            {
                // If we reach the end and still have distance left, return the last step
                step = steps[steps.Length - 1];
            }

            return step;
        }

        /// <summary>
        /// Returns a direction along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetDirectionByDistance(RayStep[] steps, float distance)
        {
             Debug.Assert(steps != null);
             Debug.Assert(steps.Length > 0);

            return GetStepByDistance(steps, distance).Direction;
        }

        #endregion
    }
}