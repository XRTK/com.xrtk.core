// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;
using Object = UnityEngine.Object;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityControllerDataProvider"/>s
    /// </summary>
    public abstract class BaseControllerDataProvider : BaseDataProvider, IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public BaseControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority)
        {
        }

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <param name="useSpecificType">Only register pointers with a specific type.</param>
        /// <returns></returns>
        protected virtual IMixedRealityPointer[] RequestPointers(SystemType controllerType, Handedness controllingHand, bool useSpecificType = false)
        {
            var pointers = new List<IMixedRealityPointer>();

            if (MixedRealityToolkit.HasActiveProfile &&
                MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile != null)
            {
                for (int i = 0; i < MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointerOptions.Length; i++)
                {
                    var pointerProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointerOptions[i];

                    if (((useSpecificType && pointerProfile.ControllerType.Type == controllerType.Type) || (!useSpecificType && pointerProfile.ControllerType.Type == null)) &&
                        (pointerProfile.Handedness == Handedness.Any || pointerProfile.Handedness == Handedness.Both || pointerProfile.Handedness == controllingHand))
                    {
                        var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, MixedRealityToolkit.CameraSystem?.MainCameraRig.PlayspaceTransform);
                        var pointer = pointerObject.GetComponent<IMixedRealityPointer>();

                        if (pointer != null)
                        {
                            pointers.Add(pointer);
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to attach {pointerProfile.PointerPrefab.name} to {controllerType.Type.Name}.");
                        }
                    }
                }
            }

            return pointers.Count == 0 ? null : pointers.ToArray();
        }

        private readonly List<IMixedRealityController> activeControllers = new List<IMixedRealityController>();

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityController> ActiveControllers => activeControllers;

        protected void AddController(IMixedRealityController controller)
        {
            activeControllers.Add(controller);
        }

        protected void RemoveController(IMixedRealityController controller)
        {
            if (controller != null)
            {
                activeControllers.Remove(controller);
            }
        }
    }
}