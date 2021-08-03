// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    public abstract class BaseTeleportLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The threshold amount for joystick input (Dead Zone).")]
        private float inputThreshold = 0.5f;

        /// <summary>
        /// The threshold amount for joystick input (Dead Zone).
        /// </summary>
        public float InputThreshold
        {
            get => inputThreshold;
            internal set => inputThreshold = value;
        }

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("If Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like the forward direction, we apply this offset to make navigation feel more natural.")]
        private float angleOffset = 0f;

        /// <summary>
        /// If Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like the forward direction, we apply this offset to make navigation feel more natural.
        /// </summary>
        public float AngleOffset
        {
            get => angleOffset;
            internal set => angleOffset = value;
        }

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the pointer's forward position that will activate the teleport.")]
        private float teleportActivationAngle = 45f;

        /// <summary>
        /// The angle from the pointer's forward position that will activate the teleport.
        /// </summary>
        public float TeleportActivationAngle
        {
            get => teleportActivationAngle;
            internal set => teleportActivationAngle = value;
        }

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the joystick left and right position that will activate a rotation.")]
        private float rotateActivationAngle = 22.5f;

        /// <summary>
        /// The angle from the joystick left and right position that will activate a rotation.
        /// </summary>
        public float RotateActivationAngle
        {
            get => rotateActivationAngle;
            internal set => rotateActivationAngle = value;
        }

        [SerializeField]
        [Range(5f, 180f)]
        [Tooltip("The amount to rotate the camera when rotation is activated.")]
        private float rotationAmount = 90f;

        /// <summary>
        /// The amount to rotate the camera when rotation is activated.
        /// </summary>
        public float RotationAmount
        {
            get => rotationAmount;
            internal set => rotationAmount = value;
        }

        [SerializeField]
        [Range(5, 90f)]
        [Tooltip("The angle from the joystick down position that will activate a strafe that will move the camera back.")]
        private float backStrafeActivationAngle = 45f;

        /// <summary>
        /// The angle from the joystick down position that will activate a strafe that will move the camera back.
        /// </summary>
        public float BackStrafeActivationAngle
        {
            get => backStrafeActivationAngle;
            internal set => backStrafeActivationAngle = value;
        }

        [SerializeField]
        [Tooltip("The distance to move the camera when the strafe is activated.")]
        private float strafeAmount = 0.25f;

        /// <summary>
        /// The distance to move the camera when the strafe is activated.
        /// </summary>
        public float StrafeAmount
        {
            get => strafeAmount;
            internal set => strafeAmount = value;
        }
    }
}
