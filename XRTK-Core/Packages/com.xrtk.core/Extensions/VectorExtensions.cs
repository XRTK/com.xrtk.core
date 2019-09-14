// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Vector struct
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector2 Mul(this Vector2 value, Vector2 scale)
        {
            return new Vector2(value.x * scale.x, value.y * scale.y);
        }

        public static Vector2 Div(this Vector2 value, Vector2 scale)
        {
            return new Vector2(value.x / scale.x, value.y / scale.y);
        }

        public static Vector3 Mul(this Vector3 value, Vector3 scale)
        {
            return new Vector3(value.x * scale.x, value.y * scale.y, value.z * scale.z);
        }

        public static Vector3 Div(this Vector3 value, Vector3 scale)
        {
            return new Vector3(value.x / scale.x, value.y / scale.y, value.z / scale.z);
        }

        /// <summary>
        /// Rotate a two dimensional point by the specified angle.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angleRadians"></param>
        /// <returns>
        /// The coordinates of the rotated point.
        /// </returns>
        public static Vector2 RotatePoint(this Vector2 point, float angleRadians)
        {
            return point.RotateAroundPoint(Vector2.zero, angleRadians);
        }

        /// <summary>
        /// Rotate a two dimensional point about another point by the specified angle.
        /// </summary>
        /// <param name="point">The point to be rotated.</param>
        /// <param name="pivot">The point about which the rotation is to occur.</param>
        /// <param name="angleRadians">The angle for the rotation, in radians</param>
        /// <returns>
        /// The coordinates of the rotated point.
        /// </returns>
        public static Vector2 RotateAroundPoint(this Vector2 point, Vector2 pivot, float angleRadians)
        {
            if (angleRadians.Equals(0f))
            {
                return point;
            }

            var rotated = point;

            // Translate to origin of rotation
            rotated.x -= pivot.x;
            rotated.y -= pivot.y;

            // Rotate the point
            var sin = Mathf.Sin(angleRadians);
            var cos = Mathf.Cos(angleRadians);
            var x = rotated.x * cos - rotated.y * sin;
            var y = rotated.x * sin + rotated.y * cos;

            // Translate back and return
            rotated.x = x + pivot.x;
            rotated.y = y + pivot.y;

            return rotated;
        }

        /// <summary>
        /// Rotate a three dimensional point about another point by the specified rotation.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pivot"></param>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        /// <returns>
        /// The coordinates of the rotated point.
        /// </returns>
        public static Vector3 RotateAroundPoint(this Vector3 point, Vector3 pivot, float angle, Vector3 axis)
        {
            return point.RotateAroundPoint(pivot, Quaternion.AngleAxis(angle, axis));
        }

        /// <summary>
        /// Rotate a three dimensional point about another point by the specified rotation.
        /// </summary>
        /// <param name="point">The point to be rotated.</param>
        /// <param name="pivot">The point about which the rotation is to occur.</param>
        /// <param name="rotation">The specified rotation.</param>
        /// <returns>
        /// The coordinates of the rotated point.
        /// </returns>
        public static Vector3 RotateAroundPoint(this Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }

        /// <summary>
        /// Rotate a three dimensional point about another point by the specified rotation.
        /// </summary>
        /// <param name="point">The point to be rotated.</param>
        /// <param name="pivot">The point about which the rotation is to occur.</param>
        /// <param name="eulerAngles">The specified rotation.</param>
        /// <returns>
        /// The coordinates of the rotated point.
        /// </returns>
        public static Vector3 RotateAroundPoint(this Vector3 point, Vector3 pivot, Vector3 eulerAngles)
        {
            return RotateAroundPoint(point, pivot, Quaternion.Euler(eulerAngles));
        }

        /// <summary>
        /// Gets the angle of rotation between two directions around the specified axis.
        /// </summary>
        /// <param name="directionA">The first direction.</param>
        /// <param name="directionB">The second direction.</param>
        /// <param name="axis">The axis of rotation.</param>
        /// <returns>
        /// The rotation angle.
        /// </returns>
        public static float AngleAroundAxis(this Vector3 directionA, Vector3 directionB, Vector3 axis)
        {
            directionA -= Vector3.Project(directionA, axis);
            directionB -= Vector3.Project(directionB, axis);
            return Vector3.Angle(directionA, directionB) * (Vector3.Dot(axis, Vector3.Cross(directionA, directionB)) < 0 ? -1 : 1);
        }

        /// <summary>
        /// Transforms a three dimensional point by the specified transform.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="transform"></param>
        /// <returns>
        /// The coordinates of the updated point.
        /// </returns>
        /// <remarks>
        /// This IS NOT the same as <see cref="Transform.TransformPoint(Vector3)"/> which translates a point from local space to world space.
        /// </remarks>
        public static Vector3 TransformPoint(this Vector3 point, Transform transform)
        {
            return point.TransformPoint(transform.position, transform.rotation, transform.localScale);
        }

        /// <summary>
        /// Transforms a three dimensional point by the specified position, rotation, and scale.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="localScale"></param>
        /// <returns>
        /// The coordinates of the updated point.
        /// </returns>
        /// <remarks>
        /// This IS NOT the same as <see cref="Transform.TransformPoint(Vector3)"/> which translates a point from local space to world space.
        /// </remarks>
        public static Vector3 TransformPoint(this Vector3 point, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            return rotation * Vector3.Scale(localScale, point) + position;
        }

        /// <summary>
        /// Inversely Transforms a three dimensional point by the specified transform.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="transform"></param>
        /// <returns>
        /// The coordinates of the updated point.
        /// </returns>
        /// <remarks>
        /// This IS NOT the same as <see cref="Transform.InverseTransformPoint(Vector3)"/> which translates a point from local space to world space.
        /// </remarks>
        public static Vector3 InverseTransformPoint(this Vector3 point, Transform transform)
        {
            return point.InverseTransformPoint(transform.position, transform.rotation, transform.localScale);
        }

        /// <summary>
        /// Transforms a three dimensional point by the specified position, rotation, and scale.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="localScale"></param>
        /// <returns>
        /// The coordinates of the updated point.
        /// </returns>
        /// <remarks>
        /// This IS NOT the same as <see cref="Transform.InverseTransformPoint(Vector3)"/> which translates a point from local space to world space.
        /// </remarks>
        public static Vector3 InverseTransformPoint(this Vector3 point, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            var scaleInv = new Vector3(1 / localScale.x, 1 / localScale.y, 1 / localScale.z);
            return Vector3.Scale(scaleInv, (Quaternion.Inverse(rotation) * (point - position)));
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            var x = 0f;
            var y = 0f;
            int count = 0;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                count++;
            }

            return new Vector2(x / count, y / count);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;
            int count = 0;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
                count++;
            }

            return new Vector3(x / count, y / count, z / count);
        }

        public static Vector2 Average(this ICollection<Vector2> vectors)
        {
            int count = vectors.Count;

            if (count == 0)
            {
                return Vector2.zero;
            }

            var x = 0f;
            var y = 0f;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
            }

            return new Vector2(x / count, y / count);
        }

        public static Vector3 Average(this ICollection<Vector3> vectors)
        {
            int count = vectors.Count;

            if (count == 0)
            {
                return Vector3.zero;
            }

            var x = 0f;
            var y = 0f;
            var z = 0f;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
            }

            return new Vector3(x / count, y / count, z / count);
        }

        public static Vector2 Median(this IEnumerable<Vector2> vectors)
        {
            var enumerable = vectors as Vector2[] ?? vectors.ToArray();
            int count = enumerable.Length;
            return count == 0 ? Vector2.zero : enumerable.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this IEnumerable<Vector3> vectors)
        {
            var enumerable = vectors as Vector3[] ?? vectors.ToArray();
            int count = enumerable.Length;
            return count == 0 ? Vector3.zero : enumerable.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector2 Median(this ICollection<Vector2> vectors)
        {
            int count = vectors.Count;
            return count == 0 ? Vector2.zero : vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this ICollection<Vector3> vectors)
        {
            int count = vectors.Count;
            return count == 0 ? Vector3.zero : vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        /// <summary>
        /// Validates the vector data to ensure that none of the values are <see cref="float.NaN"/>,
        /// <see cref="float.PositiveInfinity"/>, or <see cref="float.NegativeInfinity"/>.
        /// </summary>
        /// <param name="vector">The vector data to verify.</param>
        /// <returns>True, if the vector values are valid.</returns>
        public static bool IsValidVector(this Vector3 vector)
        {
            return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
                   !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for spherical mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to sphere</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the sphere</param>
        /// <returns></returns>
        public static Vector3 SphericalMapping(Vector3 source, float radius)
        {
            var circumference = 2f * Mathf.PI * radius;
            var xAngle = (source.x / circumference) * 360f;
            var yAngle = -(source.y / circumference) * 360f;
            var rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);

            source.Set(0.0f, 0.0f, radius);
            return rotation * source;
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for cylinder mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        public static Vector3 CylindricalMapping(Vector3 source, float radius)
        {
            var circumference = 2f * Mathf.PI * radius;
            var xAngle = (source.x / circumference) * 360f;
            var rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);

            source.Set(0.0f, source.y, radius);

            return rotation * source;
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for radial mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radialRange">The total range of the radial in degrees as a <see cref="float"/></param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the radial</param>
        /// <param name="row">The current row as a <see cref="int"/> for the radial calculation</param>
        /// <param name="totalRows">The total rows as a <see cref="int"/> for the radial calculation</param>
        /// <param name="column">The current column as a <see cref="int"/> for the radial calculation</param>
        /// <param name="totalColumns">The total columns as a <see cref="int"/> for the radial calculation</param>
        /// <param name="offset">The offset to use to adjust the start position of the items in the radial.</param>
        /// <returns>The new position for the row/column.</returns>
        public static Vector3 RadialMapping(Vector3 source, float radialRange, float radius, int row, int totalRows, int column, int totalColumns, float offset = 0.0f)
        {
            var radialCellAngle = radialRange / totalColumns;

            source.x = 0f;
            source.y = 0f;
            source.z = (radius / totalRows) * row;
            var yAngle = radialCellAngle * (column - (totalColumns * offset)) + (radialCellAngle * offset);
            var rotation = Quaternion.Euler(0.0f, yAngle, 0.0f);
            return rotation * source;
        }

        /// <summary>
        /// Randomized mapping based on a source position and a radius for randomization distance.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        public static Vector3 ScatterMapping(this Vector3 source, float radius)
        {
            source.x = Random.Range(-radius, radius);
            source.y = Random.Range(-radius, radius);
            return source;
        }

        /// <summary>
        /// Determine the move direction based off of the direction provided
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="deadZone"></param>
        /// <returns></returns>
        public static MoveDirection DetermineMoveDirection(this Vector2 direction, float deadZone = 0.6f)
        {
            if (direction.sqrMagnitude < deadZone * deadZone)
            {
                return MoveDirection.None;
            }

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }

            return direction.y > 0 ? MoveDirection.Up : MoveDirection.Down;
        }

        /// <summary>
        /// Checks if a normal is nearly vertical
        /// </summary>
        /// <param name="normal"></param>
        /// <returns>Returns true, if normal is vertical.</returns>
        public static bool IsNormalVertical(this Vector3 normal) => 1f - Mathf.Abs(normal.y) < 0.01f;

        /// <summary>
        /// Lerps Vector3 source to goal.
        /// </summary>
        /// <remarks>
        /// Handles lerpTime of 0.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Vector3 SmoothTo(this Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Returns the midpoint between two vectors.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="point"></param>
        public static Vector2 MidPoint(this Vector2 source, Vector2 point)
        {
            return (source + point) * 0.5f;
        }

        /// <summary>
        /// Returns the midpoint between two vectors.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="point"></param>
        public static Vector3 MidPoint(this Vector3 source, Vector3 point)
        {
            return (source + point) * 0.5f;
        }
    }
}