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
            epsilon = 0.01f;
            Origin = origin;
            Terminus = terminus;

            dist.x = Terminus.x - Origin.x;
            dist.y = Terminus.y - Origin.y;
            dist.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

            if (Length > 0)
            {
                dir.x = dist.x / Length;
                dir.y = dist.y / Length;
                dir.z = dist.z / Length;
            }
            else
            {
                dir = dist;
            }

            Direction = dir;
        }

        private readonly float epsilon;

        private static Vector3 dist;
        private static Vector3 dir;
        private static Vector3 pos;

        public Vector3 Origin { get; private set; }

        public Vector3 Terminus { get; private set; }

        public Vector3 Direction { get; private set; }

        public float Length { get; private set; }

        public Vector3 GetPoint(float distance)
        {
            if (Length <= distance || Length.Equals(0f))
            {
                return Origin;
            }

            pos.x = Origin.x + Direction.x * distance;
            pos.y = Origin.y + Direction.y * distance;
            pos.z = Origin.z + Direction.z * distance;

            return pos;
        }

        public void UpdateRayStep(Vector3 origin, Vector3 terminus)
        {
            Origin = origin;
            Terminus = terminus;

            dist.x = Terminus.x - Origin.x;
            dist.y = Terminus.y - Origin.y;
            dist.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

            if (Length > 0)
            {
                dir.x = dist.x / Length;
                dir.y = dist.y / Length;
                dir.z = dist.z / Length;
            }
            else
            {
                dir = dist;
            }

            Direction = dir;
        }

        public void CopyRay(Ray ray, float rayLength)
        {
            Length = rayLength;
            Origin = ray.origin;
            Direction = ray.direction;

            pos.x = Origin.x + Direction.x * Length;
            pos.y = Origin.y + Direction.y * Length;
            pos.z = Origin.z + Direction.z * Length;

            Terminus = pos;
        }

        /// <summary>
        /// Does this ray step contain the point provided?
        /// </summary>
        /// <param name="point">The point to find in the ray step.</param>
        /// <returns>True if this ray step contains the point provided.</returns>
        public bool Contains(Vector3 point)
        {
            dist.x = Origin.x - point.x;
            dist.y = Origin.y - point.y;
            dist.z = Origin.z - point.z;
            float sqrMagOriginPoint = (dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z);

            dist.x = point.x - Terminus.x;
            dist.y = point.y - Terminus.y;
            dist.z = point.z - Terminus.z;
            float sqrMagPointTerminus = (dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z);

            float sqrLength = Length * Length;
            float sqrEpsilon = epsilon * epsilon;

            return (sqrMagOriginPoint + sqrMagPointTerminus) - sqrLength > sqrEpsilon;
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
            Vector3 point = Vector3.zero;
            float remainingDistance = distance;
            Debug.Assert(steps != null);
            int numSteps = steps.Length;
            Debug.Assert(numSteps > 0);

            for (int i = 0; i < numSteps; i++)
            {
                if (remainingDistance > numSteps)
                {
                    remainingDistance -= numSteps;
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
                point = steps[numSteps - 1].Terminus;
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

            var step = new RayStep();
            float remainingDistance = distance;
            int numSteps = steps.Length;

            for (int i = 0; i < numSteps; i++)
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