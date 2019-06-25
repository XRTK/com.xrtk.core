// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Services.InputSystem.Simulation;
using XRTK.Utilities;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class CameraSimulationDataProvider : BaseSimulationDataProvider
    {
        private MixedRealityCameraInputSimulationDataProviderProfile profile;
        private bool isMouseJumping = false;
        private bool isGamepadLookEnabled = true;
        private bool isFlyKeypressEnabled = true;
        private Vector3 lastMousePosition = Vector3.zero;
        private Vector3 lastTrackerToUnityTranslation = Vector3.zero;
        private Quaternion lastTrackerToUnityRotation = Quaternion.identity;
        private bool wasLooking = false;
        private bool wasCursorVisible = true;

        public CameraSimulationDataProvider(string name, uint priority, MixedRealityCameraInputSimulationDataProviderProfile profile) : base(name, priority)
        {
            this.profile = profile;
        }

        public override void Update()
        {
            if (profile.IsCameraControlEnabled && CameraCache.Main)
            {
                UpdateTransform(CameraCache.Main.transform);
            }
        }

        private static float InputCurve(float x)
        {
            // smoothing input curve, converts from [-1,1] to [-2,2]
            return Mathf.Sign(x) * (1.0f - Mathf.Cos(0.5f * Mathf.PI * Mathf.Clamp(x, -1.0f, 1.0f)));
        }

        public void UpdateTransform(Transform transform)
        {
            // Undo the last tracker to Unity transforms applied
            transform.Translate(-lastTrackerToUnityTranslation, Space.World);
            transform.Rotate(-lastTrackerToUnityRotation.eulerAngles, Space.World);

            // Calculate and apply the camera control movement this frame
            Vector3 rotate = GetCameraControlRotation();
            Vector3 translate = GetCameraControlTranslation(transform);

            transform.Rotate(rotate.x, 0.0f, 0.0f);
            transform.Rotate(0.0f, rotate.y, 0.0f, Space.World);
            transform.Translate(translate, Space.World);

            transform.Rotate(lastTrackerToUnityRotation.eulerAngles, Space.World);
            transform.Translate(lastTrackerToUnityTranslation, Space.World);
        }

        private static float GetKeyDir(string neg, string pos)
        {
            return Input.GetKey(neg) ? -1.0f : Input.GetKey(pos) ? 1.0f : 0.0f;
        }

        private Vector3 GetCameraControlTranslation(Transform transform)
        {
            Vector3 deltaPosition = Vector3.zero;

            // Support fly up/down keypresses if the current project maps it. This isn't a standard
            // Unity InputManager mapping, so it has to gracefully fail if unavailable.
            if (isFlyKeypressEnabled)
            {
                try
                {
                    deltaPosition += InputCurve(Input.GetAxis("Fly")) * transform.up;
                }
                catch (System.Exception)
                {
                    isFlyKeypressEnabled = false;
                }
            }
            else
            {
                // use page up/down in this case
                deltaPosition += GetKeyDir("page down", "page up") * Vector3.up;
            }

            deltaPosition += InputCurve(Input.GetAxis(profile.MoveHorizontal)) * transform.right;

            if (profile.CurrentControlMode == InputSimulationControlMode.Walk)
            {
                deltaPosition += InputCurve(Input.GetAxis(profile.MoveVertical)) * new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            }
            else
            {
                deltaPosition += InputCurve(Input.GetAxis(profile.MoveVertical)) * transform.forward;
            }

            float accel = Input.GetKey(profile.FastControlKey) ? profile.ControlFastSpeed : profile.ControlSlowSpeed;
            return accel * deltaPosition;
        }

        private Vector3 GetCameraControlRotation()
        {
            float inversionFactor = profile.IsControllerLookInverted ? -1.0f : 1.0f;

            Vector3 rot = Vector3.zero;

            if (isGamepadLookEnabled)
            {
                try
                {
                    // Get the axes information from the right stick of X360 controller
                    rot.x += InputCurve(Input.GetAxis(profile.LookVertical)) * inversionFactor;
                    rot.y += InputCurve(Input.GetAxis(profile.LookHorizontal));
                }
                catch (System.Exception)
                {
                    isGamepadLookEnabled = false;
                }
            }

            if (ShouldMouseLook)
            {
                if (!wasLooking)
                {
                    OnStartMouseLook();
                }

                ManualCameraControl_MouseLookTick(ref rot);

                wasLooking = true;
            }
            else
            {
                if (wasLooking)
                {
                    OnEndMouseLook();
                }

                wasLooking = false;
            }

            rot *= profile.ExtraMouseSensitivityScale;

            return rot;
        }

        private void OnStartMouseLook()
        {
            if (profile.MouseLookButton <= InputSimulationMouseButton.Middle)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(true);
            }
            else if (profile.MouseLookButton <= InputSimulationMouseButton.Focused)
            {
                // if mousebutton is either control, shift or focused
                Cursor.lockState = CursorLockMode.Locked;
                // save current cursor visibility before hiding it
                wasCursorVisible = Cursor.visible;
                Cursor.visible = false;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void OnEndMouseLook()
        {
            if (profile.MouseLookButton <= InputSimulationMouseButton.Middle)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(false);
            }
            else if (profile.MouseLookButton <= InputSimulationMouseButton.Focused)
            {
                // if mousebutton is either control, shift or focused
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = wasCursorVisible;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void ManualCameraControl_MouseLookTick(ref Vector3 rot)
        {
            // Use frame-to-frame mouse delta in pixels to determine mouse rotation. The traditional
            // GetAxis("Mouse X") method doesn't work under Remote Desktop.
            Vector3 mousePositionDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                mousePositionDelta.x = Input.GetAxis(profile.MouseX);
                mousePositionDelta.y = Input.GetAxis(profile.MouseY);
            }
            else
            {
                mousePositionDelta.x *= profile.DefaultMouseSensitivity;
                mousePositionDelta.y *= profile.DefaultMouseSensitivity;
            }

            rot.x += -InputCurve(mousePositionDelta.y);
            rot.y += InputCurve(mousePositionDelta.x);
        }

        private bool ShouldMouseLook
        {
            get
            {
                // Only allow the mouse to control rotation when Unity has focus. This enables
                // the player to temporarily alt-tab away without having the player look around randomly
                // back in the Unity Game window.
                if (!Application.isFocused)
                {
                    return false;
                }
                else if (profile.MouseLookButton == InputSimulationMouseButton.None)
                {
                    return true;
                }
                else if (profile.MouseLookButton <= InputSimulationMouseButton.Middle)
                {
                    return Input.GetMouseButton((int)profile.MouseLookButton);
                }
                else if (profile.MouseLookButton == InputSimulationMouseButton.Control)
                {
                    return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                }
                else if (profile.MouseLookButton == InputSimulationMouseButton.Shift)
                {
                    return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                }
                else if (profile.MouseLookButton == InputSimulationMouseButton.Focused)
                {
                    if (!wasLooking)
                    {
                        // any kind of click will capture focus
                        return Input.GetMouseButtonDown((int)InputSimulationMouseButton.Left) || Input.GetMouseButtonDown((int)InputSimulationMouseButton.Right) || Input.GetMouseButtonDown((int)InputSimulationMouseButton.Middle);
                    }
                    else
                    {
                        // pressing escape will stop capture
                        return !Input.GetKeyDown(KeyCode.Escape);
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Mouse jumping is where the mouse cursor appears outside the Unity game window, but
        /// disappears when it enters the Unity game window.
        /// </summary>
        /// <param name="wantsJumping">Show the cursor</param>
        private void SetWantsMouseJumping(bool wantsJumping)
        {
            if (wantsJumping != isMouseJumping)
            {
                isMouseJumping = wantsJumping;

                if (wantsJumping)
                {
                    // unlock the cursor if it was locked
                    Cursor.lockState = CursorLockMode.None;

                    // save original state of cursor before hiding
                    wasCursorVisible = Cursor.visible;
                    // hide the cursor
                    Cursor.visible = false;

                    lastMousePosition = Input.mousePosition;
                }
                else
                {
                    // recenter the cursor (setting lockCursor has side-effects under the hood)
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.None;

                    // restore the cursor
                    Cursor.visible = wasCursorVisible;
                }

#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.SetWantsMouseJumping(wantsJumping ? 1 : 0);
#endif
            }
        }
    }
}