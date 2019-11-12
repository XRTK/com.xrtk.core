// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    /// <summary>
    /// Internal class to define current gesture and smoothly animate hand data points.
    /// </summary>
    [Serializable]
    internal class UnityEditorHandState
    {
        [SerializeField]
        private Handedness handedness = Handedness.None;
        public Handedness Handedness => handedness;

        // Show a tracked hand device
        public bool IsTracked = false;
        // Activate the pinch gesture
        public bool IsPinching { get; private set; }

        public Vector3 ScreenPosition;
        // Rotation of the hand
        public Vector3 HandRotateEulerAngles = Vector3.zero;
        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset = Vector3.zero;

        [SerializeField]
        private string gestureName = null;
        public string GestureName
        {
            get { return gestureName; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value != gestureName)
                {
                    gestureName = value;
                    gestureBlending = 0.0f;
                }
            }
        }

        // Interpolation between current pose and target gesture
        private float gestureBlending = 0.0f;
        public float GestureBlending
        {
            get { return gestureBlending; }
            set
            {
                gestureBlending = Mathf.Clamp(value, gestureBlending, 1.0f);

                // Pinch is a special gesture that triggers the Select and TriggerPress input actions
                // TODO: Probably don't want to have this here.
                //IsPinching = gesture == ArticulatedHandPose.GestureId.Pinch && gestureBlending > 0.9f;
            }
        }

        private float poseBlending = 0.0f;
        private UnityEditorHandPose pose = new UnityEditorHandPose();

        public UnityEditorHandState(Handedness handedness)
        {
            this.handedness = handedness;
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

        public void ResetGesture()
        {
            gestureBlending = 1.0f;

            if (UnityEditorHandPose.TryGetPoseByName(gestureName, out UnityEditorHandPose gesturePose))
            {
                pose.Copy(gesturePose);
            }
        }

        internal void FillCurrentFrame(MixedRealityPose[] jointsOut)
        {
            if (UnityEditorHandPose.TryGetPoseByName(gestureName, out UnityEditorHandPose targetPose))
            {
                if (gestureBlending > poseBlending)
                {
                    float range = Mathf.Clamp01(1.0f - poseBlending);
                    float lerpFactor = range > 0.0f ? (gestureBlending - poseBlending) / range : 1.0f;
                    pose.Lerp(pose, targetPose, lerpFactor);
                }
            }

            poseBlending = gestureBlending;
            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = CameraCache.Main.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            pose.ComputeJointPoses(handedness, rotation, position, jointsOut);
        }
    }
}