// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for the Unity's Camera class
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Get the horizontal FOV from the stereo camera
        /// </summary>
        public static float GetHorizontalFieldOfViewRadians(this Camera camera)
        {
            return 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * camera.aspect);
        }

        /// <summary>
        /// Returns if a point will be rendered on the screen in either eye
        /// </summary>
        /// <param name="camera">The camera to check the point against</param>
        /// <param name="position"></param>
        /// <returns>True if the position is within the Camera's FOV.</returns>
        public static bool IsInFOV(this Camera camera, Vector3 position)
        {
            float verticalFovHalf = camera.fieldOfView * 0.5f;
            float horizontalFovHalf = camera.GetHorizontalFieldOfViewRadians() * Mathf.Rad2Deg * 0.5f;

            var cameraTransform = camera.transform;
            Vector3 deltaPos = position - cameraTransform.position;
            Vector3 headDeltaPos = cameraTransform.TransformDirection(deltaPos).normalized;

            float yaw = Mathf.Asin(headDeltaPos.x) * Mathf.Rad2Deg;
            float pitch = Mathf.Asin(headDeltaPos.y) * Mathf.Rad2Deg;

            return (Mathf.Abs(yaw) < horizontalFovHalf && Mathf.Abs(pitch) < verticalFovHalf);
        }

        /// <summary>
        /// Gets the frustum size at a given distance from the camera.
        /// </summary>
        /// <param name="camera">The camera to get the frustum size for</param>
        /// <param name="distanceFromCamera">The distance from the camera to get the frustum size at</param>
        public static Vector2 GetFrustumSizeForDistance(this Camera camera, float distanceFromCamera)
        {
            var frustumSize = Vector2.zero;
            frustumSize.y = 2.0f * distanceFromCamera * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumSize.x = frustumSize.y * camera.aspect;
            return frustumSize;
        }

        /// <summary>
        /// Gets the distance to the camera that a specific frustum height would be at.
        /// </summary>
        /// <param name="camera">The camera to get the distance from</param>
        /// <param name="frustumHeight">The frustum height</param>
        public static float GetDistanceForFrustumHeight(this Camera camera, float frustumHeight)
        {
            return frustumHeight * 0.5f / Mathf.Max(0.00001f, Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        }

        /// <summary>
        /// Is the <see cref="Transform"/> inside the <see cref="Camera"/>s frustum?
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static bool IsInFrustum(this Camera camera, Transform transform)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, transform.GetRenderBounds());
        }

        /// <summary>
        /// Is the <see cref="GameObject"/> inside the <see cref="Camera"/>s frustum?
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameObject"></param>
        public static bool IsInFrustum(this Camera camera, GameObject gameObject)
        {
            return IsInFrustum(camera, gameObject.transform);
        }
    }
}