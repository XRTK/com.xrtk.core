// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Definitions.Physics
{
    /// <summary>
    /// The RayStep is a single ray cast step along the entire distance of a pointer's raycast result.
    /// This struct helps facilitate the parabolic and complex raycasting solutions needed for more interesting raycast problems.
    /// </summary>
    [Serializable]
    public struct RayStep
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="distance"></param>
        public RayStep(Ray ray, float distance) : this()
        {
            CopyRay(ray, distance);
        }

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

            newDistance.x = Terminus.x - Origin.x;
            newDistance.y = Terminus.y - Origin.y;
            newDistance.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((newDistance.x * newDistance.x) + (newDistance.y * newDistance.y) + (newDistance.z * newDistance.z));

            if (Length > 0f)
            {
                newDirection.x = newDistance.x / Length;
                newDirection.y = newDistance.y / Length;
                newDirection.z = newDistance.z / Length;
            }
            else
            {
                Length = epsilon;
                newDirection = newDistance;
            }

            Debug.Assert(Length > 0f);
            Direction = newDirection;
        }

        private readonly float epsilon;

        private static Vector3 newDistance;
        private static Vector3 newDirection;
        private static Vector3 pos;

        /// <summary>
        /// The origin of the ray step.
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// The terminus of the ray step.
        /// </summary>
        public Vector3 Terminus { get; private set; }

        /// <summary>
        /// The direction of the ray step.
        /// </summary>
        public Vector3 Direction { get; private set; }

        /// <summary>
        /// The length of the ray step.
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        /// Gets the point along the ray step at the specified <see cref="distance"/>
        /// </summary>
        /// <param name="distance"></param>
        /// <returns>The point at the specified distance.</returns>
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

        /// <summary>
        /// Update current ray step with new origin and terminus points.
        /// Pass by ref to avoid unnecessary struct copy into function since values will be copied anyways locally
        /// </summary>
        /// <param name="origin">beginning of ray step origin</param>
        /// <param name="terminus">end of ray step</param>
        public void UpdateRayStep(ref Vector3 origin, ref Vector3 terminus)
        {
            Origin = origin;
            Terminus = terminus;

            newDistance.x = Terminus.x - Origin.x;
            newDistance.y = Terminus.y - Origin.y;
            newDistance.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((newDistance.x * newDistance.x) + (newDistance.y * newDistance.y) + (newDistance.z * newDistance.z));

            if (Length > 0f)
            {
                newDirection.x = newDistance.x / Length;
                newDirection.y = newDistance.y / Length;
                newDirection.z = newDistance.z / Length;
            }
            else
            {
                newDirection = newDistance;
            }

            Direction = newDirection;
        }

        /// <summary>
        /// Copies the specified <see cref="Ray"/> data and applies it to the ray step.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="rayLength"></param>
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
            newDistance.x = Origin.x - point.x;
            newDistance.y = Origin.y - point.y;
            newDistance.z = Origin.z - point.z;
            var sqrMagOriginPoint = (newDistance.x * newDistance.x) + (newDistance.y * newDistance.y) + (newDistance.z * newDistance.z);

            newDistance.x = point.x - Terminus.x;
            newDistance.y = point.y - Terminus.y;
            newDistance.z = point.z - Terminus.z;
            var sqrMagPointTerminus = (newDistance.x * newDistance.x) + (newDistance.y * newDistance.y) + (newDistance.z * newDistance.z);

            var sqrLength = Length * Length;
            var sqrEpsilon = epsilon * epsilon;

            return (sqrMagOriginPoint + sqrMagPointTerminus) - sqrLength > sqrEpsilon;
        }

        /// <summary>
        /// Creates a new <see cref="Ray"/> for this ray step.
        /// </summary>
        /// <param name="step"></param>
        public static implicit operator Ray(RayStep step)
        {
            return new Ray(step.Origin, step.Direction);
        }

        #region Static utility functions

        /// <summary>
        /// Returns a point along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetPointByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0f);

            var (rayStep, remainingDistance) = GetStepByDistance(steps, distance);

            return remainingDistance > 0f
                ? Vector3.Lerp(rayStep.Origin, rayStep.Terminus, remainingDistance / rayStep.Length)
                : rayStep.Terminus;
        }

        /// <summary>
        /// Returns a RayStep along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static (RayStep rayStep, float traveledDistance) GetStepByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null && steps.Length > 0);

            var traveledDistance = 0f;

            foreach (var step in steps)
            {
                var stepLength = step.Length;

                if (distance > traveledDistance + stepLength)
                {
                    traveledDistance += stepLength;
                }
                else
                {
                    return (step, Mathf.Clamp(distance - traveledDistance, 0f, stepLength));
                }
            }

            return (steps[steps.Length - 1], 0f);
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

            return GetStepByDistance(steps, distance).rayStep.Direction;
        }

        #endregion Static utility functions
    }
}