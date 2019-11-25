// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// Internal class to define current gesture and smoothly animate hand data points.
    /// </summary>
    [Serializable]
    internal class SimulationHandState
    {
        /// <summary>
        /// Gets the handedness for the hand simulated by this state instance.
        /// </summary>
        public Handedness Handedness { get; } = Handedness.None;

        // Show a tracked hand device
        public bool IsTracked { get; set; } = false;

        // Activate the pinch gesture
        public bool IsPinching { get; private set; }

        public Vector3 ScreenPosition;

        // Rotation of the hand
        public Vector3 HandRotateEulerAngles { get; set; } = Vector3.zero;

        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset { get; set; } = Vector3.zero;

        private string gestureName = null;
        /// <summary>
        /// Currently used gesture name.
        /// </summary>
        public string GestureName
        {
            get => gestureName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, gestureName))
                {
                    gestureName = value;
                    gestureBlending = 0.0f;
                }
            }
        }

        private float gestureBlending = 0.0f;
        /// <summary>
        /// Interpolation between current pose and target gesture.
        /// </summary>
        public float GestureBlending
        {
            get => gestureBlending;
            set => gestureBlending = Mathf.Clamp(value, gestureBlending, 1.0f);
        }

        private float poseBlending = 0.0f;
        private SimulatedHandPose pose;

        public SimulationHandState(Handedness handedness)
        {
            Handedness = handedness;
            pose = new SimulatedHandPose();
        }

        public void Reset()
        {
            ScreenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;

            ResetGesture();
        }

        /// <summary>
        /// Set the position in viewport space rather than screen space (pixels).
        /// </summary>
        public void SetViewportPosition(Vector3 point)
        {
            ScreenPosition = CameraCache.Main.ViewportToScreenPoint(point);
        }

        public void SimulateInput(Vector3 mouseDelta, float noiseAmount, Vector3 rotationDeltaEulerAngles)
        {
            // Apply mouse delta x/y in screen space, but depth offset in world space
            ScreenPosition.x += mouseDelta.x;
            ScreenPosition.y += mouseDelta.y;
            Vector3 newWorldPoint = CameraCache.Main.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += CameraCache.Main.transform.forward * mouseDelta.z;
            ScreenPosition = CameraCache.Main.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rotationDeltaEulerAngles;
            JitterOffset = UnityEngine.Random.insideUnitSphere * noiseAmount;
        }

        /// <summary>
        /// Reset the current hand pose.
        /// </summary>
        private void ResetGesture()
        {
            gestureBlending = 1.0f;
            if (SimulatedHandPose.TryGetPoseByName(gestureName, out SimulatedHandPose gesturePose))
            {
                pose.Copy(gesturePose);
            }
        }

        internal void FillCurrentFrame(MixedRealityPose[] jointsOut)
        {
            if (SimulatedHandPose.TryGetPoseByName(gestureName, out SimulatedHandPose targetPose))
            {
                if (gestureBlending > poseBlending)
                {
                    float range = Mathf.Clamp01(1.0f - poseBlending);
                    float lerpFactor = range > 0.0f ? (gestureBlending - poseBlending) / range : 1.0f;
                    pose = SimulatedHandPose.Lerp(pose, targetPose, lerpFactor);
                }
            }

            poseBlending = gestureBlending;
            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = CameraCache.Main.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            pose.ComputeJointPoses(Handedness, rotation, position, jointsOut);
        }
    }
}