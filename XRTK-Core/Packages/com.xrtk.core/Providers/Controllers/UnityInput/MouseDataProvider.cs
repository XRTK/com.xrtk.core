// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;
using XRTK.Utilities.Physics;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// The mouse data provider.
    /// </summary>
    public class MouseDataProvider : BaseControllerDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public MouseDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Input.mousePresent)
            {
                Disable();
                return;
            }

#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            IMixedRealityInputSource mouseInputSource = null;

            MixedRealityRaycaster.DebugEnabled = true;

            if (MixedRealityToolkit.InputSystem != null)
            {
                var pointers = RequestPointers(new SystemType(typeof(MouseController)), Handedness.Any, true);
                mouseInputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource("Mouse Input", pointers);
            }

            Controller = new MouseController(TrackingState.NotApplicable, Handedness.Any, mouseInputSource);

            if (mouseInputSource != null)
            {
                for (int i = 0; i < mouseInputSource.Pointers.Length; i++)
                {
                    mouseInputSource.Pointers[i].Controller = Controller;
                }
            }

            if (!Controller.SetupConfiguration(typeof(MouseController)))
            {
                Debug.LogError($"Failed to configure {typeof(MouseController).Name} controller!");
                return;
            }

            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
            AddController(Controller);
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (Input.mousePresent && Controller == null) { Enable(); }

            Controller?.Update();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
                RemoveController(Controller);
            }
        }
    }
}