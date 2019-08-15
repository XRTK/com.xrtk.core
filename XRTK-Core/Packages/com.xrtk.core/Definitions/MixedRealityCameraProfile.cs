// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions
{
    /// <summary>
    /// This <see cref="ScriptableObject"/> tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Camera Profile", fileName = "MixedRealityCameraProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private bool isCameraPersistent = false;

        /// <summary>
        /// Should the camera be reused in each scene?
        /// If so, then the camera's root will be flagged so it is not destroyed when the scene is unloaded.
        /// </summary>
        public bool IsCameraPersistent => isCameraPersistent;

        [SerializeField]
        [Tooltip("The near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        /// <summary>
        /// The near clipping plane distance for an opaque display.
        /// </summary>
        public float NearClipPlaneOpaqueDisplay
        {
            get => nearClipPlaneOpaqueDisplay;
            internal set => nearClipPlaneOpaqueDisplay = value;
        }

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsOpaqueDisplay = CameraClearFlags.Skybox;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsOpaqueDisplay => cameraClearFlagsOpaqueDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorOpaqueDisplay = Color.black;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorOpaqueDisplay => backgroundColorOpaqueDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for opaque display.")]
        private int opaqueQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for opaque display.
        /// </summary>
        public int OpaqueQualityLevel => opaqueQualityLevel;

        [SerializeField]
        [Tooltip("The near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        /// <summary>
        /// The near clipping plane distance for a transparent display.
        /// </summary>
        public float NearClipPlaneTransparentDisplay => nearClipPlaneTransparentDisplay;

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsTransparentDisplay = CameraClearFlags.SolidColor;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsTransparentDisplay => cameraClearFlagsTransparentDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorTransparentDisplay = Color.clear;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorTransparentDisplay => backgroundColorTransparentDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for transparent display.")]
        private int transparentQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for transparent display.
        /// </summary>
        public int TransparentQualityLevel => transparentQualityLevel;

        #region Editor Camera Controls

        [SerializeField]
        [Tooltip("Enable manual camera control")]
        private bool isCameraControlEnabled = true;
        public bool IsCameraControlEnabled => isCameraControlEnabled;

        [SerializeField]
        private float extraMouseSensitivityScale = 3.0f;
        public float ExtraMouseSensitivityScale => extraMouseSensitivityScale;
        [SerializeField]
        private float defaultMouseSensitivity = 0.1f;
        public float DefaultMouseSensitivity => defaultMouseSensitivity;

        [SerializeField]
        [Tooltip("Controls how mouse look control is activated.")]
        private EditorCameraControlMouseButton mouseLookButton = EditorCameraControlMouseButton.Right;
        public EditorCameraControlMouseButton MouseLookButton => mouseLookButton;
        [SerializeField]
        private bool isControllerLookInverted = true;
        public bool IsControllerLookInverted => isControllerLookInverted;

        [SerializeField]
        private EditorCameraControlMode currentControlMode = EditorCameraControlMode.Fly;
        public EditorCameraControlMode CurrentControlMode => currentControlMode;
        [SerializeField]
        private KeyCode fastControlKey = KeyCode.RightControl;
        public KeyCode FastControlKey => fastControlKey;
        [SerializeField]
        private float controlSlowSpeed = 0.1f;
        public float ControlSlowSpeed => controlSlowSpeed;
        [SerializeField]
        private float controlFastSpeed = 1.0f;
        public float ControlFastSpeed => controlFastSpeed;

        // Input axes  to coordinate with the Input Manager (Project Settings -> Input)

        // Horizontal movement string for keyboard and left stick of game controller
        [SerializeField]
        [Tooltip("Horizontal movement Axis ")]
        private string moveHorizontal = "Horizontal";
        public string MoveHorizontal => moveHorizontal;
        // Vertical movement string for keyboard and left stick of game controller 
        [SerializeField]
        [Tooltip("Vertical movement Axis ")]
        private string moveVertical = "Vertical";
        public string MoveVertical => moveVertical;
        // Mouse movement string for the x-axis
        [SerializeField]
        [Tooltip("Mouse Movement X-axis")]
        private string mouseX = "Mouse X";
        public string MouseX => mouseX;
        // Mouse movement string for the y-axis
        [SerializeField]
        [Tooltip("Mouse Movement Y-axis")]
        private string mouseY = "Mouse Y";
        public string MouseY => mouseY;
        // Look horizontal string for right stick of game controller
        // The right stick has no default settings in the Input Manager and will need to be setup for a game controller to look
        [SerializeField]
        [Tooltip("Look Horizontal Axis - Right Stick On Controller")]
        private string lookHorizontal = ControllerMappingLibrary.AXIS_4;
        public string LookHorizontal => lookHorizontal;
        // Look vertical string for right stick of game controller
        [SerializeField]
        [Tooltip("Look Vertical Axis - Right Stick On Controller ")]
        private string lookVertical = ControllerMappingLibrary.AXIS_5;
        public string LookVertical => lookVertical;

        #endregion
    }
}
