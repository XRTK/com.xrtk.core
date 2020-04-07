// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Used to define a controller or other input device's physical buttons, and other attributes.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerMapping
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description">Description of the Device.</param>
        /// <param name="controllerType">Controller Type to instantiate at runtime.</param>
        /// <param name="handedness">The designated hand that the device is managing.</param>
        /// <param name="useCustomInteractionMappings">Details the list of available buttons / interactions available from the device.</param>
        public MixedRealityControllerMapping(string description, Type controllerType, Handedness handedness = Handedness.None, bool useCustomInteractionMappings = false) : this()
        {
            this.description = description;
            this.controllerType = new SystemType(controllerType);
            this.handedness = handedness;
            this.useCustomInteractionMappings = useCustomInteractionMappings;
            interactions = null;
        }

        [SerializeField]
        private string description;

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType => controllerType;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("Override the default interaction mappings.")]
        private bool useCustomInteractionMappings;

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        public bool HasCustomInteractionMappings => useCustomInteractionMappings;

        [SerializeField]
        [Tooltip("Details the list of available buttons / interactions available from the device.")]
        private MixedRealityInteractionMapping[] interactions;

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        public MixedRealityInteractionMapping[] Interactions => interactions;

        /// <summary>
        /// Default controller mapping that does not contain any interactions.
        /// </summary>
        public static MixedRealityControllerMapping None { get; } = new MixedRealityControllerMapping("None", null);

        /// <summary>
        /// Sets the default interaction mapping based on the current controller type.
        /// </summary>
        internal void SetDefaultInteractionMapping(bool overwrite = false)
        {
            if (Activator.CreateInstance(controllerType, null, TrackingState.NotTracked, handedness, null, null) is BaseController detectedController &&
                (interactions == null || interactions.Length == 0 || overwrite))
            {
                switch (handedness)
                {
                    case Handedness.Left:
                        interactions = detectedController.DefaultLeftHandedInteractions;
                        break;
                    case Handedness.Right:
                        interactions = detectedController.DefaultRightHandedInteractions;
                        break;
                    default:
                        interactions = detectedController.DefaultInteractions;
                        break;
                }
            }
        }

        /// <summary>
        /// Synchronizes the Input Actions of the same physical controller of a different concrete type.
        /// </summary>
        /// <param name="otherControllerMapping"></param>
        internal void SynchronizeInputActions(MixedRealityInteractionMapping[] otherControllerMapping)
        {
            if (otherControllerMapping.Length != interactions.Length)
            {
                Debug.LogError("Controller Input Actions must be the same length!");
                return;
            }

            for (int i = 0; i < interactions.Length; i++)
            {
                interactions[i].MixedRealityInputAction = otherControllerMapping[i].MixedRealityInputAction;
            }
        }
    }
}
