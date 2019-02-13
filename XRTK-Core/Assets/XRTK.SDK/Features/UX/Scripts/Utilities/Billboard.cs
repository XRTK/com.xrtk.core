// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;

namespace XRTK.SDK.UX.Utilities
{
    /// <summary>
    /// The Billboard class implements the behaviors needed to keep a GameObject oriented towards the user.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        public PivotAxis PivotAxis
        {
            get => pivotAxis;
            set => pivotAxis = value;
        }

        [SerializeField]
        [Tooltip("Specifies the axis about which the object will rotate.")]
        private PivotAxis pivotAxis = PivotAxis.XY;

        /// <summary>
        /// The target we will orient to. If no target is specified, the main camera will be used.
        /// </summary>
        public Transform TargetTransform
        {
            get => targetTransform = targetTransform == null ? CameraCache.Main.transform : targetTransform;
            set => targetTransform = value == null ? CameraCache.Main.transform : value;
        }

        [SerializeField]
        [Tooltip("Specifies the target we will orient to. If no target is specified, the main camera will be used.")]
        private Transform targetTransform;

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            // Get a Vector that points from the target to the main camera.
            Vector3 directionToTarget = TargetTransform.position - transform.position;

            bool useCameraAsUpVector = true;

            // Adjust for the pivot axis.
            switch (pivotAxis)
            {
                case PivotAxis.X:
                    directionToTarget.x = 0.0f;
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.Y:
                    directionToTarget.y = 0.0f;
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.Z:
                    directionToTarget.x = 0.0f;
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.XY:
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.XZ:
                    directionToTarget.x = 0.0f;
                    break;

                case PivotAxis.YZ:
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.Free:
                default:
                    // No changes needed.
                    break;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }

            // Calculate and apply the rotation required to reorient the object
            transform.rotation = useCameraAsUpVector
                ? Quaternion.LookRotation(-directionToTarget, CameraCache.Main.transform.up)
                : Quaternion.LookRotation(-directionToTarget);
        }
    }
}