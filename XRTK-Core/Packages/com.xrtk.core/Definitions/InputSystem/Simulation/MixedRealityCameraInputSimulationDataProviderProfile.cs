// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Simulation/Camera Input Simulation Profile", fileName = "MixedRealityCameraInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    public class MixedRealityCameraInputSimulationDataProviderProfile : BaseMixedRealityInputSimulationDataProviderProfile
    {
        [Header("Camera Control")]
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
        private InputSimulationMouseButton mouseLookButton = InputSimulationMouseButton.Right;
        public InputSimulationMouseButton MouseLookButton => mouseLookButton;
        [SerializeField]
        private bool isControllerLookInverted = true;
        public bool IsControllerLookInverted => isControllerLookInverted;

        [SerializeField]
        private InputSimulationControlMode currentControlMode = InputSimulationControlMode.Fly;
        public InputSimulationControlMode CurrentControlMode => currentControlMode;
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
    }
}